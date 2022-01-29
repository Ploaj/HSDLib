#version 330

#define GX_CC_CPREV 0
#define GX_CC_APREV 1
#define GX_CC_C0 2
#define GC_CC_A0 3
#define GX_CC_C1 4
#define GC_CC_A1 5
#define GX_CC_C2 6
#define GC_CC_A2 7
#define GX_CC_TEXC 8
#define GX_CC_TEXA 9
#define GX_CC_RASC 10
#define GX_CC_RASA 11
#define GX_CC_ONE 12
#define GX_CC_HALF 13
#define GX_CC_KONST 14
#define GX_CC_ZERO 15

#define GX_CS_SCALE_1 0
#define GX_CS_SCALE_2 1
#define GX_CS_SCALE_4 2
#define GX_CS_DIVIDE_2 3

#define GX_TB_ZERO 0
#define GX_TB_ADDHALF 1
#define GX_TB_SUBHALF 2

#define MAX_TEX 4

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
/*
///
/// Gets the inputs for TEV color operation
///
vec3 GetTEVColorIn(int type, vec4 tex, vec4 konst, vec4 regPrev, vec4 reg0, vec4 reg1)
{
	switch(type)
	{
		case GX_CC_CPREV: return regPrev.rgb; break;
		case GX_CC_APREV: return regPrev.aaa; break;
		case GX_CC_C0: return reg0.rgb; break;
		case GC_CC_A0: return reg0.aaa; break;
		case GX_CC_C1: return reg1.rgb; break;
		case GC_CC_A1: return reg1.aaa; break;
		case GX_CC_C2: return vec3(0);  break;// unused for hsd?
		case GC_CC_A2: return vec3(0);  break;// unused for hsd?
		case GX_CC_TEXC: return tex.rgb; break;
		case GX_CC_TEXA: return tex.aaa; break;
		case GX_CC_RASC: return vertexColor.rgb; break;
		case GX_CC_RASA: return vertexColor.aaa; break;
		case GX_CC_ONE: return vec3(1, 1, 1); break;
		case GX_CC_HALF: return vec3(0.5, 0.5, 0.5); break;
		case GX_CC_KONST: return konst.rgb; break;
		default: return vec3(0); break;
	}
}

///
/// Gets the value of tev bias
///
float GetTEVBias(int bias)
{
	switch(bias)
	{
	case GX_TB_ZERO: return 0; break;
	case GX_TB_ADDHALF: return 0.5; break;
	case GX_TB_SUBHALF: return -0.5; break;
	}

	return 0;
}

///
/// Gets the value of tev bias
///
float GetTEVScale(int scale)
{
	switch(scale)
	{
	case GX_CS_SCALE_1: return 1; break;
	case GX_CS_SCALE_2: return 2; break;
	case GX_CS_SCALE_4: return 4; break;
	case GX_CS_DIVIDE_2: return 0.5; break;
	}

	return 1;
}

///
/// Applys TEV color operation
///
vec4 ApplyTEVColorOp(int op, bool is_alpha, float bias, float scale, bool tclamp, vec4 a, vec4 b, vec4 c, vec4 d)
{
	vec4 col = vec4(0);

	switch(op)
	{
		case 0: 
			col = (d + ((vec4(1) - c ) * a + c * b) + vec4(bias)) * vec4(scale); 
			if(tclamp)
				col = clamp(col, vec4(0), vec4(1));
			return col;
		case 1: 
			col = (d + ((vec4(1) - c ) * a + c * b) + vec4(bias)) * vec4(scale); 
			if(tclamp)
				col = clamp(col, vec4(0), vec4(1));
			return col;
		case 2: return d + ((a.r > b.r) ? c : col); 
		case 3: return d + ((a.r == b.r) ? c : col); 
		case 4: return d + ((a.g > b.g && a.r > b.r) ? c : col); 
		case 5: return d + ((a.g == b.g && a.r == b.r) ? c : col); 
		case 6: return d + ((a.b > b.b && a.g > b.g && a.r > b.r) ? c : col); 
		case 7: return d + ((a.b == b.b && a.g == b.g && a.r == b.r) ? c : col); 
		case 8: 
			if(is_alpha)
				return d + ((a.a > b.a) ? c : col);
			else
			{
				float re = d.r + ((a.r > b.r) ? c.r : 0);
				float gr = d.g + ((a.g > b.g) ? c.g : 0);
				float bl = d.b + ((a.b > b.b) ? c.b : 0);
				return vec4(re, gr, bl, 0);
			}
		break;
		case 9:
			if(is_alpha)
				return d + ((a.a == b.a) ? c : col);
			else
			{
				float re = d.r + ((a.r == b.r) ? c.r : 0);
				float gr = d.g + ((a.g == b.g) ? c.g : 0);
				float bl = d.b + ((a.b == b.b) ? c.b : 0);
				return vec4(re, gr, bl, 0);
			}
		break;
	}

	return col;
}


///
/// 
///
vec4 ApplyTev(vec4 texColor, TevUnit tev)
{
	vec4 a = ApplyTEVColorOp(
		tev.color_op, 
		true, 
		GetTEVBias(tev.color_bias), 
		GetTEVScale(tev.color_scale), 
		tev.color_clamp == 1, 
		vec4(GetTEVColorIn(tev.color_a, texColor, tev.konst, texColor, tev.tev0, tev.tev1), 1),
		vec4(GetTEVColorIn(tev.color_b, texColor, tev.konst, texColor, tev.tev0, tev.tev1), 1),
		vec4(GetTEVColorIn(tev.color_c, texColor, tev.konst, texColor, tev.tev0, tev.tev1), 1),
		vec4(GetTEVColorIn(tev.color_d, texColor, tev.konst, texColor, tev.tev0, tev.tev1), 1));
		
	vec4 c = ApplyTEVColorOp(
		tev.alpha_op, 
		false, 
		GetTEVBias(tev.alpha_bias), 
		GetTEVScale(tev.alpha_scale), 
		tev.alpha_clamp == 1, 
		vec4(GetTEVColorIn(tev.alpha_a, texColor, tev.konst, texColor, tev.tev0, tev.tev1), 1),
		vec4(GetTEVColorIn(tev.alpha_b, texColor, tev.konst, texColor, tev.tev0, tev.tev1), 1),
		vec4(GetTEVColorIn(tev.alpha_c, texColor, tev.konst, texColor, tev.tev0, tev.tev1), 1),
		vec4(GetTEVColorIn(tev.alpha_d, texColor, tev.konst, texColor, tev.tev0, tev.tev1), 1));

	return vec4(c.rgb, a.r);
}
*/