#version 330

#define MAX_TEX 4

in float PNMTXIDX;
in vec3 GX_VA_POS;
in vec3 GX_VA_NRM;
in vec3 GX_VA_TAN;
in vec3 GX_VA_BTAN;
in vec2 GX_VA_TEX0;
in vec2 GX_VA_TEX1;
in vec2 GX_VA_TEX2;
in vec2 GX_VA_TEX3;
in vec4 GX_VA_CLR0;

in vec3 GX_VA_POS_SHAPE;
in vec3 GX_VA_NRM_SHAPE;

out vec3 vertPosition;
out vec3 normal;
out vec3 tan;
out vec3 bitan;
out float spec;
out vec2 texcoord[MAX_TEX];
out vec4 vertexColor;
flat out vec4 vbones;
out vec4 vweights;
out float fogAmt;

uniform mat4 mvp;

uniform int isSkeleton;
uniform int enableParentTransform;
uniform mat4 singleBind;

uniform int hasEnvelopes;

uniform BoneTransforms
{
    mat4 transforms[200];
} ;

uniform mat4 binds[200];

uniform vec4 envelopeIndex[10];
uniform vec4 weights[10];

uniform float shape_blend;

uniform vec3 cameraPos;

struct Light
{
	int useCamera;
	vec3 position;
	vec4 ambient;
	vec4 diffuse;
	float ambientPower;
	float diffusePower;
};

uniform Light light;

struct Fog
{
	int type;
	float start;
	float end;
	vec4 color;
};

uniform Fog fog;

///
///
///
float fogFactorLinear(
  const float dist,
  const float start,
  const float end
) 
{
	return 1.0 - clamp((end - dist) / (end - start), 0.0, 1.0);
}

///
///
///
float fogFactorExp(
  const float dist,
  const float density
) 
{
	return 1.0 - clamp(exp(-density * dist), 0.0, 1.0);
}

///
///
///
void main()
{
	vec4 pos = vec4(mix(GX_VA_POS, GX_VA_POS_SHAPE, shape_blend), 1);
	
	tan = GX_VA_TAN;
	bitan = GX_VA_BTAN;
	normal = mix(GX_VA_NRM, GX_VA_NRM_SHAPE, shape_blend);

	vbones = vec4(0, 0, 0, 0);
	vweights = vec4(0, 0, 0, 0);

	if(enableParentTransform == 1 && hasEnvelopes == 0) // todo maybe not accurate
	{
		pos = singleBind * pos;
		normal = (inverse(transpose(singleBind)) * vec4(normal, 1)).xyz;
	}

	if (hasEnvelopes == 1)
	{
		int matrixIndex = int(PNMTXIDX / 3);
		vbones = envelopeIndex[matrixIndex];
		vweights = weights[matrixIndex];
		
		if(isSkeleton == 1 && vweights.x == 1)
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
				if(vweights[i] > 0)
				{
					mat4 transform = binds[int(vbones[i])];
					skinnedPos += (transform * vec4(pos.xyz, 1) * vweights[i]).xyz;
					skinnedNrm += (inverse(transpose(transform)) * vec4(normal, 1) * vweights[i]).xyz;
				}
			}
			pos = vec4(skinnedPos, 1);
			normal = skinnedNrm;
		}
	}
	
	vertPosition = pos.xyz;
	normal = normalize(normal);

	texcoord[0] = GX_VA_TEX0;
	texcoord[1] = GX_VA_TEX1;
	texcoord[2] = GX_VA_TEX2;
	texcoord[3] = GX_VA_TEX3;

	vertexColor = GX_VA_CLR0;
	
	vec3 V = vertPosition - cameraPos;

	if(light.useCamera == 0)
		V = light.position;

	V = normalize(V);

    spec = clamp(dot(normal, V), 0, 1);

	gl_Position = mvp * vec4(pos.xyz, 1);

	if(fog.type == 1)
		fogAmt = fogFactorLinear(length(gl_Position.xyz), fog.start, fog.end);
	else
		fogAmt = 0;
	
}