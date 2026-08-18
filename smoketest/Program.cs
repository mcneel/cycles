using System;
using System.IO;
using ccl;

// Minimal end-to-end check of the Cycles 5.2 build: initialise, create a
// session and scene, render a few samples on the CPU and write the result out.
// Not a correctness test - it exercises device init, kernel load, sampling and
// buffer readback, which is everything between "it links" and "it renders".
internal static class Program
{
    // keep the delegate alive across the native call boundary
    private static CSycles.LoggerCallback s_logger;
    private static void Main()
    {
        string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? ".";
        string userpath = Path.Combine(path, "userpath");
        Directory.CreateDirectory(userpath);

        Console.WriteLine("path_init  : " + path);
        CSycles.path_init(path, userpath);
        CSycles.initialise(DeviceTypeMask.CPU);
        CSycles.log_to_stdout(true);
        s_logger = (msg) => Console.WriteLine("[ccycles] " + msg);
        CSycles.set_logger(s_logger);
        Console.WriteLine("devices    : " + CSycles.number_devices());
        for (int i = 0; i < CSycles.number_devices(); i++)
            Console.WriteLine("   [" + i + "] " + CSycles.device_decription(i));

        const uint W = 160, H = 120;

        IntPtr sp = CSycles.session_params_create(0);
        CSycles.session_params_set_samples(sp, 4);

        IntPtr session = CSycles.session_create(sp);
        Console.WriteLine("session    : " + (session != IntPtr.Zero ? "created" : "NULL"));
        // NOTE: scene_create is legacy - csycles has it behind #if SCENESTUFF with
        // the comment that scenes are created by the ccl::Session constructor now.

        CSycles.camera_set_size(session, W, H);
        CSycles.camera_compute_auto_viewplane(session);
        CSycles.camera_update(session);

        CSycles.session_add_pass(session, PassType.Combined);
        CSycles.session_set_samples(session, 4);

        if (Environment.GetEnvironmentVariable("SMOKE_NOSTART") == "1")
        {
            Console.WriteLine("skipping start");
            CSycles.session_destroy(session);
            CSycles.shutdown();
            Console.WriteLine("clean exit without start");
            return;
        }
        int rc = CSycles.session_reset(session, (int)W, (int)H, 4, 0, 0, (int)W, (int)H, 1);
        Console.WriteLine("reset      : rc=" + rc);

        Console.WriteLine("rendering  ...");
        CSycles.session_start(session);

        // Poll rather than session_wait, so a stalled render is visible instead
        // of hanging.
        var sw = System.Diagnostics.Stopwatch.StartNew();
        float progress = 0.0f;
        int lastSample = -1;
        while (sw.Elapsed.TotalSeconds < 60.0)
        {
            CSycles.progress_get_progress(session, out progress);
            int sample = CSycles.progress_get_sample(session);
            if (sample != lastSample)
            {
                Console.WriteLine("  sample " + sample + "  progress " + progress.ToString("P1") +
                                  "  status " + CSycles.progress_get_status(session));
                lastSample = sample;
            }
            if (progress >= 1.0f) break;
            System.Threading.Thread.Sleep(250);
        }
        Console.WriteLine("render     : progress=" + progress.ToString("P1") + " after " +
                          sw.Elapsed.TotalSeconds.ToString("F1") + "s");
        CSycles.session_cancel(session, "done");

        IntPtr pixels = IntPtr.Zero;
        int pixelSize = 0;
        CSycles.session_retain_float_buffer(session, PassType.Combined, (int)W, (int)H, ref pixels, ref pixelSize);
        Console.WriteLine("buffer     : ptr=" + (pixels != IntPtr.Zero ? "ok" : "NULL") + " stride=" + pixelSize);

        if (pixels != IntPtr.Zero && pixelSize > 0)
        {
            int n = (int)(W * H) * pixelSize;
            float[] buf = new float[n];
            System.Runtime.InteropServices.Marshal.Copy(pixels, buf, 0, n);

            double sum = 0.0;
            for (int i = 0; i < n; i++) sum += buf[i];
            Console.WriteLine("buffer sum : " + sum.ToString("F4"));

            string ppm = Path.Combine(path, "smoketest.ppm");
            using (var fs = new FileStream(ppm, FileMode.Create))
            using (var w = new StreamWriter(fs))
            {
                w.Write("P3\n" + W + " " + H + "\n255\n");
                for (int i = 0; i < (int)(W * H); i++)
                {
                    int o = i * pixelSize;
                    int r = (int)(Math.Min(1.0f, Math.Max(0.0f, buf[o])) * 255);
                    int g = pixelSize > 1 ? (int)(Math.Min(1.0f, Math.Max(0.0f, buf[o + 1])) * 255) : r;
                    int b = pixelSize > 2 ? (int)(Math.Min(1.0f, Math.Max(0.0f, buf[o + 2])) * 255) : r;
                    w.Write(r + " " + g + " " + b + "\n");
                }
            }
            Console.WriteLine("wrote      : " + ppm);
            CSycles.session_release_float_buffer(session, PassType.Combined);
        }

        CSycles.session_destroy(session);
        CSycles.shutdown();
        Console.WriteLine("done");
    }
}
