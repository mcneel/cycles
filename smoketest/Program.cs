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

    private static IntPtr FindOutputNode(IntPtr shader)
    {
        int count = CSycles.shader_node_count(shader);
        for (int i = 0; i < count; i++)
        {
            IntPtr n = CSycles.shader_node_get(shader, i);
            string name = CSycles.shadernode_get_name(n);
            Console.WriteLine("  graph node " + i + ": " + name);
            if (name.IndexOf("output", StringComparison.OrdinalIgnoreCase) >= 0)
                return n;
        }
        return IntPtr.Zero;
    }
    [System.Runtime.InteropServices.DllImport("ccycles", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
    private static extern void cycles_debug_scene_stats(IntPtr session);

    [System.Runtime.InteropServices.DllImport("ccycles", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
    private static extern void cycles_debug_install_crash_handler();

    private static void Main()
    {
        string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? ".";
        string userpath = Path.Combine(path, "userpath");
        Directory.CreateDirectory(userpath);

        Console.WriteLine("path_init  : " + path);
        cycles_debug_install_crash_handler();
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
        // emission rather than diffuse: self-lit, so visibility does not depend
        // on the light transform being right.
        // SMOKE_EMIT drives emission instead of a BSDF, so a texture's output
        // lands in the pixels directly. Under diffuse it is multiplied by the
        // light and quantises to almost nothing, which is why patterns looked
        // flat and told us nothing about whether the node actually works.
        bool emit = Environment.GetEnvironmentVariable("SMOKE_EMIT") == "1";
        IntPtr bsdf = CSycles.add_shader_node(diffuse, emit ? "emission" : "diffuse_bsdf", "surf");
        if (emit) {
            // Strength is not 1 by default here, so an emission surface with only
            // its Color wired renders black - which looked exactly like a node that
            // produces nothing.
            CSycles.shadernode_set_member_float(bsdf, "strength", 4.0f);
        }

        // SMOKE_NODE drives the surface colour from one Rhino shader node, which
        // is how the Rhino SVM nodes get exercised at all - nothing else in this
        // harness reaches them. One node per process so a crash in one does not
        // hide the rest.
        string nodeName = Environment.GetEnvironmentVariable("SMOKE_NODE");
        if (!string.IsNullOrEmpty(nodeName)) {
            IntPtr tex = CSycles.add_shader_node(diffuse, nodeName, "tex");
            Console.WriteLine("node       : " + nodeName + " created=" + (tex != IntPtr.Zero));
            if (tex == IntPtr.Zero) { Console.WriteLine("RESULT     : NOT-CREATED"); return; }
            // Rhino textures pattern off their UVW input; left unconnected they
            // evaluate at a single point and come out flat, which would let a
            // broken SVM encoding pass unnoticed.
            IntPtr texco = CSycles.add_shader_node(diffuse, "texture_coordinate", "texco");
            foreach (string inName in new[] { "UVW", "Vector", "UVW1" }) {
                if (CSycles.shader_connect_nodes(diffuse, texco, "Generated", tex, inName)) {
                    Console.WriteLine("node       : uvw via " + inName);
                    break;
                }
            }

            bool wired = false;
            foreach (string outName in new[] { "Color", "Vector", "UVW1", "Alpha" }) {
                if (CSycles.shader_connect_nodes(diffuse, tex, outName, bsdf, "Color")) {
                    Console.WriteLine("node       : wired via " + outName);
                    wired = true;
                    break;
                }
            }
            if (!wired) { Console.WriteLine("RESULT     : NOT-WIRED"); return; }
        }
        // shader_new_graph already creates the graph's output node; adding another
        // gives an orphan that nothing reads.
        IntPtr outNode = FindOutputNode(diffuse);
        bool connected = CSycles.shader_connect_nodes(diffuse, bsdf, emit ? "Emission" : "BSDF", outNode, "Surface");
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

        if (Environment.GetEnvironmentVariable("SMOKE_NOMESH") != "1") {
        IntPtr obj = CSycles.scene_add_object(session);
        CSycles.object_set_geometry(session, obj, mesh);
        CSycles.object_set_matrix(session, obj, new Transform(
            1f, 0f, 0f, 0f,
            0f, 1f, 0f, 0f,
            0f, 0f, 1f, 0f));
        Console.WriteLine("object     : added");
        }

        IntPtr lightShader = CSycles.create_shader(session);
        CSycles.shader_new_graph(lightShader);
        IntPtr emission = CSycles.add_shader_node(lightShader, "emission", "emit");
        IntPtr lightOut = FindOutputNode(lightShader);
        CSycles.shader_connect_nodes(lightShader, emission, "Emission", lightOut, "Surface");

        if (Environment.GetEnvironmentVariable("SMOKE_NOLIGHT") != "1") {
        IntPtr light = CSycles.create_light(session, lightShader);
        // SMOKE_SPOTZ exercises the light basis rather than just its position:
        // a point light only uses the translation of the object transform, a spot
        // light has to be pointed the right way as well.
        string spotDir = Environment.GetEnvironmentVariable("SMOKE_SPOTZ");
        bool spot = spotDir != null;
        bool area = Environment.GetEnvironmentVariable("SMOKE_AREA") == "1";
        CSycles.light_set_type(session, light, area ? LightType.Area : spot ? LightType.Spot : LightType.Point);
        float lightX = float.Parse(Environment.GetEnvironmentVariable("SMOKE_LIGHTX") ?? "4",
            System.Globalization.CultureInfo.InvariantCulture);
        float lightZ = float.Parse(Environment.GetEnvironmentVariable("SMOKE_LIGHTZ") ?? "-6",
            System.Globalization.CultureInfo.InvariantCulture);
        CSycles.light_set_co(session, light, lightX, 0f, lightZ);
        CSycles.light_set_dir(session, light, 0f, 0f, spot ? float.Parse(spotDir,
            System.Globalization.CultureInfo.InvariantCulture) : 1f);
        if (area) {
            // A 4x4 rectangle in the XY plane, so its axes are unambiguous and
            // any mix-up between them and dir shows as a misplaced or dark quad.
            CSycles.light_set_axisu(session, light, 1f, 0f, 0f);
            CSycles.light_set_axisv(session, light, 0f, 1f, 0f);
            CSycles.light_set_sizeu(session, light, 4f);
            CSycles.light_set_sizev(session, light, 4f);
        }
        if (spot) {
            CSycles.light_set_spot_angle(session, light, 1.2f);
            CSycles.light_set_spot_smooth(session, light, 0.1f);
        }
        CSycles.light_set_size(session, light, 1.0f);
        CSycles.light_tag_update(session, light);
        Console.WriteLine("light      : at (" + lightX + ",0," + lightZ + ")");
        }

        // Black world: the default background is a random colour that swamps
        // anything the light contributes.
        IntPtr worldShader = CSycles.create_shader(session);
        CSycles.shader_new_graph(worldShader);
        CSycles.scene_set_background_shader(session, worldShader);
        CSycles.shader_set_name(worldShader, "black_world");

        // Camera above the quad, looking down -Z.
        CSycles.camera_set_matrix(session, new Transform(
            1f, 0f, 0f, 0f,
            0f, 1f, 0f, 0f,
            0f, 0f, 1f, float.Parse(Environment.GetEnvironmentVariable("SMOKE_CAMZ") ?? "-12",
                System.Globalization.CultureInfo.InvariantCulture)));
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
        cycles_debug_scene_stats(session);
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
            float mn = float.MaxValue, mx = float.MinValue;
            for (int i = 0; i < n; i++) { if (buf[i] < mn) mn = buf[i]; if (buf[i] > mx) mx = buf[i]; }
            Console.WriteLine("buffer rng : min=" + mn + " max=" + mx);
            int mid = (int)((H / 2) * W + W / 2) * comps;
            Console.WriteLine("centre px  : " + buf[mid] + " " + buf[mid+1] + " " + buf[mid+2] + " " + buf[mid+3]);
            Console.WriteLine("corner px  : " + buf[0] + " " + buf[1] + " " + buf[2] + " " + buf[3]);

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
