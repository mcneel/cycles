/**
Copyright 2014-2025 Robert McNeel and Associates

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

----------------------------------------------------------------------
NOTE: Do NOT modify this file directly, it is automatically generated.

Code generated at: 2025-12-02 03:24:08 UTC
----------------------------------------------------------------------

**/

using ccl;
using ccl.Attributes;
using ccl.ShaderNodes;
using ccl.ShaderNodes.Sockets;
using ccl.NodeSockets;
using System;
using System.Collections.Generic;

using System.Threading;
namespace ccl
{
    using cclext;
    public class Scene : Node
    {
        public Scene() : this("a scene node") { }

        public Scene(string name) :
            base(name)
        {
            FinalizeConstructor();
        }

        internal Scene(IntPtr intPtr) : base(intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
        }
        public bool TryLock()
        {
            return CSycles.scene_try_lock(this);
        }

        public void Unlock()
        {
            CSycles.scene_unlock(this);
        }

        public void WaitUntilLocked()
        {
            while(!TryLock())
            {
                Thread.Sleep(10);
            }
        }

        private readonly object addMeshLock = new object();
        public Mesh AddMesh()
        {
            lock(addMeshLock)
            {
                return CSycles.scene_createmesh(this);
            }
        }

        private readonly object addObjectLock = new object();
        public ccl.Object AddObject()
        {
            lock(addObjectLock)
            {
                return CSycles.scene_createobject(this);
            }
        }

        private readonly object addLightLock = new object();
        public Light AddLight()
        {
            lock(addLightLock)
            {
                return CSycles.scene_createlight(this);
            }
        }

        private readonly object addShaderLock = new object();
        public Shader AddShader()
        {
            lock(addShaderLock)
            {
                Shader shader = CSycles.scene_createshader(this);
                OutputNode outnode = (OutputNode)CSycles.CreateShaderNode(shader, CSycles.shader_get_outputnode(shader), "output");

                return shader;
            }
        }

        public Pass AddPass()
        {
            return CSycles.scene_createpass(this);
        }

        public IntPtr DefaultVolume {
            get { return CSycles.scene_get_default_volume(Ptr); }
            set { CSycles.scene_set_default_volume(Ptr, value); }
        }

        public Integrator Integrator {
            get { return new Integrator(CSycles.scene_get_integrator(Ptr)); }
            set { CSycles.scene_set_integrator(Ptr, value.Ptr); }
        }

        public Camera Camera {
            get { return new Camera(CSycles.scene_get_camera(Ptr)); }
            set { CSycles.scene_set_camera(Ptr, value.Ptr); }
        }

        public bool NeedReset(bool check_camera) {
            return CSycles.scene_need_reset(Ptr, check_camera);
        }

        public Background Background {
            get { return new Background(CSycles.scene_get_background(Ptr)); }
            set { CSycles.scene_set_background(Ptr, value.Ptr); }
        }

        public Shader DefaultBackground {
            get { return new Shader(CSycles.scene_get_default_background(Ptr)); }
            set { CSycles.scene_set_default_background(Ptr, value.Ptr); }
        }

        public void CollectStatistics(IntPtr stats) {
            CSycles.scene_collect_statistics(Ptr, stats);
        }

        public Shader DefaultLight {
            get { return new Shader(CSycles.scene_get_default_light(Ptr)); }
            set { CSycles.scene_set_default_light(Ptr, value.Ptr); }
        }

        public Shader DefaultSurface {
            get { return new Shader(CSycles.scene_get_default_surface(Ptr)); }
            set { CSycles.scene_set_default_surface(Ptr, value.Ptr); }
        }

        public float MotionShutterTime() {
            return CSycles.scene_motion_shutter_time(Ptr);
        }

        public Film Film {
            get { return new Film(CSycles.scene_get_film(Ptr)); }
            set { CSycles.scene_set_film(Ptr, value.Ptr); }
        }

        public IntPtr DefaultEmpty {
            get { return CSycles.scene_get_default_empty(Ptr); }
            set { CSycles.scene_set_default_empty(Ptr, value); }
        }

        public IntPtr DicingCamera {
            get { return CSycles.scene_get_dicing_camera(Ptr); }
            set { CSycles.scene_set_dicing_camera(Ptr, value); }
        }

        public bool HasShadowCatcher() {
            return CSycles.scene_has_shadow_catcher(Ptr);
        }

        public void Reset() {
            CSycles.scene_reset(Ptr);
        }
    }

}