
#version 330

#define PASS_AMBIENT 1
#define PASS_DIFFUSE 2
#define PASS_SPECULAR 3
#define PASS_EXT 4

#define COLORMAP_NONE 0
#define COLORMAP_ALPHA_MASK 1
#define COLORMAP_RGB_MASK 2
#define COLORMAP_BLEND 3
#define COLORMAP_MODULATE 4
#define COLORMAP_REPLACE 5
#define COLORMAP_PASS 6
#define COLORMAP_ADD 7
#define COLORMAP_SUB 8

#define ALPHAMAP_NONE 0
#define ALPHAMAP_ALPHA_MASK 1
#define ALPHAMAP_BLEND 2
#define ALPHAMAP_MODULATE 3
#define ALPHAMAP_REPLACE 4
#define ALPHAMAP_PASS 5
#define ALPHAMAP_ADD 6
#define ALPHAMAP_SUB 7

#define MAX_TEX 4

in vec3 normal;
in vec3 tan;
in vec3 bitan;
in vec2 texcoord[MAX_TEX];

struct TexUnit
{
	int is_ambient;
	int is_diffuse;
	int is_specular;
	int is_ext;
	int is_bump;
	int color_operation;
	int alpha_operation;
	int coord_type;
	float blend;
	vec2 uv_scale;
	int mirror_fix;
	mat4 transform;
};
uniform int hasTEX[MAX_TEX];
uniform TexUnit TEX[MAX_TEX];

uniform sampler2D textures[MAX_TEX];

// 
vec2 GetCoordType(int coordType, vec2 tex0, vec3 N);

///
///
///
vec2 CalculateCoords(TexUnit tex, vec2 texCoord)
{
    vec2 coords = GetCoordType(tex.coord_type, texCoord, normal);

	coords = (tex.transform * vec4(coords.x, coords.y, 0, 1)).xy;
	
	coords *= tex.uv_scale;

	if(tex.mirror_fix == 1) // GX OPENGL difference
		coords.y += 1;

	return coords;
}

///
///
///
vec4 GetBumpShading(vec3 V)
{
	for(int i = 0; i < MAX_TEX ; i++)
	{
		if (hasTEX[i] == 1)
		{
			if(TEX[i].is_bump == 1) 
			{
				vec2 tex0 = CalculateCoords(TEX[i], texcoord[i]);
				vec2 tex1 = tex0 + vec2(dot(V, bitan.xyz), dot(V, tan.xyz));

				vec3 bump0 = texture(textures[i], tex0).rgb;
				vec3 bump1 = texture(textures[i], tex1).rgb;

				return vec4((bump0 - bump1) + 1.0, 1);
			}
		}
	}
	return vec4(1);
}

///
/// Gets the texture color after applying all necessary transforms and TEV
///
vec4 GetTextureFragment(int index)
{
	if (hasTEX[index] == 0)
		return vec4(0);
	else
		return texture(textures[index], CalculateCoords(TEX[index], texcoord[index]));
}

///
///
///
vec4 PerformTextureOp(int index, vec4 passColor)
{
	vec4 pass = GetTextureFragment(index);
	
	switch (TEX[index].color_operation)
	{
		case COLORMAP_NONE:
			break;
		case COLORMAP_ALPHA_MASK:
			if(pass.a != 0)
				passColor.rgb = mix(passColor.rgb, pass.rgb, pass.a);
			break;
		case COLORMAP_RGB_MASK:
			{
				//TODO: I don't know what this is
				if(pass.r != 0)
					passColor.r = pass.r;
				else
					passColor.r = 0;
				if(pass.g != 0)
					passColor.g = pass.g;
				else
					passColor.g = 0;
				if(pass.b != 0)
					passColor.b = pass.b;
				else
					passColor.b = 0;
			}
			break;
		case COLORMAP_BLEND:
			passColor.rgb = mix(passColor.rgb, pass.rgb, TEX[index].blend);
			break;
		case COLORMAP_MODULATE:
			passColor.rgb *= pass.rgb;
			break;
		case COLORMAP_REPLACE:
			passColor.rgb = pass.rgb;
			break;
		case COLORMAP_PASS:
			break;
		case COLORMAP_ADD:
			passColor.rgb += pass.rgb * pass.a;
			break;
		case COLORMAP_SUB:
			passColor.rgb -= pass.rgb * pass.a;
			break;
	}

	switch (TEX[index].alpha_operation)
	{
		case ALPHAMAP_NONE:
			break;
		case ALPHAMAP_ALPHA_MASK:
			// TODO: alpha mask with alpha?
			break;
		case ALPHAMAP_BLEND:
			passColor.a = mix(passColor.a, pass.a, TEX[index].blend);
			break;
		case ALPHAMAP_MODULATE:
			passColor.a *= pass.a;
			break;
		case ALPHAMAP_REPLACE:
			passColor.a = pass.a;
			break;
		case ALPHAMAP_PASS:
			break;
		case ALPHAMAP_ADD:
			passColor.a += pass.a;
			break;
		case ALPHAMAP_SUB:
			passColor.a -= pass.a;
			break;
	}

	return passColor;
}

///
///
///
vec4 TexturePass(vec4 color, int pass_type)
{
	for(int i = 0; i < MAX_TEX ; i++)
	{
		if (hasTEX[i] == 1)
		{
			if(
			(pass_type == PASS_DIFFUSE && TEX[i].is_diffuse == 1) ||
			(pass_type == PASS_SPECULAR && TEX[i].is_specular == 1) ||
			(pass_type == PASS_AMBIENT && TEX[i].is_ambient == 1) ||
			(pass_type == PASS_EXT && TEX[i].is_ext == 1)
			)
				color = PerformTextureOp(i, color);
		}
	}

	return color;
}