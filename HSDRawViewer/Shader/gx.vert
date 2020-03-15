#version 330

in float PNMTXIDX;
in vec3 GX_VA_POS;
in vec3 GX_VA_NRM;
in vec2 GX_VA_TEX0;
in vec2 GX_VA_TEX1;
in vec4 GX_VA_CLR0;

out vec3 vertPosition;
out vec3 normal;
out float spec;
out vec2 texcoord0;
out vec2 texcoord1;
out vec4 vertexColor;

uniform mat4 mvp;

uniform int isRootBound;
uniform int notInverted;
uniform mat4 singleBind;

uniform vec3 cameraPos;
uniform int hasEnvelopes;

uniform BoneTransforms
{
    mat4 transforms[200];
} ;

uniform mat4 binds[200];

uniform vec4 envelopeIndex[10];
uniform vec4 weights[10];

void main()
{
	vec4 pos = vec4(GX_VA_POS, 1);
	
	normal = GX_VA_NRM;

	if(notInverted == 0)
	{
		pos = singleBind * pos;
		normal = (inverse(transpose(singleBind)) * vec4(normal, 1)).xyz;
	}

	texcoord0 = GX_VA_TEX0;

	texcoord1 = GX_VA_TEX1;

	vertexColor = GX_VA_CLR0;

	if (hasEnvelopes == 1)
	{
		int matrixIndex = int(PNMTXIDX / 3);

		if(weights[matrixIndex].x == 1 && (isRootBound == 1 || notInverted == 1))
		{
			pos = transforms[int(envelopeIndex[matrixIndex].x)] * vec4(pos.xyz, 1);
			normal = (inverse(transpose(transforms[int(envelopeIndex[matrixIndex].x)])) * vec4(normal, 1)).xyz;
		}
		else
		{
			vec3 skinnedPos = vec3(0);
			vec3 skinnedNrm = vec3(0);
			int i = 0;
			for(i = 0; i < 4 ; i+=1)
			{
				if(weights[matrixIndex][i] > 0)
				{
					mat4 transform = binds[int(envelopeIndex[matrixIndex][i])];
					skinnedPos += (transform * vec4(pos.xyz, 1) * weights[matrixIndex][i]).xyz;
					skinnedNrm += (inverse(transpose(transform)) * vec4(normal, 1) * weights[matrixIndex][i]).xyz;
				}
			}
			pos = vec4(skinnedPos, 1);
			normal = skinnedNrm;
		}
	}
	
	vertPosition = pos.xyz;
	
	vec3 V = normalize(vertPosition - cameraPos);
    spec = clamp(dot(normal, V), 0, 1);

	gl_Position = mvp * vec4(pos.xyz, 1);
	
}