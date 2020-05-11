#version 330

in vec3 vertPosition;
in vec3 normal;
in float spec;
in vec2 texcoord0;
in vec2 texcoord1;
in vec4 vertexColor;
in vec4 vbones;
in vec4 vweights;

out vec4 fragColor;

// textures
struct TexUnit
{
	sampler2D tex;
	int light_type;
	int color_operation;
	int alpha_operation;
	int coord_type;
	float blend;
	vec2 uv_scale;
	int mirror_fix;
	mat4 transform;
};

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

// textures
uniform int hasTEX0;
uniform int hasTEX0Tev;
uniform TexUnit TEX0;
uniform TevUnit TEX0Tev;

uniform int hasTEX1;
uniform int hasTEX1Tev;
uniform TexUnit TEX1;
uniform TevUnit TEX1Tev;

// material
uniform vec4 ambientColor;
uniform vec4 diffuseColor;
uniform vec4 specularColor;
uniform float shinniness;
uniform float alpha;


// flags
uniform int no_zupdate;
uniform int useConstant;
uniform int useVertexColor;
uniform int enableDiffuse;
uniform int enableSpecular;


// Non rendering system

uniform vec3 cameraPos;
uniform int colorOverride;
uniform vec3 overlayColor;
uniform mat4 sphereMatrix;
uniform int renderOverride;
uniform int selectedBone;

///
/// Gets spherical UV coords, this is used for reflection effects
///
vec2 GetSphereCoords(vec3 N)
{
    vec3 viewNormal = mat3(sphereMatrix) * N;
    return viewNormal.xy * 0.5 + 0.5;
}

///
/// Returns Coords for specified coord type
///
vec2 GetCoordType(int coordType, vec2 tex0)
{
	//COORD_UV

	//COORD_REFLECTION
	if(coordType == 1) 
		return GetSphereCoords(normal);
		
	//COORD_HIGHLIGHT
    //COORD_SHADOW
    //COORD_TOON
    //COORD_GRADATION

	return tex0;
}



///
/// Gets the inputs for TEV color operation
///
vec3 GetTEVColorIn(int type, vec4 tex, vec4 konst, vec4 regPrev, vec4 reg0, vec4 reg1)
{
	switch(type)
	{
		case 0: return regPrev.rgb;
		case 1: return regPrev.aaa;
		case 2: return reg0.rgb;
		case 3: return reg0.aaa;
		case 4: return reg1.rgb;
		case 5: return reg1.aaa;
		case 6: return vec3(0); // unused for hsd?
		case 7: return vec3(0); // unused for hsd?
		case 8: return tex.rgb;
		case 9: return tex.aaa;
		case 10: return vertexColor.rgb;
		case 11: return vertexColor.aaa;
		case 12: return vec3(1, 1, 1);
		case 13: return vec3(0.5, 0.5, 0.5);
		case 14: return konst.rgb;
	}
	return vec3(0);
}

///
/// Gets the value of tev bias
///
float GetTEVBias(int bias)
{
	switch(bias)
	{
	case 0: return 0;
	case 1: return 0.5;
	case 2: return -0.5;
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
	case 0: return 1;
	case 1: return 2;
	case 2: return 4;
	case 3: return 0.5;
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

///
/// Gets the texture color after applying all necessary transforms and TEV
///
vec4 MixTextureColor(TexUnit tex, vec2 texCoord)
{
    vec2 coords = GetCoordType(tex.coord_type, texCoord);

	coords = (tex.transform * vec4(coords.x, coords.y, 0, 1)).xy;
	
	coords *= tex.uv_scale;

	if(tex.mirror_fix == 1) // GX OPENGL difference
		coords.y += 1;

    return texture(tex.tex, coords);
}

///
/// Gets the diffuse material
///
vec4 GetDiffuseMaterial(vec3 V, vec3 N)
{
	if(enableDiffuse == 0)
		return vec4(1, 1, 1, 1);

    float lambert = clamp(dot(N, V), 0, 1);

	return vec4(vec3(lambert), 1);
}

///
/// Gets the specular material
///
vec4 GetSpecularMaterial(vec3 V, vec3 N)
{
	if(enableSpecular == 0)
		return vec4(0, 0, 0, 1);
	
    float phong = pow(spec, shinniness);

	return vec4(vec3(phong), 1);
}

///
///
///
vec4 ColorMap_Pass(vec4 passColor, TexUnit tex, bool enableTev, TevUnit tev, vec2 texCoord)
{
	vec4 pass = MixTextureColor(tex, texCoord);

	//if(enableTev)
	//	pass = ApplyTev(pass, tev);

	if(tex.color_operation == 1 && pass.a != 0) // Alpha Mask
		passColor.rgb = pass.rgb;
		
	//TODO: I don't know what this is
	if(tex.color_operation == 2) // 8 RGB Mask 
	{
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
	
	if(tex.color_operation == 3) // Blend
		passColor.rgb = mix(passColor.rgb, pass.rgb, tex.blend);

	if(tex.color_operation == 4) // Modulate
		passColor.rgb *= pass.rgb;

	if(tex.color_operation == 5) // Replace
		passColor.rgb = pass.rgb;

	//if(tex.color_operation == 6) // Pass

	if(tex.color_operation == 7) // Add
		passColor.rgb += pass.rgb * pass.a;

	if(tex.color_operation == 8) // Subtract
		passColor.rgb -= pass.rgb * pass.a;
			
	
	
	//if(tex.alpha_operation == 1 && pass.a != 0) //Alpha Mask
	//	passColor.a = pass.a;
		
	if(tex.alpha_operation == 2) // Blend
		passColor.a = mix(passColor.a, pass.a, tex.blend);

	if(tex.alpha_operation == 3) // Modulate
		passColor.a *= pass.a;

	if(tex.alpha_operation == 4) // Replace
		passColor.a = pass.a;

	//if(tex.alpha_operation == 5) //Pass
		
	if(tex.alpha_operation == 6) //Add
		passColor.a += pass.a;

	if(tex.alpha_operation == 7) //Subtract
		passColor.a -= pass.a;
	

	return passColor;
}

///
/// Main mixing function
///
void main()
{
	if(colorOverride == 1)
	{
		fragColor = vec4(1, 1, 1, 1);
		return;
	}

	// color passes

	vec4 ambientPass = ambientColor;
	vec4 diffusePass = diffuseColor;
	vec4 specularPass = specularColor;

	if(hasTEX0 == 1)
	{
		if(TEX0.light_type == 0) ambientPass = ColorMap_Pass(ambientPass, TEX0, hasTEX0Tev == 1, TEX0Tev, texcoord0);
		if(TEX0.light_type == 1) diffusePass = ColorMap_Pass(diffusePass, TEX0, hasTEX0Tev == 1, TEX0Tev, texcoord0);
		if(TEX0.light_type == 2) specularPass = ColorMap_Pass(specularPass, TEX0, hasTEX0Tev == 1, TEX0Tev, texcoord0);
	}
	if(hasTEX1 == 1)
	{
		if(TEX1.light_type == 0) ambientPass = ColorMap_Pass(ambientPass, TEX1, hasTEX1Tev == 1, TEX1Tev, texcoord1);
		if(TEX1.light_type == 1) diffusePass = ColorMap_Pass(diffusePass, TEX1, hasTEX1Tev == 1, TEX1Tev, texcoord1);
		if(TEX1.light_type == 2) specularPass = ColorMap_Pass(specularPass, TEX1, hasTEX1Tev == 1, TEX1Tev, texcoord1);
	}
	

	// calculate material
	vec3 V = normalize(vertPosition - cameraPos);


	fragColor.rgb = diffusePass.rgb * GetDiffuseMaterial(normalize(normal), V).rgb
					+ specularPass.rgb * GetSpecularMaterial(normalize(normal), V).rgb;

	fragColor.rgb = clamp(fragColor.rgb, ambientPass.rgb * fragColor.rgb, vec3(1));


	// ext light map
	vec4 extColor = vec4(1, 1, 1, 1);
	if(hasTEX0 == 1 && TEX0.light_type == 4)
	{
		extColor = ColorMap_Pass(extColor, TEX0, hasTEX0Tev == 1, TEX0Tev, texcoord0);
		fragColor = ColorMap_Pass(fragColor, TEX0, hasTEX0Tev == 1, TEX0Tev, texcoord0);
	}
	if(hasTEX1 == 1 && TEX1.light_type == 4)
	{
		extColor = ColorMap_Pass(extColor, TEX1, hasTEX1Tev == 1, TEX1Tev, texcoord1);
		fragColor = ColorMap_Pass(fragColor, TEX1, hasTEX1Tev == 1, TEX1Tev, texcoord1);
	}


	// diffuse alpha
	fragColor.a = diffusePass.a;
	

	// material alpha
	fragColor.a *= alpha;


	// vertex color
	if(useVertexColor == 1)
		fragColor.rgb *= vertexColor.rgb * vertexColor.aaa;
		

	// gx overlay
	fragColor.xyz *= overlayColor;


	// debug render modes
	switch(renderOverride)
	{
	case 1: fragColor = vec4(vec3(0.5) + normal / 2, 1); break;
	case 2: fragColor = vertexColor; break;
	case 3: fragColor = vec4(texcoord0.x, 0, texcoord0.y, 1); break;
	case 4: fragColor = vec4(texcoord1.x, 0, texcoord1.y, 1); break;
	case 5: fragColor = ambientPass; break;
	case 6: fragColor = diffusePass; break;
	case 7: fragColor = specularPass; break;
	case 8: fragColor = extColor; break;
	case 9: fragColor = diffusePass * GetDiffuseMaterial(normalize(normal), V); break;
	case 10: fragColor = specularPass * GetSpecularMaterial(normalize(normal), V); break;
	case 11: 
		fragColor = vec4(0, 0, 0, 1);
		for(int i = 0; i < 4 ; i++)
			if(int(vbones[i]) == selectedBone)
				fragColor.r += vweights[i];
		fragColor.gb = fragColor.rr;
		break;
	}
}