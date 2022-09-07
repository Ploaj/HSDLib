#version 330

layout (location = 0) in vec3 pos;
layout (location = 1) in vec2 uv;
layout (location = 2) in vec4 col;

out vec4 color;
out vec2 texcoord;

uniform vec2 texscale;
uniform mat4 mvp;

void main()
{
	color = col;
	texcoord = uv * texscale;

	if(texscale.y > 1) // correct wrap mode
		texcoord.y += 1;

	gl_Position = mvp * vec4(pos, 1);

	return;
}