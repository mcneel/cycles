static const GLchar* fs_src =
"#version 330                                                     \n"

"uniform sampler2D tex;                                           \n"
"uniform vec4 subsize;                                            \n"
"uniform vec4 vp_rect;                                            \n"
"uniform float alpha;                                             \n"

"out vec4 Color;                                                  \n"

"void main()                                                      \n"
"{                                                                \n"
"  vec2 shifted = gl_FragCoord.xy - vp_rect.xy;                   \n"

"  if(shifted.x < subsize.x || shifted.x > subsize.x + subsize.z || \n"
"     shifted.y < subsize.y || shifted.y > subsize.y + subsize.w)   \n"
"  {                                                              \n"
"    discard;                                                     \n"
"  }                                                              \n"

"  vec2 tc = (shifted - subsize.xy) / subsize.zw;                 \n"
"  vec4 px = texture(tex, tc);                                    \n"
"  Color = vec4(px.rgb, alpha);                                   \n"
"}                                                                \n";
