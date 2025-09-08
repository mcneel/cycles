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

Code generated at: 2025-11-23 08:00:00 UTC
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
    public class Progress
    {
        public IntPtr Ptr { get; private set; } = IntPtr.Zero;

        public Progress() {}

        public Progress(IntPtr intPtr) { Ptr = intPtr; }
        public (string status, string substatus) GetStatus()
        {
            return CSycles.progress_get_status(this);
        }

        public void SetCancel(string cancel_message_) {
            CSycles.progress_set_cancel(Ptr, cancel_message_);
        }

        public void GetTime(out double total_time_, out double render_time_) {
            CSycles.progress_get_time(Ptr, out total_time_, out render_time_);
        }

        public bool GetCancel() {
            return CSycles.progress_get_cancel(Ptr);
        }

        public int GetDenoisedTiles() {
            return CSycles.progress_get_denoised_tiles(Ptr);
        }

        public void SetSyncSubstatus(string substatus_) {
            CSycles.progress_set_sync_substatus(Ptr, substatus_);
        }

        public void SetError(string error_message_) {
            CSycles.progress_set_error(Ptr, error_message_);
        }

        public void SetStartTime() {
            CSycles.progress_set_start_time(Ptr);
        }

        public void ResetSample() {
            CSycles.progress_reset_sample(Ptr);
        }

        public void SetSubstatus(string substatus_) {
            CSycles.progress_set_substatus(Ptr, substatus_);
        }

        public void SetRenderStartTime() {
            CSycles.progress_set_render_start_time(Ptr);
        }

        public int GetRenderedTiles() {
            return CSycles.progress_get_rendered_tiles(Ptr);
        }

        public int GetCurrentSample() {
            return CSycles.progress_get_current_sample(Ptr);
        }

        public void SetTimeLimit(double time_limit_) {
            CSycles.progress_set_time_limit(Ptr, time_limit_);
        }

        public void SetSyncStatus(string status_, string substatus_) {
            CSycles.progress_set_sync_status(Ptr, status_, substatus_);
        }

        public void AddFinishedTile(bool denoised) {
            CSycles.progress_add_finished_tile(Ptr, denoised);
        }

        public void SetTotalPixelSamples(ulong total_pixel_samples_) {
            CSycles.progress_set_total_pixel_samples(Ptr, total_pixel_samples_);
        }

        public string GetCancelMessage() {
            return CSycles.progress_get_cancel_message(Ptr);
        }

        public bool GetError() {
            return CSycles.progress_get_error(Ptr);
        }

        public void Reset() {
            CSycles.progress_reset(Ptr);
        }

        public void AddSamples(ulong pixel_samples_, int tile_sample) {
            CSycles.progress_add_samples(Ptr, pixel_samples_, tile_sample);
        }

        public string GetErrorMessage() {
            return CSycles.progress_get_error_message(Ptr);
        }
    }

}