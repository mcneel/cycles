static const GLchar* vs_src =
	"#version 330                            \n"

	"layout(location = 0) in vec2 Vertex;    \n"

	"void main()                             \n"
	"{                                       \n"
	"  gl_Position = vec4(Vertex, 0.0, 1.0); \n"
	"}                                       \n";
