#version 330

#define COORD_UV 0
#define COORD_REFLECTION 1
#define COORD_HIGHLIGHT 2
#define COORD_SHADOW 3
#define COORD_TOON 4
#define COORD_GRADATION 5

#define GX_TG_POS 0
#define GX_TG_NRM 1
#define GX_TG_BINRM 2
#define GX_TG_TANGENT 3
#define GX_TG_TEX0 4
#define GX_TG_TEX1 5
#define GX_TG_TEX2 6
#define GX_TG_TEX3 7
#define GX_TG_TEX4 8
#define GX_TG_TEX5 9
#define GX_TG_TEX6 10
#define GX_TG_TEX7 11
#define GX_TG_TEXCOORD0 12
#define GX_TG_TEXCOORD1 13
#define GX_TG_TEXCOORD2 14
#define GX_TG_TEXCOORD3 15
#define GX_TG_TEXCOORD4 16
#define GX_TG_TEXCOORD5 17
#define GX_TG_TEXCOORD6 18
#define GX_TG_COLOR0 19
#define GX_TG_COLOR1 20

in vec3 posVA;
in vec3 normal;
in vec3 ntan;
in vec3 bitan;
in vec3 vertPosition;
in vec2 texcoord[4];

uniform mat4 sphereMatrix;
uniform vec3 cameraPos;

uniform struct Light
{
	int useCamera;
	vec3 position;
	vec4 ambient;
	vec4 diffuse;
	float ambientPower;
	float diffusePower;
} light;

///
/// Gets spherical UV coords, this is used for reflection effects
///
vec2 GetSphereCoords()
{
    vec3 viewNormal = mat3(sphereMatrix) * normal;

	vec2 cord = viewNormal.xy * 0.5 + 0.5;

	cord.y = 1 - cord.y;

    return cord;
}

///
/// Gets TOON coords
///
vec2 GetToonCoords()
{
	vec3 V = vertPosition - cameraPos;

	if(light.useCamera == 0)
		V = light.position;

	V = normalize(V);

    float lambert = clamp(dot(normal, V) + 0.4, 0, 1);

    return vec2(lambert, lambert);
}

///
/// Returns Coords for specified coord type
///
vec2 GetCoordType(int coordType, int gensrc)
{
	vec2 tex0 = vec2(0, 0);

	// TODO: more texture uv channels
	switch (gensrc)
	{
		case GX_TG_POS:		tex0 = vec2(posVA.x, posVA.y); break;
		case GX_TG_NRM:		tex0 = vec2(normal.x, normal.y); break;
		case GX_TG_BINRM:	tex0 = vec2(bitan.x, bitan.y); break;
		case GX_TG_TANGENT:	tex0 = vec2(ntan.x, ntan.y);  break;

		case GX_TG_TEX0: tex0 = texcoord[0]; break;
		case GX_TG_TEX1: tex0 = texcoord[1]; break;
		case GX_TG_TEX2: tex0 = texcoord[2]; break;
		case GX_TG_TEX3: tex0 = texcoord[3]; break;
		case GX_TG_TEX4: tex0 = texcoord[3]; break;
		case GX_TG_TEX5: tex0 = texcoord[3]; break;
		case GX_TG_TEX6: tex0 = texcoord[3]; break;
		case GX_TG_TEX7: tex0 = texcoord[3]; break;
		
		case GX_TG_TEXCOORD0: tex0 = texcoord[0]; break;
		case GX_TG_TEXCOORD1: tex0 = texcoord[1]; break;
		case GX_TG_TEXCOORD2: tex0 = texcoord[2]; break;
		case GX_TG_TEXCOORD3: tex0 = texcoord[3]; break;
		case GX_TG_TEXCOORD4: tex0 = texcoord[3]; break;
		case GX_TG_TEXCOORD5: tex0 = texcoord[3]; break;
		case GX_TG_TEXCOORD6: tex0 = texcoord[3]; break;

		case GX_TG_COLOR0:
			// this is usually for toon shading
			break;
		case GX_TG_COLOR1:
			// this seems to be some combination of spherical reflection and toon shading
			break;
	}

	//COORD_HIGHLIGHT
    //COORD_SHADOW
    //COORD_GRADATION

	switch(coordType)
	{
		case COORD_REFLECTION:
			return GetSphereCoords();
			break;
		case COORD_TOON:
			return GetToonCoords();
			break;
		case COORD_UV:
		default:
			return tex0;
			break;
	}

	return tex0;
}