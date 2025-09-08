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
namespace ccl
{
    using cclext;
    public class Session
    {
        public IntPtr Ptr { get; private set; } = IntPtr.Zero;

        public Session() {}

        public Session(IntPtr intPtr) { Ptr = intPtr; }
        public Scene Scene { get; private set; } = null;

        internal IntPtr _ccsession = IntPtr.Zero;

        static public (IntPtr idPtr, SessionParams sessionParams, SceneParams sceneParams, BufferParams bufferParams) PrepareForSession()
        {
            (IntPtr _idPtr, SessionParams sessParams, SceneParams sceParams, BufferParams bufParams) = CSycles.prepare_ccsession();

            return (_idPtr, sessParams, sceParams, bufParams);
        }

        static public Session CreateSession(IntPtr idPtr, SessionParams sessionParams, SceneParams sceneParams)
        {
            Session sess = CSycles.create_session(idPtr, sessionParams, sceneParams);
            sess.Scene = CSycles.session_get_scene(sess);
            sess.Scene.Background.ins.Shader.Value = sess.Scene.DefaultBackground.Ptr;
            return sess;
        }

        readonly List<PassType> passes = [];
        public List<PassType> Passes => passes;

        public void AddPass(Pass pass)
        {
            CSycles.session_add_pass(_ccsession, pass);
            passes.Add((PassType)pass.ins.Type.Value);
        }

        public void Reset(SessionParams session_params, BufferParams buffer_params)
        {
            CSycles.session_reset(this, session_params, buffer_params);
        }

        public Progress Progress {
            get {
                return CSycles.get_progress(this);
            }
        }

        public SessionParams SessionParams {
            get {
                return CSycles.session_get_session_params(_ccsession);
            }
        }

        public SceneParams SceneParams {
            get {
                return CSycles.session_get_scene_params(_ccsession);
            }
        }

        public BufferParams BufferParams {
            get {
                return CSycles.session_get_buffer_params(_ccsession);
            }
        }

        public void RetainPixelBuffer(PassType pt, int width, int height, ref IntPtr pixel_buffer, ref int pixel_size_from_cycles)
        {
            if (_destroyed)
            {
                pixel_buffer = IntPtr.Zero;
                pixel_size_from_cycles = 0;
            }
            else
            {
                CSycles.session_retain_float_buffer(_ccsession, pt, width, height, ref pixel_buffer, ref pixel_size_from_cycles);
            }
        }

        public void ReleasePixelBuffer(PassType pt)
        {
            if (!_destroyed)
            {
                CSycles.session_release_float_buffer(_ccsession, pt);
            }
        }

        private bool _destroyed = false;
        ~Session()
        {
            Dispose();
        }

        public void Dispose()
        {
            if(!_destroyed) {
                Cancel(quick: true);
                CSycles.session_destroy(_ccsession);
                _destroyed = true;
            }
            GC.SuppressFinalize(this);
        }

        public void Start() {
            CSycles.session_start(Ptr);
        }

        public void CollectStatistics(IntPtr stats) {
            CSycles.session_collect_statistics(Ptr, stats);
        }

        public void SetTimeLimit(double time_limit) {
            CSycles.session_set_time_limit(Ptr, time_limit);
        }

        public void Draw() {
            CSycles.session_draw(Ptr);
        }

        public void Cancel(bool quick) {
            if (!_destroyed)
            {
                CSycles.session_cancel(Ptr, quick);
            }
        }

        public bool ReadyToReset() {
            return CSycles.session_ready_to_reset(Ptr);
        }

        public void SetSamples(int samples) {
            CSycles.session_set_samples(Ptr, samples);
        }

        public void Wait() {
            CSycles.session_wait(Ptr);
        }

        public double GetEstimatedRemainingTime() {
            return CSycles.session_get_estimated_remaining_time(Ptr);
        }

        public void SetPause(bool pause) {
            CSycles.session_set_pause(Ptr, pause);
        }
    }

}