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

#define GX_CC_TEXC 8
#define GX_CC_TEXA 9
#define GX_CC_ONE 12
#define GX_CC_HALF 13
#define GX_CC_ZERO 15
#define KONST_RGB (0x01 << 7 | 0)
#define KONST_RRR (0x01 << 7 | 1)
#define KONST_GGG (0x01 << 7 | 2)
#define KONST_BBB (0x01 << 7 | 3)
#define KONST_AAA (0x01 << 7 | 4)
#define TEX0_RGB (0x01 << 7 | 5)
#define TEX0_AAA (0x01 << 7 | 6)
#define TEX1_RGB (0x01 << 7 | 7)
#define TEX1_AAA (0x01 << 7 | 8)

#define NONE 7
#define GX_CC_TEXA 9
#define GX_CC_ZERO 15
#define KONST_R (0x01 << 6 | 0)
#define KONST_G (0x01 << 6 | 1)
#define KONST_B (0x01 << 6 | 2)
#define KONST_A (0x01 << 6 | 3)
#define TEX0_A (0x01 << 6 | 4)
#define TEX1_A (0x01 << 6 | 5)

#define GX_CS_SCALE_1 0
#define GX_CS_SCALE_2 1
#define GX_CS_SCALE_4 2
#define GX_CS_DIVIDE_2 3

#define GX_TB_ZERO 0
#define GX_TB_ADDHALF 1
#define GX_TB_SUBHALF 2

#define GX_TEV_ADD 0
#define GX_TEV_SUB 1
#define GX_TEV_COMP_R8_GT 8
#define GX_TEV_COMP_R8_EQ 9
#define GX_TEV_COMP_GR16_GT 10
#define GX_TEV_COMP_GR16_EQ 11
#define GX_TEV_COMP_BGR24_GT 12
#define GX_TEV_COMP_BGR24_EQ 13
#define GX_TEV_COMP_RGB8_GT 14
#define GX_TEV_COMP_RGB8_EQ 15
#define GX_TEV_COMP_A8_EQ GX_TEV_COMP_RGB8_EQ
#define GX_TEV_COMP_A8_GT GX_TEV_COMP_RGB8_GT

#define MAX_TEX 4

in vec3 tan;
in vec3 bitan;

struct TexUnit
{
	int gensrc;
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
	sampler2D texture;
};
uniform int hasTEX[MAX_TEX];
uniform TexUnit TEX[MAX_TEX];

struct TevUnit
{
	int color_op;
	int alpha_op;
	int color_bias;
	int alpha_bias;
	int color_scale;
	int alpha_scale;
	int color_clamp;
	int alpha_clamp;
	int color_a;
	int color_b;
	int color_c;
	int color_d;
	int alpha_a;
	int alpha_b;
	int alpha_c;
	int alpha_d;
	vec4 konst;
	vec4 tev0;
	vec4 tev1;
};

uniform int hasTev[MAX_TEX];
uniform TevUnit Tev[MAX_TEX];

// 
vec2 GetCoordType(int coordType, int gensrc);

///
///
///
vec2 CalculateCoords(TexUnit tex)
{
    vec2 coords = GetCoordType(tex.coord_type, tex.gensrc);

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
		if (hasTEX[i] == 1 && TEX[i].is_bump == 1)
		{
			vec2 tex0 = CalculateCoords(TEX[i]);
			vec2 tex1 = tex0 + vec2(dot(V, bitan), dot(V, tan));

			vec3 bump0 = texture(TEX[i].texture, tex0).rgb;
			vec3 bump1 = texture(TEX[i].texture, tex1).rgb;

			return vec4((bump0 - bump1) + 1.0, 1);
		}
	}
	return vec4(1);
}

///
///
///
vec3 TevUnit_GetInput(TevUnit unit, vec4 tex, int source)
{
	switch (source)
	{
		case GX_CC_TEXC:
			return tex.rgb;
		case GX_CC_TEXA:
			return tex.aaa;
		case GX_CC_ONE:
			return vec3(1);
		case GX_CC_HALF:
			return vec3(0.5);
		case GX_CC_ZERO:
			return vec3(0);
		case KONST_RGB:
			return unit.konst.rgb;
		case KONST_RRR:
			return unit.konst.rrr;
		case KONST_GGG:
			return unit.konst.ggg;
		case KONST_BBB:
			return unit.konst.bbb;
		case KONST_AAA:
			return unit.konst.aaa;
		case TEX0_RGB:
			return unit.tev0.rgb;
		case TEX0_AAA:
			return unit.tev0.aaa;
		case TEX1_RGB:
			return unit.tev1.rgb;
		case TEX1_AAA:
			return unit.tev1.aaa;
	}
	return tex.rgb;
}

float TevUnit_GetBias(TevUnit unit)
{
	switch (unit.color_bias)
	{
		case GX_TB_ADDHALF:
			return 0.5;
		case GX_TB_SUBHALF:
			return -0.5;
		case GX_TB_ZERO:
		default:
			return 0.0;
	}
}

float TevUnit_GetScale(TevUnit unit)
{
	switch (unit.color_scale)
	{
		case GX_CS_SCALE_2:
			return 2.0;
		case GX_CS_SCALE_4:
			return 4.0;
		case GX_CS_DIVIDE_2:
			return 0.5;
		case GX_CS_SCALE_1:
		default:
			return 1.0;
	}
}

///
///
///
vec4 ApplyTEV(int index)
{
	vec4 TEX = texture(TEX[index].texture, CalculateCoords(TEX[index]));

	if (hasTev[index] == 1)
	{
		// get tev unit
		TevUnit tev = Tev[index];

		// inputs
		vec3 a = TevUnit_GetInput(tev, TEX, tev.color_a);
		vec3 b = TevUnit_GetInput(tev, TEX, tev.color_b);
		vec3 c = TevUnit_GetInput(tev, TEX, tev.color_c);
		vec3 d = TevUnit_GetInput(tev, TEX, tev.color_d);
		
		// op
		vec3 op = vec3(0);
		switch (tev.color_op)
		{
			case GX_TEV_ADD:
				op = d + (a * (1 - c) + b * c);
				break;
			case GX_TEV_SUB:
				op = d - (a * (1 - c) + b * c);
				break;
			case GX_TEV_COMP_R8_GT:
				op = d + ((a.r > b.r) ? c : vec3(0));
				break;
			case GX_TEV_COMP_R8_EQ:
				op = d + ((a.r == b.r) ? c : vec3(0));
				break;
			case GX_TEV_COMP_GR16_GT:
				op = d + (all(greaterThan(a.gr, b.gr)) ? c : vec3(0));
				break;
			case GX_TEV_COMP_GR16_EQ:
				op = d + ((a.gr == b.gr) ? c : vec3(0));
				break;
			case GX_TEV_COMP_BGR24_GT:
				op = d + (all(greaterThan(a.bgr, b.bgr)) ? c : vec3(0));
				break;
			case GX_TEV_COMP_BGR24_EQ:
				op = d + ((a.bgr == b.bgr) ? c : vec3(0));
				break;
			case GX_TEV_COMP_RGB8_GT:
				op.r = d.r + ((a.r > b.r) ? c.r : 0);
				op.g = d.g + ((a.g > b.g) ? c.g : 0);
				op.b = d.b + ((a.b > b.b) ? c.b : 0);
				break;
			case GX_TEV_COMP_RGB8_EQ:
				op.r = d.r + ((a.r == b.r) ? c.r : 0);
				op.g = d.g + ((a.g == b.g) ? c.g : 0);
				op.b = d.b + ((a.b == b.b) ? c.b : 0);
				break;
		}
		
		if (tev.color_op == GX_TEV_ADD || tev.color_op == GX_TEV_SUB)
		{
			// bias and scale
			float bias = TevUnit_GetBias(tev);
			float scale = TevUnit_GetScale(tev);
			op = (op + vec3(bias)) * scale;

			// clamp
			if (tev.color_clamp == 1)
				op = clamp(op, vec3(0), vec3(1));
		}

		return vec4(op, TEX.a);
	}

	return TEX;
}

///
/// Gets the texture color after applying all necessary transforms and TEV
///
vec4 GetTextureFragment(int index)
{
	if (hasTEX[index] == 0)
		return vec4(0);
	else
		return ApplyTEV(index);
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

///
///
///
vec4 GetToonTexture()
{
	for(int i = 0; i < MAX_TEX ; i++)
	{
		if (hasTEX[i] == 1)
		{
			if(TEX[i].coord_type == 4)
			{
				return GetTextureFragment(i);
			}
		}
	}

	return vec4(0, 0, 0, 1);
}