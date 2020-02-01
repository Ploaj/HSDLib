#version 330

in float PNMTXIDX;
in vec3 GX_VA_POS;
in vec3 GX_VA_NRM;
in vec2 GX_VA_TEX0;

out vec3 vertPosition;
out vec3 normal;
out vec2 texcoord0;

uniform mat4 mvp;

uniform mat4 singleBind;

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
	vec4 pos = singleBind * vec4(GX_VA_POS, 1);

	normal = GX_VA_NRM;

	texcoord0 = GX_VA_TEX0;

	if (hasEnvelopes == 1)
	{

	int matrixIndex = int(PNMTXIDX / 3);

	if(weights[matrixIndex].x == 1)
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

	gl_Position = mvp * vec4(pos.xyz, 1);
	
}