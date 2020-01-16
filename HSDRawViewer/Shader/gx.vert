#version 330

in float PNMTXIDX;
in vec3 GX_VA_POS;
in vec3 GX_VA_NRM;
in vec2 GX_VA_TEX0;

out vec3 normal;
out vec2 texcoord0;

uniform vec2 UVScale;

uniform mat4 mvp;

uniform mat4 singleBind;

uniform mat4 transforms[200];

uniform vec4 envelopeIndex[10];
uniform vec4 weights[10];

void main()
{
	vec4 pos = singleBind * vec4(GX_VA_POS, 1);

	normal = GX_VA_NRM;

	texcoord0 = GX_VA_TEX0 * UVScale;

	int matrixIndex = int(PNMTXIDX / 3);

	if(weights[matrixIndex].x == 1)
	{
		pos = transforms[int(envelopeIndex[matrixIndex].x)] * vec4(pos.xyz, 1);
		normal = (inverse(transpose(transforms[int(envelopeIndex[matrixIndex].x)])) * vec4(normal, 1)).xyz;
	}
	
	gl_Position = mvp * vec4(pos.xyz, 1);
	
}