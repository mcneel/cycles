# Batch render driver for the Beta-vs-dev Cycles comparison.
#
# Runs identically in both builds (the Debug build reaches RunPythonScript via a
# command file), so the two sides differ only in the renderer code under test.
#
# Everything that feeds Cycles' integrator is pinned explicitly on the document,
# because the two builds do NOT share defaults: the dev branch carries commits
# that made adaptive sampling per-document and added render presets, so relying
# on DefaultEngineSettings would compare two different configurations. Adaptive
# sampling is switched off so every pixel gets exactly the requested sample
# count on both sides, isolating shading differences from sampling-termination
# differences.
#
# Paths and values arrive through the environment so nothing has to survive the
# command line.
import os
import traceback

import System
import Rhino
import rhinoscriptsyntax as rs

OUT = os.environ["RHDIFF_OUT"]          # output path stem, no extension
MODEL = os.environ["RHDIFF_MODEL"]
LOG = os.environ["RHDIFF_LOG"]
SAMPLES = int(os.environ.get("RHDIFF_SAMPLES", "250"))
WIDTH = int(os.environ.get("RHDIFF_WIDTH", "1200"))
HEIGHT = int(os.environ.get("RHDIFF_HEIGHT", "700"))
# RHDIFF_PIN=0 pins only resolution and sample count, leaving every other
# integrator value at each build's own default. Used to check whether a
# difference is caused by the pinning itself.
PIN = os.environ.get("RHDIFF_PIN", "1") != "0"
# RHDIFF_UNTOUCHED=1 changes nothing at all: open the document, render at whatever
# the file itself specifies, save. Used to show a repro needs no settings help.
UNTOUCHED = os.environ.get("RHDIFF_UNTOUCHED", "0") == "1"
# RHDIFF_PINKEYS="A,B,C" pins only those keys, for bisecting which pinned
# setting changes a render.
PINKEYS = [k.strip() for k in os.environ.get("RHDIFF_PINKEYS", "").split(",") if k.strip()]
# Log every material with its child texture slots.
MATDUMP = os.environ.get("RHDIFF_MATDUMP", "0") == "1"
# Enumerating doc.RenderMaterials / doc.RenderEnvironments instantiates RDK
# content and starts the material preview system. The production render then
# begins while those previews are still going and two engines run at once:
# Rhino Logo_texture_mapping_types_saved_from_v8 crashed exactly that way - 36
# CreatePreview ops over 5 preview engines, preview traffic still arriving 71 s
# after the modal engine started - while the same document rendered by hand,
# with no preview activity, was fine. Off by default so a regression run never
# touches content; set RHDIFF_CONTENT=1 for A/B diagnosis.
CONTENT_DUMP = os.environ.get("RHDIFF_CONTENT", "0") == "1"
# Keep each document's own render size instead of forcing WIDTH x HEIGHT. A golden
# image test only needs the size to be stable between runs, and a document with
# UseViewportSize=False already guarantees that. Forcing one fixed size re-frames
# any scene authored at another aspect ratio: Test_backgroundimage is 800x600 with
# a WallpaperImage background, and at 1920x1080 the wallpaper is cropped to two
# slivers in the corners. Five of the eight models in the RH-81636 set are 4:3.
# The A/B harness must NOT set this - there both builds have to render the same
# pixel grid or the comparison is meaningless.
KEEPSIZE = os.environ.get("RHDIFF_KEEPSIZE", "0") == "1"
# Replace the material with this name with an untextured Physically Based one.
REPLACEMAT = os.environ.get("RHDIFF_REPLACEMAT", "").strip()
# Repoint textures whose stored absolute path does not exist here at the local copy.
FIXTEX = os.environ.get("RHDIFF_FIXTEX", "") not in ("", "0")
# RHDIFF_SET="Key=Value,Key=Value" writes these on top of whatever pinning did, so a
# single field can be swept to a value the INTEGRATOR table does not carry.
SETS = [kv.split("=", 1) for kv in os.environ.get("RHDIFF_SET", "").split(",") if "=" in kv]
# RHDIFF_LIGHTS=radius:<f>,shadow:<f> forces every document light to one emitter size.
# dev derives emitter size from Light.Radius (RH-96957/RH-96839) where shipping still
# uses the shadow-intensity term, so the two disagree on soft shadows and near-field
# falloff unless both are pinned.
LIGHTS = dict(kv.split(":", 1) for kv in os.environ.get("RHDIFF_LIGHTS", "").split(",")
              if ":" in kv)
# RHDIFF_GP=off disables the ground plane, =opaque keeps it but clears shadow-only.
# SimpleVaseTest's shadow catcher is the ground plane, and neither build can be
# instrumented on the shipping side, so the only way to attribute the 2.5x to the
# shadow-catcher composite is to take the catcher out of the document for both.
GP = os.environ.get("RHDIFF_GP", "").strip().lower()
# Save the (possibly edited) document here and skip rendering entirely.
SAVEAS = os.environ.get("RHDIFF_SAVEAS", "").strip()

# Every field EngineDocumentSettings.IntegratorHash CRCs, plus the neighbouring
# light/caustic switches, pinned to one known set of values.
INTEGRATOR = {
    "Seed": 128,
    "Samples": SAMPLES,
    "UseDocumentSamples": True,
    "AaSamples": 32,
    "DiffuseSamples": 32,
    "GlossySamples": 32,
    "TransmissionSamples": 32,
    "MaxBounce": 32,
    "MaxDiffuseBounce": 4,
    "MaxGlossyBounce": 16,
    "MaxVolumeBounce": 32,
    "MaxTransmissionBounce": 32,
    "TransparentMaxBounce": 32,
    "UseAdaptiveSampling": False,
    "AdaptiveMinSamples": 16,
    "AdaptiveThreshold": 0.01,
    "FilterGlossy": 0.5,
    "SampleClampDirect": 3.0,
    "SampleClampIndirect": 3.0,
    "LightSamplingThreshold": 0.05,
    "NoCaustics": False,
    "UseDirectLight": True,
    "UseIndirectLight": True,
}


def log(msg):
    f = open(LOG, "a")
    try:
        f.write("{0}\n".format(msg))
        f.flush()
    finally:
        f.close()


def dget(d, key):
    try:
        if d.ContainsKey(key):
            return d[key]
    except Exception:
        pass
    return None


try:
    log("OPENING {0}".format(MODEL))
    ok = rs.Command('_-Open "{0}"'.format(MODEL), False)
    # On Mac the open has not finished when the command returns - ActiveDoc is still
    # the previous (empty) document, and rendering it produces a blank image at the
    # wrong size while every step still reports success. Wait for the document to
    # actually swap in. On Windows the open is already done and this exits on the
    # first pass.
    want = os.path.basename(MODEL).lower()
    waited = 0.0
    for _ in range(1200):
        doc = Rhino.RhinoDoc.ActiveDoc
        if doc is not None and doc.Path and os.path.basename(doc.Path).lower() == want:
            break
        Rhino.RhinoApp.Wait()
        System.Threading.Thread.Sleep(500)
        waited += 0.5
    doc = Rhino.RhinoDoc.ActiveDoc
    log("OPENED rc={0} path={1} waited={2}s".format(ok, doc.Path, waited))
    if not doc.Path or os.path.basename(doc.Path).lower() != want:
        raise RuntimeError("document did not open: " + MODEL)
    log("RHINO {0}".format(Rhino.RhinoApp.Version))

    if LIGHTS:
        n = 0
        for i in range(doc.Lights.Count):
            lo = doc.Lights[i]
            lg = lo.LightGeometry
            if "radius" in LIGHTS:
                try:
                    lg.Radius = float(LIGHTS["radius"])
                except Exception as exc:
                    log("LIGHTS radius failed: {0}".format(exc))
            if "shadow" in LIGHTS:
                try:
                    lg.ShadowIntensity = float(LIGHTS["shadow"])
                except Exception as exc:
                    log("LIGHTS shadow failed: {0}".format(exc))
            doc.Lights.Modify(i, lg)
            n += 1
        log("LIGHTS pinned {0} light(s) to {1}".format(n, LIGHTS))

    if GP:
        try:
            gp = doc.GroundPlane
            log("GP before: enabled={0} shadow_only={1}".format(
                gp.Enabled, getattr(gp, "ShadowOnly", "n/a")))
            if GP == "off":
                gp.Enabled = False
            elif GP == "opaque":
                gp.ShadowOnly = False
            log("GP after:  enabled={0} shadow_only={1}".format(
                doc.GroundPlane.Enabled,
                getattr(doc.GroundPlane, "ShadowOnly", "n/a")))
        except Exception as exc:
            log("GP change failed: {0}".format(exc))

    st = doc.RenderSettings.Duplicate()
    d = st.UserDictionary
    log("BEFORE Samples={0} UseDocumentSamples={1} UseAdaptiveSampling={2} Seed={3}".format(
        dget(d, "Samples"), dget(d, "UseDocumentSamples"),
        dget(d, "UseAdaptiveSampling"), dget(d, "Seed")))
    # SimpleVaseTest composites a shadow catcher, and dev renders alpha uniformly 1.0.
    # Whether the document even asks for a transparent background decides whether that
    # is a dev bug or the correct result.
    try:
        log("BEFORE TransparentBackground={0} shadow_catcher_objects=?".format(
            st.TransparentBackground))
    except Exception as exc:
        log("BEFORE TransparentBackground unreadable: {0}".format(exc))
    log("BEFORE UseViewportSize={0} ImageSize={1} AntialiasLevel={2}".format(
        st.UseViewportSize, st.ImageSize, st.AntialiasLevel))

    if UNTOUCHED:
        log("UNTOUCHED -- rendering the file exactly as it is, nothing set")
    elif KEEPSIZE and not st.UseViewportSize:
        # The document has its own size and it is stable across runs, so use it.
        log("KEEPSIZE -- keeping the document's own ImageSize {0}".format(st.ImageSize))
    else:
        # Pin the resolution. Left alone, UseViewportSize makes the render follow
        # each Rhino window's viewport, and the two windows are not the same size --
        # the first attempt produced 1458x835 against 1462x791, undiffable. This is
        # also the path a KEEPSIZE run takes when the document has UseViewportSize
        # set, since then it has no size of its own to keep.
        if KEEPSIZE:
            log("KEEPSIZE -- document has UseViewportSize set, pinning instead")
        st.UseViewportSize = False
        st.ImageSize = System.Drawing.Size(WIDTH, HEIGHT)

    if UNTOUCHED:
        pass
    elif PINKEYS:
        unknown = [k for k in PINKEYS if k not in INTEGRATOR]
        if unknown:
            raise Exception("unknown pin keys: {0}".format(unknown))
        d["Samples"] = SAMPLES
        d["UseDocumentSamples"] = True
        for k in PINKEYS:
            d[k] = INTEGRATOR[k]
        log("PINKEYS {0}".format(",".join(PINKEYS)))
    elif PIN:
        for k in sorted(INTEGRATOR):
            d[k] = INTEGRATOR[k]
    else:
        d["Samples"] = SAMPLES
        d["UseDocumentSamples"] = True
    for key, raw in SETS:
        key = key.strip()
        raw = raw.strip()
        if raw.lower() in ("true", "false"):
            val = raw.lower() == "true"
        elif "." in raw:
            val = float(raw)
        else:
            val = int(raw)
        d[key] = val
        log("SET {0}={1!r}".format(key, val))

    if not UNTOUCHED:
        doc.RenderSettings = st

    st2 = doc.RenderSettings
    d2 = st2.UserDictionary
    log("AFTER UseViewportSize={0} ImageSize={1}".format(st2.UseViewportSize, st2.ImageSize))
    log("AFTER Samples={0} UseDocumentSamples={1} UseAdaptiveSampling={2} Seed={3}".format(
        dget(d2, "Samples"), dget(d2, "UseDocumentSamples"),
        dget(d2, "UseAdaptiveSampling"), dget(d2, "Seed")))
    for k in sorted(INTEGRATOR):
        log("INTVAL {0}={1}".format(k, dget(d2, k)))

    if PIN and not PINKEYS:
        mismatch = [k for k in sorted(INTEGRATOR) if dget(d2, k) != INTEGRATOR[k]]
        log("PINNED {0}/{1} settings, mismatched={2}".format(
            len(INTEGRATOR) - len(mismatch), len(INTEGRATOR), mismatch))
    else:
        log("PINNED off -- only resolution and Samples set")
    log("RENDERER {0}".format(Rhino.Render.Utilities.DefaultRenderPlugInId))

    # RhinoCycles reads BackgroundStyle straight from here and its background code is
    # identical to shipping, so if dev paints a white background where shipping paints
    # the environment, the difference has to be visible in what Rhino itself reports.
    try:
        # Not named rs: that is rhinoscriptsyntax at module scope, and shadowing it
        # here made the render call below fail with AttributeError.
        rsettings = doc.RenderSettings
        bgenv = None
        try:
            bgenv = doc.CurrentEnvironment.ForBackground
        except Exception:
            bgenv = None
        log("BGSTYLE style={0} top={1} bottom={2} forBackground='{3}'".format(
            rsettings.BackgroundStyle, rsettings.BackgroundColorTop,
            rsettings.BackgroundColorBottom,
            bgenv.Name if bgenv is not None else None))
    except Exception:
        log("BGSTYLE_FAILED {0}".format(traceback.format_exc().splitlines()[-1]))

    # Inventory of render content, to correlate any image difference with the
    # node types known to have drifted in 5.2. Isolated so it can never stop the
    # render.
    try:
        kinds = {}

        def walk(node, prefix):
            # NativeRenderMaterial has no .Children; RenderContent exposes the
            # tree as FirstChild / NextSibling, which works for both.
            stack = [node]
            while stack:
                n = stack.pop()
                key = prefix + n.TypeName
                kinds[key] = kinds.get(key, 0) + 1
                child = n.FirstChild
                while child is not None:
                    stack.append(child)
                    child = child.NextSibling

        if CONTENT_DUMP:
            for c in doc.RenderMaterials:
                walk(c, "")
            for c in doc.RenderEnvironments:
                walk(c, "env:")
        for k in sorted(kinds):
            log("CONTENT {0} x{1}".format(k, kinds[k]))
        log("CONTENT_TOTAL {0} distinct".format(len(kinds)))
    except Exception:
        log("CONTENT_FAILED {0}".format(traceback.format_exc().splitlines()[-1]))

    # Per material detail, and an optional texture strip. The tabletop in
    # Brian25YearRhinoGlas renders black while the glass and lamp shade in the same
    # document convert fine, so the question is whether that material's own graph or
    # its bitmap texture is at fault. Stripping the texture children answers it:
    # if the surface comes back as a flat colour the texture wiring is to blame, if
    # it stays black the material is. Isolated so it can never stop the render.
    try:
        if MATDUMP or REPLACEMAT:
            # Environments too. SimpleVaseTest's background is an HDR texture that never
            # reaches Cycles - no "Updating Images" line at all - and whether it has a
            # file on disk or supplies pixels in memory decides which path is at fault.
            for e in doc.RenderEnvironments:
                log("ENV '{0}' type='{1}'".format(e.Name, e.TypeName))
                child = e.FirstChild
                while child is not None:
                    fn = getattr(child, "Filename", None)
                    log("  ENVCHILD slot='{0}' type='{1}' file='{2}' exists={3}".format(
                        child.ChildSlotName, child.TypeName, fn,
                        os.path.exists(fn) if fn else "n/a"))
                    child = child.NextSibling

            for c in doc.RenderMaterials:
                log("MAT '{0}' type='{1}' internal='{2}'".format(
                    c.Name, c.TypeName, getattr(c, "TypeInternalName", "?")))
                child = c.FirstChild
                while child is not None:
                    fn = getattr(child, "Filename", None)
                    log("  CHILD slot='{0}' type='{1}' file='{2}' exists={3}".format(
                        child.ChildSlotName, child.TypeName, fn,
                        os.path.exists(fn) if fn else "n/a"))
                    child = child.NextSibling

        # Swap a fresh untextured Physically Based material onto whatever objects use
        # the named one. Deleting child content in place stalled the render pipeline,
        # so build a new material instead and leave the document's content tree alone.
        # Table renders -> the bitmap texture node is forcing black.
        # Table stays black -> the PBR conversion itself is at fault.
        if FIXTEX:
            # Every one of the four parity models stores its textures as absolute paths
            # from the machine that made it, under another users profile, so the studio
            # environment is missing here. The two builds then fall back differently: dev
            # resolves StudioC.hdr and shipping loads no image at all, which shows up as a
            # uniform background difference and gets mistaken for a shading bug. Repoint
            # anything missing at the local copy of the same file so both builds render
            # the same scene.
            local = os.path.join(
                os.environ.get("APPDATA", ""), "McNeel", "Rhinoceros", "9.0",
                "Localization", "en-US", "Render Content", "Textures")
            counts = {"fixed": 0, "missing": 0}

            def fix_child(child):
                fn = getattr(child, "Filename", None)
                if not fn or os.path.exists(fn):
                    return
                cand = os.path.join(local, os.path.basename(fn))
                if os.path.exists(cand):
                    child.BeginChange(Rhino.Render.RenderContent.ChangeContexts.Program)
                    child.Filename = cand
                    child.EndChange()
                    log("FIXTEX {0} -> {1}".format(os.path.basename(fn), cand))
                    counts["fixed"] += 1
                else:
                    log("FIXTEX no local copy of {0}".format(os.path.basename(fn)))
                    counts["missing"] += 1

            def walk(content):
                child = content.FirstChild
                while child is not None:
                    fix_child(child)
                    walk(child)
                    child = child.NextSibling

            for e in doc.RenderEnvironments:
                fix_child(e)
                walk(e)
            for m in doc.RenderMaterials:
                fix_child(m)
                walk(m)
            log("FIXTEX repointed {0}, still missing {1}".format(
                counts["fixed"], counts["missing"]))

        if REPLACEMAT:
            m = Rhino.DocObjects.Material()
            m.ToPhysicallyBased()
            m.PhysicallyBased.BaseColor = Rhino.Display.Color4f(0.7, 0.7, 0.7, 1.0)
            rm = Rhino.Render.RenderMaterial.CreateBasicMaterial(m, doc)
            rm.Name = "probe_pbr_notex"
            try:
                doc.RenderMaterials.Add(rm)
            except Exception:
                pass
            n = 0
            for obj in doc.Objects:
                cur = None
                try:
                    cur = obj.RenderMaterial
                except Exception:
                    cur = None
                if cur is not None and cur.Name == REPLACEMAT:
                    obj.RenderMaterial = rm
                    obj.CommitChanges()
                    n += 1
            log("REPLACEMAT '{0}' -> untextured PBR on {1} objects".format(REPLACEMAT, n))

        # Changing render content and then rendering in the same session stalls the
        # dev build - the render starts and burns no CPU. So save the edited document
        # and render it in a separate, unmodified session instead.
        if SAVEAS:
            rs.Command('_-SaveAs "{0}"'.format(SAVEAS), False)
            log("SAVEAS {0} exists={1}".format(SAVEAS, os.path.exists(SAVEAS)))
            log("DONE")
            raise SystemExit(0)
    except Exception:
        log("MATDIAG_FAILED {0}".format(traceback.format_exc().splitlines()[-1]))

    log("RENDER_START")
    ok = rs.Command("_-Render", False)
    log("RENDER_END rc={0}".format(ok))

    # hdr/exr carry float values; the 8-bit formats clip, which made every
    # background tap read 255 and stalled the bisect.
    for ext in ("bmp", "png", "hdr", "exr"):
        path = "{0}.{1}".format(OUT, ext)
        if os.path.exists(path):
            os.remove(path)
        ok = rs.Command('_-SaveRenderWindowAs "{0}"'.format(path), False)
        exists = os.path.exists(path)
        size = os.path.getsize(path) if exists else -1
        log("SAVED {0} rc={1} exists={2} bytes={3}".format(path, ok, exists, size))

    log("DONE")
except Exception:
    log("ERROR\n{0}".format(traceback.format_exc()))
    log("DONE")
