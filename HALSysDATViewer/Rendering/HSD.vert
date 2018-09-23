#version 330

in vec3 in_pos;
in vec3 in_nrm;
in vec2 in_tex0;
in vec4 in_binds;
in vec4 in_weights;

out vec2 UV0;

uniform mat4 mvp;
uniform int JOBJIndex;
uniform mat4 binds[100];
uniform int UVSW;
uniform int UVSH;

void main()
{

UV0 = vec2(in_tex0.x * UVSW, in_tex0.y * UVSH);

vec4 Pos = vec4(in_pos, 1);

Pos = binds[JOBJIndex] * Pos;
if(in_weights.x == 1)
	Pos = binds[int(in_binds.x)] * Pos;

gl_Position = mvp * vec4(Pos.xyz, 1);

}