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

        // ---- a real scene -------------------------------------------------
        // A ground quad lit by a point light above it. This is what exercises
        // the two riskiest parts of the port: the mesh upload (vertices moved
        // into ATTR_STD_POSITION as packed_float3, and Mesh::add_triangle is
        // gone) and CCyclesLight, which now has to build the light's Object
        // transform from co/dir because 5.2 dropped those sockets.
        IntPtr diffuse = CSycles.create_shader(session);
        CSycles.shader_new_graph(diffuse);
        IntPtr bsdf = CSycles.add_shader_node(diffuse, "diffuse_bsdf", "diff");
        IntPtr outNode = CSycles.add_shader_node(diffuse, "output", "out");
        bool connected = CSycles.shader_connect_nodes(diffuse, bsdf, "BSDF", outNode, "Surface");
        Console.WriteLine("shader     : connected=" + connected);

        IntPtr mesh = CSycles.scene_add_mesh(session, diffuse);
        float[] verts = new float[] {
            -5f, -5f, 0f,
             5f, -5f, 0f,
             5f,  5f, 0f,
            -5f,  5f, 0f,
        };
        int[] tris = new int[] { 0, 1, 2, 0, 2, 3 };
        CSycles.mesh_set_verts(session, mesh, ref verts, 4);
        CSycles.mesh_set_tris(session, mesh, ref tris, 2, diffuse, false);
        Console.WriteLine("mesh       : 4 verts, 2 tris");

        IntPtr obj = CSycles.scene_add_object(session);
        CSycles.object_set_geometry(session, obj, mesh);
        CSycles.object_set_matrix(session, obj, new Transform(
            1f, 0f, 0f, 0f,
            0f, 1f, 0f, 0f,
            0f, 0f, 1f, 0f));
        Console.WriteLine("object     : added");

        IntPtr lightShader = CSycles.create_shader(session);
        CSycles.shader_new_graph(lightShader);
        IntPtr emission = CSycles.add_shader_node(lightShader, "emission", "emit");
        IntPtr lightOut = CSycles.add_shader_node(lightShader, "output", "lout");
        CSycles.shader_connect_nodes(lightShader, emission, "Emission", lightOut, "Surface");

        IntPtr light = CSycles.create_light(session, lightShader);
        CSycles.light_set_type(session, light, LightType.Point);
        CSycles.light_set_co(session, light, 0f, 0f, 6f);
        CSycles.light_set_dir(session, light, 0f, 0f, -1f);
        CSycles.light_set_size(session, light, 1.0f);
        CSycles.light_tag_update(session, light);
        Console.WriteLine("light      : point at (0,0,6)");

        // Camera above the quad, looking down -Z.
        CSycles.camera_set_matrix(session, new Transform(
            1f, 0f, 0f, 0f,
            0f, 1f, 0f, 0f,
            0f, 0f, 1f, 12f));
        CSycles.camera_set_type(session, CameraType.Perspective);
        CSycles.camera_set_fov(session, 0.8f);
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
            // pixelSize is the reset pixel_size (supersampling), not the component
            // count. The Combined pass is RGBA.
            const int comps = 4;
            int n = (int)(W * H) * comps;
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
                    int o = i * comps;
                    int r = (int)(Math.Min(1.0f, Math.Max(0.0f, buf[o])) * 255);
                    int g = (int)(Math.Min(1.0f, Math.Max(0.0f, buf[o + 1])) * 255);
                    int b = (int)(Math.Min(1.0f, Math.Max(0.0f, buf[o + 2])) * 255);
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
