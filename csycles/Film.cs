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

    public class FilmNodeInputs : NodeInputs
    {
        public BoolNodeSocket ShowActivePixels { get; private set; }
        public FloatNodeSocket PassAlphaThreshold { get; private set; }
        public FloatNodeSocket Exposure { get; private set; }
        public FloatNodeSocket MistStart { get; private set; }
        public EnumNodeSocket CryptomattePasses { get; private set; }
        public FloatNodeSocket FilterWidth { get; private set; }
        public FloatNodeSocket MistDepth { get; private set; }
        public IntNodeSocket CryptomatteDepth { get; private set; }
        public EnumNodeSocket FilterType { get; private set; }
        public FloatNodeSocket MistFalloff { get; private set; }
        public BoolNodeSocket UseApproximateShadowCatcher { get; private set; }
        public EnumNodeSocket DisplayPass { get; private set; }

        public FilmNodeInputs(Node parentNode)
        {
            ShowActivePixels = new BoolNodeSocket(parentNode, "Show Active Pixels", "show_active_pixels", true);
            AddSocket(ShowActivePixels);
            PassAlphaThreshold = new FloatNodeSocket(parentNode, "Pass Alpha Threshold", "pass_alpha_threshold", true);
            AddSocket(PassAlphaThreshold);
            Exposure = new FloatNodeSocket(parentNode, "Exposure", "exposure", true);
            AddSocket(Exposure);
            MistStart = new FloatNodeSocket(parentNode, "Mist Start", "mist_start", true);
            AddSocket(MistStart);
            CryptomattePasses = new EnumNodeSocket(parentNode, "Cryptomatte Passes", "cryptomatte_passes", true);
            AddSocket(CryptomattePasses);
            FilterWidth = new FloatNodeSocket(parentNode, "Filter Width", "filter_width", true);
            AddSocket(FilterWidth);
            MistDepth = new FloatNodeSocket(parentNode, "Mist Depth", "mist_depth", true);
            AddSocket(MistDepth);
            CryptomatteDepth = new IntNodeSocket(parentNode, "Cryptomatte Depth", "cryptomatte_depth", true);
            AddSocket(CryptomatteDepth);
            FilterType = new EnumNodeSocket(parentNode, "Filter Type", "filter_type", true);
            AddSocket(FilterType);
            MistFalloff = new FloatNodeSocket(parentNode, "Mist Falloff", "mist_falloff", true);
            AddSocket(MistFalloff);
            UseApproximateShadowCatcher = new BoolNodeSocket(parentNode, "Use Approximate Shadow Catcher", "use_approximate_shadow_catcher", true);
            AddSocket(UseApproximateShadowCatcher);
            DisplayPass = new EnumNodeSocket(parentNode, "Display Pass", "display_pass", true);
            AddSocket(DisplayPass);
        }
    }
    [Node("film")]
    public class Film : Node
    {
        public enum FilmCryptomattePasses : uint {
            None = ccl.CryptomatteType.CRYPT_NONE,
            Object = ccl.CryptomatteType.CRYPT_OBJECT,
            Material = ccl.CryptomatteType.CRYPT_MATERIAL,
            Asset = ccl.CryptomatteType.CRYPT_ASSET,
            Accurate = ccl.CryptomatteType.CRYPT_ACCURATE,
        }
        public enum FilmFilter : uint {
            Box = ccl.FilterType.FILTER_BOX,
            Gaussian = ccl.FilterType.FILTER_GAUSSIAN,
            BlackmanHarris = ccl.FilterType.FILTER_BLACKMAN_HARRIS,
        }
        public FilmNodeInputs FilmNodeInputs { get; set; }
        public FilmNodeInputs ins => FilmNodeInputs;

        public Film() : this("a film node") { }

        public Film(string name) :
            base(name)
        {
            FinalizeConstructor();
        }

        internal Film(IntPtr intPtr) : base(intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            FilmNodeInputs = new FilmNodeInputs(this);

        }
        public void Update()
        {
        }
        public uint GetKernelFeatures(IntPtr scene) {
            return CSycles.film_get_kernel_features(Ptr, scene);
        }
        public int GetAovOffset(IntPtr scene, string name, bool is_color) {
            return CSycles.film_get_aov_offset(Ptr, scene, name, is_color);
        }
        public static IntPtr GetNodeType() {
            return CSycles.film_get_node_type();
        }
        public static void AddDefault(IntPtr scene) {
            CSycles.film_add_default(scene);
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "pass_alpha_threshold":
                    /* film . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'pass_alpha_threshold', 'ui_name': 'Pass Alpha Threshold'} */
                    {
                    CSycles.film_set_pass_alpha_threshold(this.Ptr, data);
                    }
                    break;
            case "exposure":
                    /* film . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'exposure', 'ui_name': 'Exposure'} */
                    {
                    CSycles.film_set_exposure(this.Ptr, data);
                    }
                    break;
            case "mist_start":
                    /* film . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'mist_start', 'ui_name': 'Mist Start'} */
                    {
                    CSycles.film_set_mist_start(this.Ptr, data);
                    }
                    break;
            case "filter_width":
                    /* film . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'filter_width', 'ui_name': 'Filter Width'} */
                    {
                    CSycles.film_set_filter_width(this.Ptr, data);
                    }
                    break;
            case "mist_depth":
                    /* film . {'datatype': 'FLOAT', 'default_value': '100.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'mist_depth', 'ui_name': 'Mist Depth'} */
                    {
                    CSycles.film_set_mist_depth(this.Ptr, data);
                    }
                    break;
            case "mist_falloff":
                    /* film . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'mist_falloff', 'ui_name': 'Mist Falloff'} */
                    {
                    CSycles.film_set_mist_falloff(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Film (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "show_active_pixels":
                    /* film . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'show_active_pixels', 'ui_name': 'Show Active Pixels'} */
                    {
                    CSycles.film_set_show_active_pixels(this.Ptr, data);
                    }
                    break;
            case "use_approximate_shadow_catcher":
                    /* film . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_approximate_shadow_catcher', 'ui_name': 'Use Approximate Shadow Catcher'} */
                    {
                    CSycles.film_set_use_approximate_shadow_catcher(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Film (setter)");
            }
        }

        internal override void SetInt(string name, int data)
        {
            switch(name) {
            case "cryptomatte_depth":
                    /* film . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'cryptomatte_depth', 'ui_name': 'Cryptomatte Depth'} */
                    {
                    CSycles.film_set_cryptomatte_depth(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Film (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "cryptomatte_passes":
                    /* film . {'datatype': 'ENUM', 'default_value': 'CRYPT_NONE', 'default_value_type': 'CryptomatteType', 'is_input': True, 'member_name': 'cryptomatte_passes', 'ui_name': 'Cryptomatte Passes'} */
                    {
                    CSycles.film_set_cryptomatte_passes(this.Ptr, (ccl.CryptomatteType)data);
                    }
                    break;
            case "filter_type":
                    /* film . {'datatype': 'ENUM', 'default_value': 'FILTER_BOX', 'default_value_type': 'FilterType', 'is_input': True, 'member_name': 'filter_type', 'ui_name': 'Filter Type'} */
                    {
                    CSycles.film_set_filter_type(this.Ptr, (ccl.FilterType)data);
                    }
                    break;
            case "display_pass":
                    /* film . {'datatype': 'ENUM', 'default_value': 'PASS_COMBINED', 'default_value_type': 'PassType', 'is_input': True, 'member_name': 'display_pass', 'ui_name': 'Display Pass'} */
                    {
                    CSycles.film_set_display_pass(this.Ptr, (ccl.PassType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Film (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "pass_alpha_threshold":
                /* film . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'pass_alpha_threshold', 'ui_name': 'Pass Alpha Threshold'} */
                {
                    return CSycles.film_get_pass_alpha_threshold(this.Ptr);
                }
            case "exposure":
                /* film . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'exposure', 'ui_name': 'Exposure'} */
                {
                    return CSycles.film_get_exposure(this.Ptr);
                }
            case "mist_start":
                /* film . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'mist_start', 'ui_name': 'Mist Start'} */
                {
                    return CSycles.film_get_mist_start(this.Ptr);
                }
            case "filter_width":
                /* film . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'filter_width', 'ui_name': 'Filter Width'} */
                {
                    return CSycles.film_get_filter_width(this.Ptr);
                }
            case "mist_depth":
                /* film . {'datatype': 'FLOAT', 'default_value': '100.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'mist_depth', 'ui_name': 'Mist Depth'} */
                {
                    return CSycles.film_get_mist_depth(this.Ptr);
                }
            case "mist_falloff":
                /* film . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'mist_falloff', 'ui_name': 'Mist Falloff'} */
                {
                    return CSycles.film_get_mist_falloff(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Film (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "show_active_pixels":
                /* film . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'show_active_pixels', 'ui_name': 'Show Active Pixels'} */
                {
                    return CSycles.film_get_show_active_pixels(this.Ptr);
                }
            case "use_approximate_shadow_catcher":
                /* film . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_approximate_shadow_catcher', 'ui_name': 'Use Approximate Shadow Catcher'} */
                {
                    return CSycles.film_get_use_approximate_shadow_catcher(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Film (getter)");
            }
        }

        internal override int GetInt(string name)
        {
            switch(name) {
            case "cryptomatte_depth":
                /* film . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'cryptomatte_depth', 'ui_name': 'Cryptomatte Depth'} */
                {
                    return CSycles.film_get_cryptomatte_depth(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Film (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "cryptomatte_passes":
                /* film . {'datatype': 'ENUM', 'default_value': 'CRYPT_NONE', 'default_value_type': 'CryptomatteType', 'is_input': True, 'member_name': 'cryptomatte_passes', 'ui_name': 'Cryptomatte Passes'} */
                {
                    return (uint)CSycles.film_get_cryptomatte_passes(this.Ptr);
                }
            case "filter_type":
                /* film . {'datatype': 'ENUM', 'default_value': 'FILTER_BOX', 'default_value_type': 'FilterType', 'is_input': True, 'member_name': 'filter_type', 'ui_name': 'Filter Type'} */
                {
                    return (uint)CSycles.film_get_filter_type(this.Ptr);
                }
            case "display_pass":
                /* film . {'datatype': 'ENUM', 'default_value': 'PASS_COMBINED', 'default_value_type': 'PassType', 'is_input': True, 'member_name': 'display_pass', 'ui_name': 'Display Pass'} */
                {
                    return (uint)CSycles.film_get_display_pass(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Film (getter)");
            }
        }

#endregion
    }

}