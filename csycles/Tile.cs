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

Code generated at: 2025-11-21 07:20:37 UTC
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
    public class Tile
    {
        public IntPtr Ptr { get; private set; } = IntPtr.Zero;

        public Tile() {}

        public Tile(IntPtr intPtr) { Ptr = intPtr; }
        public int WindowX {
            get { return CSycles.tile_get_window_x(Ptr); }
            set { CSycles.tile_set_window_x(Ptr, value); }
        }

        public int X {
            get { return CSycles.tile_get_x(Ptr); }
            set { CSycles.tile_set_x(Ptr, value); }
        }

        public int WindowY {
            get { return CSycles.tile_get_window_y(Ptr); }
            set { CSycles.tile_set_window_y(Ptr, value); }
        }

        public int WindowHeight {
            get { return CSycles.tile_get_window_height(Ptr); }
            set { CSycles.tile_set_window_height(Ptr, value); }
        }

        public int Y {
            get { return CSycles.tile_get_y(Ptr); }
            set { CSycles.tile_set_y(Ptr, value); }
        }

        public int WindowWidth {
            get { return CSycles.tile_get_window_width(Ptr); }
            set { CSycles.tile_set_window_width(Ptr, value); }
        }

        public int Width {
            get { return CSycles.tile_get_width(Ptr); }
            set { CSycles.tile_set_width(Ptr, value); }
        }

        public int Height {
            get { return CSycles.tile_get_height(Ptr); }
            set { CSycles.tile_set_height(Ptr, value); }
        }
    }

}