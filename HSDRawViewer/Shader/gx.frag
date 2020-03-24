#version 330

in vec3 vertPosition;
in vec3 normal;
in float spec;
in vec2 texcoord0;
in vec2 texcoord1;
in vec4 vertexColor;

out vec4 fragColor;


// textures
uniform int hasTEX0;
uniform sampler2D TEX0;
uniform int TEX0LightType;
uniform int TEX0ColorOperation;
uniform int TEX0AlphaOperation;
uniform int TEX0CoordType;
uniform float TEX0Blend;
uniform vec2 TEX0UVScale;
uniform int TEX0MirrorFix;
uniform mat4 TEX0Transform;

uniform int hasTEX1;
uniform sampler2D TEX1;
uniform int TEX1LightType;
uniform int TEX1ColorOperation;
uniform int TEX1AlphaOperation;
uniform int TEX1CoordType;
uniform float TEX1Blend;
uniform vec2 TEX1UVScale;
uniform int TEX1MirrorFix;
uniform mat4 TEX1Transform;


// material
uniform vec4 ambientColor;
uniform vec4 diffuseColor;
uniform vec4 specularColor;
uniform float shinniness;
uniform float alpha;


// flags
uniform int dfNone;
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
	//COORD_REFLECTION
	if(coordType == 1) 
		return GetSphereCoords(normal);

	//COORD_UV
	return tex0;
}

///
/// Gets the texture color after applying all necessary transforms
///
vec4 MixTextureColor(sampler2D tex, vec2 texCoord, mat4 uvTransform, vec2 uvscale, int coordType, int mirrorFix)
{
    vec2 coords = GetCoordType(coordType, texCoord);

	coords = (uvTransform * vec4(coords.x, coords.y, 0, 1)).xy;
	
	coords *= uvscale;

	if(mirrorFix == 1) // GX OPENGL difference
		coords.y += 1;

    return texture(tex, coords);
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
vec4 ColorMap_Pass(vec4 passColor, int operation, int alphaOperation, sampler2D tex, vec2 texCoord, mat4 uvTransform, vec2 uvscale, int coordType, int mirrorFix, float blend)
{
	vec4 pass = MixTextureColor(tex, texCoord, uvTransform, uvscale, coordType, mirrorFix);

	if(operation == 1) // Modulate
		passColor.rgb *= pass.rgb;

	if(operation == 2) // Replace
		passColor.rgb = pass.rgb;

	if(operation == 3) // Blend
		passColor.rgb = mix(passColor, pass, blend).rgb;

	if(operation == 4) // Add
		passColor.rgb += pass.rgb;

	if(operation == 5) // Subtract
		passColor.rgb -= pass.rgb;

	//if(operation == 6) // Pass
			
	if(operation == 7 && pass.a != 0) // Alpha Mask
		passColor.rgb = pass.rgb;

	// 8 RGB Mask

	
	if(alphaOperation == 1) // Modulate
		passColor.a *= pass.a;
		
	if(alphaOperation == 2) // Replace
		passColor.a = pass.a;
		
	if(alphaOperation == 3) // Blend
		passColor.a = mix(passColor.a, pass.a, blend);

	if(alphaOperation == 4) //Add
		passColor.a += pass.a;

	if(alphaOperation == 5) //Subtract
		passColor.a -= pass.a;

	//if(alphaOperation == 6) //Pass
	
	// 7 Alpha Mask

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

	// get vectors
	vec3 V = normalize(vertPosition - cameraPos);

	vec4 ambientPass = ambientColor;
	vec4 diffusePass = diffuseColor;
	vec4 specularPass = specularColor;

	if(hasTEX0 == 1)
	{
		vec4 col = vec4(0);
		if(TEX0LightType == 0) col = ambientPass;
		if(TEX0LightType == 1) col = diffusePass;
		if(TEX0LightType == 2) col = specularPass;

		col = ColorMap_Pass(
		col, 
		TEX0ColorOperation, 
		TEX0AlphaOperation, 
		TEX0,
		texcoord0, 
		TEX0Transform, 
		TEX0UVScale, 
		TEX0CoordType, 
		TEX0MirrorFix,
		TEX0Blend);

		if(TEX0LightType == 0) ambientPass = col;
		if(TEX0LightType == 1) diffusePass = col;
		if(TEX0LightType == 2) specularPass = col;
	}
	if(hasTEX1 == 1)
	{
		vec4 col = vec4(0);
		if(TEX1LightType == 0) col = ambientPass;
		if(TEX1LightType == 1) col = diffusePass;
		if(TEX1LightType == 2) col = specularPass;

		col = ColorMap_Pass(
		col, 
		TEX1ColorOperation, 
		TEX1AlphaOperation, 
		TEX1,
		texcoord1, 
		TEX1Transform, 
		TEX1UVScale, 
		TEX1CoordType, 
		TEX1MirrorFix,
		TEX1Blend);
		
		if(TEX1LightType == 0) ambientPass = col;
		if(TEX1LightType == 1) diffusePass = col;
		if(TEX1LightType == 2) specularPass = col;
	}

	fragColor.rgb = diffusePass.rgb * GetDiffuseMaterial(normalize(normal), V).rgb
					+ specularPass.rgb * GetSpecularMaterial(normalize(normal), V).rgb;

	fragColor.rgb = clamp(fragColor.rgb, ambientPass.rgb * fragColor.rgb, vec3(1));

	vec4 extColor = vec4(1, 1, 1, 1);
	if(hasTEX0 == 1 && TEX0LightType == 3)
	{
		extColor = ColorMap_Pass(
		extColor, 
		TEX0ColorOperation, 
		TEX0AlphaOperation, 
		TEX0,
		texcoord0, 
		TEX0Transform, 
		TEX0UVScale, 
		TEX0CoordType, 
		TEX0MirrorFix,
		TEX0Blend);

		fragColor = ColorMap_Pass(
		fragColor, 
		TEX0ColorOperation, 
		TEX0AlphaOperation, 
		TEX0,
		texcoord0, 
		TEX0Transform, 
		TEX0UVScale, 
		TEX0CoordType, 
		TEX0MirrorFix,
		TEX0Blend);
	}
	if(hasTEX1 == 1 && TEX1LightType == 3)
	{
		extColor = ColorMap_Pass(
		extColor, 
		TEX1ColorOperation, 
		TEX1AlphaOperation, 
		TEX1,
		texcoord1, 
		TEX1Transform, 
		TEX1UVScale, 
		TEX1CoordType, 
		TEX1MirrorFix,
		TEX1Blend);

		fragColor = ColorMap_Pass(
		fragColor, 
		TEX1ColorOperation, 
		TEX1AlphaOperation, 
		TEX1,
		texcoord1, 
		TEX1Transform, 
		TEX1UVScale, 
		TEX1CoordType, 
		TEX1MirrorFix,
		TEX1Blend);
	}

	fragColor.a = diffusePass.a;

	if(useVertexColor == 1)
		fragColor *= vertexColor;

	fragColor.a *= alpha;
		
	fragColor.xyz *= overlayColor;

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
	}
}