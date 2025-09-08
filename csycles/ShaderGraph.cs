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
    public class ShaderGraph : Node
    {
        public ShaderGraph() : this("a shadergraph node") { }

        public ShaderGraph(string name) :
            base(name)
        {
            FinalizeConstructor();
        }

        internal ShaderGraph(IntPtr intPtr) : base(intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
        }
        public void Relink(IntPtr from, IntPtr to) {
            CSycles.shadergraph_relink(Ptr, from, to);
        }

        public void Disconnect(IntPtr from) {
            CSycles.shadergraph_disconnect(Ptr, from);
        }

        public int GetNumClosures() {
            return CSycles.shadergraph_get_num_closures(Ptr);
        }

        public void Connect(IntPtr from, IntPtr to) {
            CSycles.shadergraph_connect(Ptr, from, to);
        }

        public long NumNodeIds {
            get { return CSycles.shadergraph_get_num_node_ids(Ptr); }
            set { CSycles.shadergraph_set_num_node_ids(Ptr, value); }
        }

        public void Finalize(IntPtr scene, bool do_bump, bool bump_in_object_space) {
            CSycles.shadergraph_finalize(Ptr, scene, do_bump, bump_in_object_space);
        }

        public void Relink1(IntPtr node, IntPtr from, IntPtr to) {
            CSycles.shadergraph_relink_1(Ptr, node, from, to);
        }

        public bool Finalized {
            get { return CSycles.shadergraph_get_finalized(Ptr); }
            set { CSycles.shadergraph_set_finalized(Ptr, value); }
        }

        public bool Simplified {
            get { return CSycles.shadergraph_get_simplified(Ptr); }
            set { CSycles.shadergraph_set_simplified(Ptr, value); }
        }

        public void Relink2(IntPtr from, IntPtr to) {
            CSycles.shadergraph_relink_2(Ptr, from, to);
        }

        public void Disconnect1(IntPtr to) {
            CSycles.shadergraph_disconnect_1(Ptr, to);
        }

        public void RemoveProxyNodes() {
            CSycles.shadergraph_remove_proxy_nodes(Ptr);
        }

        public void ComputeDisplacementHash() {
            CSycles.shadergraph_compute_displacement_hash(Ptr);
        }

        public void Simplify(IntPtr scene) {
            CSycles.shadergraph_simplify(Ptr, scene);
        }

        public void DumpGraph(IntPtr filename) {
            CSycles.shadergraph_dump_graph(Ptr, filename);
        }
    }

}