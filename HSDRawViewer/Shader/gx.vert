#version 330

#define MAX_TEX 4
#define MAX_WEIGHTS 6
#define WEIGHT_STRIDE 10

#define MAX_BONES 200

layout (location = 0) in float PNMTXIDX;
layout (location = 1) in vec3 GX_VA_POS;
layout (location = 2) in vec3 GX_VA_NRM;
layout (location = 3) in vec3 GX_VA_TAN;
layout (location = 4) in vec3 GX_VA_BTAN;
layout (location = 5) in vec2 GX_VA_TEX0;
layout (location = 6) in vec2 GX_VA_TEX1;
layout (location = 7) in vec2 GX_VA_TEX2;
layout (location = 8) in vec2 GX_VA_TEX3;
layout (location = 9) in vec4 GX_VA_CLR0;

layout (location = 10) in vec3 GX_VA_POS_SHAPE;
layout (location = 11) in vec3 GX_VA_NRM_SHAPE;

out vec3 posVA;
out vec3 vertPosition;
out vec3 normal;
out vec3 ntan;
out vec3 bitan;

out vec3 ambientLight;
out vec3 diffuseLight;
out vec3 specularLight;

out vec2 texcoord[MAX_TEX];
out vec4 vertexColor;

flat out int vbones[MAX_WEIGHTS];
out float vweights[MAX_WEIGHTS];
out float fogAmt;

uniform mat4 mvp;

uniform int isSkeleton;
uniform int enableParentTransform;
uniform mat4 singleBind;

uniform int hasEnvelopes;

uniform BoneTransforms
{
    mat4 transforms[MAX_BONES];
    mat4 binds[MAX_BONES];
};

uniform int envelopeIndex[WEIGHT_STRIDE * MAX_WEIGHTS];
uniform float weights[WEIGHT_STRIDE * MAX_WEIGHTS];

uniform float shape_blend;

uniform vec3 cameraPos;

// material
uniform int perPixelLighting;
void CalculateDiffuseShading(vec3 vert, vec3 N, inout vec3 amb, inout vec3 diff, inout vec3 spec);

// fog
uniform struct Fog
{
	int type;
	float start;
	float end;
	vec4 color;
} fog;

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
	
	ntan = GX_VA_TAN;
	bitan = GX_VA_BTAN;
	normal = mix(GX_VA_NRM, GX_VA_NRM_SHAPE, shape_blend);

	
	for (int i = 0; i < MAX_WEIGHTS; i += 1)
	{
		vbones[i] = 0;
		vweights[i] = 0.0;
	}

	if (hasEnvelopes == 1)
	{
		int matrixIndex = int(PNMTXIDX / 3);

		// set output attributes
		for (int i = 0; i < MAX_WEIGHTS; i += 1)
		{
			vbones[i] = envelopeIndex[matrixIndex * MAX_WEIGHTS + i];
			vweights[i] = weights[matrixIndex * MAX_WEIGHTS + i];
		}

		// always transform by parent
		if (isSkeleton == 0)
		{
			pos = singleBind * pos;
			normal = (inverse(transpose(singleBind)) * vec4(normal, 1)).xyz;
		}

		// single bind optimization
		if(isSkeleton == 1 && vweights[0] == 1.0)
		{
			pos = transforms[vbones[0]] * vec4(pos.xyz, 1);
			normal = (inverse(transpose(transforms[vbones[0]])) * vec4(normal, 1)).xyz;
		}
		else
		{
			// skin mesh
			vec3 skinnedPos = vec3(0);
			vec3 skinnedNrm = vec3(0);
			for(int i = 0; i < MAX_WEIGHTS; i += 1)
			{
				if(vweights[i] > 0)
				{
					mat4 transform = binds[vbones[i]];
					skinnedPos += (transform * vec4(pos.xyz, 1) * vweights[i]).xyz;
					skinnedNrm += (inverse(transpose(transform)) * vec4(normal, 1) * vweights[i]).xyz;
				}
			}
			pos = vec4(skinnedPos, 1);
			normal = skinnedNrm;
		}
	}
	else
	if (enableParentTransform == 1)
	{
		pos = singleBind * pos;
		normal = (inverse(transpose(singleBind)) * vec4(normal, 1)).xyz;
	}
	else
	if (enableParentTransform == 2)
	{
		int matrixIndex = int(PNMTXIDX / 3);

		// set output attributes
		for (int i = 0; i < MAX_WEIGHTS; i += 1)
		{
			vbones[i] = envelopeIndex[matrixIndex * MAX_WEIGHTS + i];
			vweights[i] = weights[matrixIndex * MAX_WEIGHTS + i];
		}

		// 
		pos = transforms[vbones[0]] * vec4(pos.xyz, 1);
		normal = (inverse(transpose(transforms[vbones[0]])) * vec4(normal, 1)).xyz;
	}
	
	// raw outputs
	posVA = GX_VA_POS;
	vertPosition = pos.xyz;

	texcoord[0] = GX_VA_TEX0;
	texcoord[1] = GX_VA_TEX1;
	texcoord[2] = GX_VA_TEX2;
	texcoord[3] = GX_VA_TEX3;

	vertexColor = GX_VA_CLR0;
	
	// view lighting calculations
	normal = normalize(normal);
	if (perPixelLighting == 0)
	{
		CalculateDiffuseShading(vertPosition, normal, ambientLight, diffuseLight, specularLight);
	}
	else
	{
		ambientLight = vec3(0);
		diffuseLight = vec3(0);
		specularLight = vec3(0);
	}

	// final position
	gl_Position = mvp * vec4(pos.xyz, 1);

	// fog calcuation
	if(fog.type == 1)
		fogAmt = fogFactorLinear(length(gl_Position.xyz), fog.start, fog.end);
	else
		fogAmt = 0;
	
}