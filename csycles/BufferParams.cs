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

    public class BufferParamsNodeInputs : NodeInputs
    {
        public IntNodeSocket FullWidth { get; private set; }
        public IntNodeSocket FullHeight { get; private set; }
        public IntNodeSocket Width { get; private set; }
        public StringNodeSocket Layer { get; private set; }
        public IntNodeSocket Height { get; private set; }
        public StringNodeSocket View { get; private set; }
        public IntNodeSocket WindowX { get; private set; }
        public IntNodeSocket Samples { get; private set; }
        public IntNodeSocket WindowY { get; private set; }
        public FloatNodeSocket Exposure { get; private set; }
        public IntNodeSocket WindowWidth { get; private set; }
        public BoolNodeSocket UseApproximateShadowCatcher { get; private set; }
        public IntNodeSocket WindowHeight { get; private set; }
        public BoolNodeSocket TransparentBackground { get; private set; }
        public IntNodeSocket FullX { get; private set; }
        public IntNodeSocket FullY { get; private set; }

        public BufferParamsNodeInputs(Node parentNode)
        {
            FullWidth = new IntNodeSocket(parentNode, "Full Width", "full_width", true);
            AddSocket(FullWidth);
            FullHeight = new IntNodeSocket(parentNode, "Full Height", "full_height", true);
            AddSocket(FullHeight);
            Width = new IntNodeSocket(parentNode, "Width", "width", true);
            AddSocket(Width);
            Layer = new StringNodeSocket(parentNode, "Layer", "layer", true);
            AddSocket(Layer);
            Height = new IntNodeSocket(parentNode, "Height", "height", true);
            AddSocket(Height);
            View = new StringNodeSocket(parentNode, "View", "view", true);
            AddSocket(View);
            WindowX = new IntNodeSocket(parentNode, "Window X", "window_x", true);
            AddSocket(WindowX);
            Samples = new IntNodeSocket(parentNode, "Samples", "samples", true);
            AddSocket(Samples);
            WindowY = new IntNodeSocket(parentNode, "Window Y", "window_y", true);
            AddSocket(WindowY);
            Exposure = new FloatNodeSocket(parentNode, "Exposure", "exposure", true);
            AddSocket(Exposure);
            WindowWidth = new IntNodeSocket(parentNode, "Window Width", "window_width", true);
            AddSocket(WindowWidth);
            UseApproximateShadowCatcher = new BoolNodeSocket(parentNode, "Use Approximate Shadow Catcher", "use_approximate_shadow_catcher", true);
            AddSocket(UseApproximateShadowCatcher);
            WindowHeight = new IntNodeSocket(parentNode, "Window Height", "window_height", true);
            AddSocket(WindowHeight);
            TransparentBackground = new BoolNodeSocket(parentNode, "Transparent Background", "use_transparent_background", true);
            AddSocket(TransparentBackground);
            FullX = new IntNodeSocket(parentNode, "Full X", "full_x", true);
            AddSocket(FullX);
            FullY = new IntNodeSocket(parentNode, "Full Y", "full_y", true);
            AddSocket(FullY);
        }
    }
    [Node("buffer_params")]
    public class BufferParams : Node
    {
        public BufferParamsNodeInputs BufferParamsNodeInputs { get; set; }
        public BufferParamsNodeInputs ins => BufferParamsNodeInputs;

        public BufferParams() : this("a buffer_params node") { }

        public BufferParams(string name) :
            base(name)
        {
            FinalizeConstructor();
        }

        internal BufferParams(IntPtr intPtr) : base(intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            BufferParamsNodeInputs = new BufferParamsNodeInputs(this);

        }
        public float Exposure {
            get { return CSycles.bufferparams_get_exposure(Ptr); }
            set { CSycles.bufferparams_set_exposure(Ptr, value); }
        }

        public IntPtr FindPass(string name) {
            return CSycles.bufferparams_find_pass(Ptr, name);
        }

        public int GetPassOffset(PassType type, PassMode mode) {
            return CSycles.bufferparams_get_pass_offset(Ptr, type, mode);
        }

        public int Height {
            get { return CSycles.bufferparams_get_height(Ptr); }
            set { CSycles.bufferparams_set_height(Ptr, value); }
        }

        public int FullY {
            get { return CSycles.bufferparams_get_full_y(Ptr); }
            set { CSycles.bufferparams_set_full_y(Ptr, value); }
        }

        public int Samples {
            get { return CSycles.bufferparams_get_samples(Ptr); }
            set { CSycles.bufferparams_set_samples(Ptr, value); }
        }

        public int WindowX {
            get { return CSycles.bufferparams_get_window_x(Ptr); }
            set { CSycles.bufferparams_set_window_x(Ptr, value); }
        }

        public int PassStride {
            get { return CSycles.bufferparams_get_pass_stride(Ptr); }
            set { CSycles.bufferparams_set_pass_stride(Ptr, value); }
        }

        public int WindowY {
            get { return CSycles.bufferparams_get_window_y(Ptr); }
            set { CSycles.bufferparams_set_window_y(Ptr, value); }
        }

        public int WindowHeight {
            get { return CSycles.bufferparams_get_window_height(Ptr); }
            set { CSycles.bufferparams_set_window_height(Ptr, value); }
        }

        public int FullWidth {
            get { return CSycles.bufferparams_get_full_width(Ptr); }
            set { CSycles.bufferparams_set_full_width(Ptr, value); }
        }

        public int Offset {
            get { return CSycles.bufferparams_get_offset(Ptr); }
            set { CSycles.bufferparams_set_offset(Ptr, value); }
        }

        public IntPtr GetActualDisplayPass(IntPtr pass) {
            return CSycles.bufferparams_get_actual_display_pass(Ptr, pass);
        }

        public string View {
            get { return CSycles.bufferparams_get_view(Ptr); }
            set { CSycles.bufferparams_set_view(Ptr, value); }
        }

        public string Layer {
            get { return CSycles.bufferparams_get_layer(Ptr); }
            set { CSycles.bufferparams_set_layer(Ptr, value); }
        }

        public bool UseApproximateShadowCatcher {
            get { return CSycles.bufferparams_get_use_approximate_shadow_catcher(Ptr); }
            set { CSycles.bufferparams_set_use_approximate_shadow_catcher(Ptr, value); }
        }

        public IntPtr FindPass1(PassType type, PassMode mode) {
            return CSycles.bufferparams_find_pass_1(Ptr, type, mode);
        }

        public int Stride {
            get { return CSycles.bufferparams_get_stride(Ptr); }
            set { CSycles.bufferparams_set_stride(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.bufferparams_get_node_type();
        }

        public bool UseTransparentBackground {
            get { return CSycles.bufferparams_get_use_transparent_background(Ptr); }
            set { CSycles.bufferparams_set_use_transparent_background(Ptr, value); }
        }

        public int WindowWidth {
            get { return CSycles.bufferparams_get_window_width(Ptr); }
            set { CSycles.bufferparams_set_window_width(Ptr, value); }
        }

        public int FullHeight {
            get { return CSycles.bufferparams_get_full_height(Ptr); }
            set { CSycles.bufferparams_set_full_height(Ptr, value); }
        }

        public int FullX {
            get { return CSycles.bufferparams_get_full_x(Ptr); }
            set { CSycles.bufferparams_set_full_x(Ptr, value); }
        }

        public int Width {
            get { return CSycles.bufferparams_get_width(Ptr); }
            set { CSycles.bufferparams_set_width(Ptr, value); }
        }

        public IntPtr GetActualDisplayPass1(PassType type, PassMode mode) {
            return CSycles.bufferparams_get_actual_display_pass_1(Ptr, type, mode);
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "exposure":
                    /* bufferparams . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'exposure', 'ui_name': 'Exposure'} */
                    {
                    CSycles.bufferparams_set_exposure(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BufferParams (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "use_approximate_shadow_catcher":
                    /* bufferparams . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_approximate_shadow_catcher', 'ui_name': 'Use Approximate Shadow Catcher'} */
                    {
                    CSycles.bufferparams_set_use_approximate_shadow_catcher(this.Ptr, data);
                    }
                    break;
            case "use_transparent_background":
                    /* bufferparams . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_transparent_background', 'ui_name': 'Transparent Background'} */
                    {
                    CSycles.bufferparams_set_use_transparent_background(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BufferParams (setter)");
            }
        }

        internal override void SetInt(string name, int data)
        {
            switch(name) {
            case "full_width":
                    /* bufferparams . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'full_width', 'ui_name': 'Full Width'} */
                    {
                    CSycles.bufferparams_set_full_width(this.Ptr, data);
                    }
                    break;
            case "full_height":
                    /* bufferparams . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'full_height', 'ui_name': 'Full Height'} */
                    {
                    CSycles.bufferparams_set_full_height(this.Ptr, data);
                    }
                    break;
            case "width":
                    /* bufferparams . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'width', 'ui_name': 'Width'} */
                    {
                    CSycles.bufferparams_set_width(this.Ptr, data);
                    }
                    break;
            case "height":
                    /* bufferparams . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'height', 'ui_name': 'Height'} */
                    {
                    CSycles.bufferparams_set_height(this.Ptr, data);
                    }
                    break;
            case "window_x":
                    /* bufferparams . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'window_x', 'ui_name': 'Window X'} */
                    {
                    CSycles.bufferparams_set_window_x(this.Ptr, data);
                    }
                    break;
            case "samples":
                    /* bufferparams . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'samples', 'ui_name': 'Samples'} */
                    {
                    CSycles.bufferparams_set_samples(this.Ptr, data);
                    }
                    break;
            case "window_y":
                    /* bufferparams . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'window_y', 'ui_name': 'Window Y'} */
                    {
                    CSycles.bufferparams_set_window_y(this.Ptr, data);
                    }
                    break;
            case "window_width":
                    /* bufferparams . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'window_width', 'ui_name': 'Window Width'} */
                    {
                    CSycles.bufferparams_set_window_width(this.Ptr, data);
                    }
                    break;
            case "window_height":
                    /* bufferparams . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'window_height', 'ui_name': 'Window Height'} */
                    {
                    CSycles.bufferparams_set_window_height(this.Ptr, data);
                    }
                    break;
            case "full_x":
                    /* bufferparams . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'full_x', 'ui_name': 'Full X'} */
                    {
                    CSycles.bufferparams_set_full_x(this.Ptr, data);
                    }
                    break;
            case "full_y":
                    /* bufferparams . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'full_y', 'ui_name': 'Full Y'} */
                    {
                    CSycles.bufferparams_set_full_y(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BufferParams (setter)");
            }
        }

        internal override void SetString(string name, string data)
        {
            switch(name) {
            case "layer":
                    /* bufferparams . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'layer', 'ui_name': 'Layer'} */
                    {
                    CSycles.bufferparams_set_layer(this.Ptr, data);
                    }
                    break;
            case "view":
                    /* bufferparams . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'view', 'ui_name': 'View'} */
                    {
                    CSycles.bufferparams_set_view(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BufferParams (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "exposure":
                /* bufferparams . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'exposure', 'ui_name': 'Exposure'} */
                {
                    return CSycles.bufferparams_get_exposure(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BufferParams (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "use_approximate_shadow_catcher":
                /* bufferparams . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_approximate_shadow_catcher', 'ui_name': 'Use Approximate Shadow Catcher'} */
                {
                    return CSycles.bufferparams_get_use_approximate_shadow_catcher(this.Ptr);
                }
            case "use_transparent_background":
                /* bufferparams . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_transparent_background', 'ui_name': 'Transparent Background'} */
                {
                    return CSycles.bufferparams_get_use_transparent_background(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BufferParams (getter)");
            }
        }

        internal override int GetInt(string name)
        {
            switch(name) {
            case "full_width":
                /* bufferparams . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'full_width', 'ui_name': 'Full Width'} */
                {
                    return CSycles.bufferparams_get_full_width(this.Ptr);
                }
            case "full_height":
                /* bufferparams . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'full_height', 'ui_name': 'Full Height'} */
                {
                    return CSycles.bufferparams_get_full_height(this.Ptr);
                }
            case "width":
                /* bufferparams . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'width', 'ui_name': 'Width'} */
                {
                    return CSycles.bufferparams_get_width(this.Ptr);
                }
            case "height":
                /* bufferparams . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'height', 'ui_name': 'Height'} */
                {
                    return CSycles.bufferparams_get_height(this.Ptr);
                }
            case "window_x":
                /* bufferparams . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'window_x', 'ui_name': 'Window X'} */
                {
                    return CSycles.bufferparams_get_window_x(this.Ptr);
                }
            case "samples":
                /* bufferparams . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'samples', 'ui_name': 'Samples'} */
                {
                    return CSycles.bufferparams_get_samples(this.Ptr);
                }
            case "window_y":
                /* bufferparams . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'window_y', 'ui_name': 'Window Y'} */
                {
                    return CSycles.bufferparams_get_window_y(this.Ptr);
                }
            case "window_width":
                /* bufferparams . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'window_width', 'ui_name': 'Window Width'} */
                {
                    return CSycles.bufferparams_get_window_width(this.Ptr);
                }
            case "window_height":
                /* bufferparams . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'window_height', 'ui_name': 'Window Height'} */
                {
                    return CSycles.bufferparams_get_window_height(this.Ptr);
                }
            case "full_x":
                /* bufferparams . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'full_x', 'ui_name': 'Full X'} */
                {
                    return CSycles.bufferparams_get_full_x(this.Ptr);
                }
            case "full_y":
                /* bufferparams . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'full_y', 'ui_name': 'Full Y'} */
                {
                    return CSycles.bufferparams_get_full_y(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BufferParams (getter)");
            }
        }

        internal override string GetString(string name)
        {
            switch(name) {
            case "layer":
                /* bufferparams . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'layer', 'ui_name': 'Layer'} */
                {
                    return CSycles.bufferparams_get_layer(this.Ptr);
                }
            case "view":
                /* bufferparams . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'view', 'ui_name': 'View'} */
                {
                    return CSycles.bufferparams_get_view(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BufferParams (getter)");
            }
        }

#endregion
    }

}