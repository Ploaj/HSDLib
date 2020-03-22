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
//uniform int useMaterialLighting;
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
    vec4 clr = vec4(1);

    vec2 coords = GetCoordType(coordType, texCoord);

	coords = (uvTransform * vec4(coords.x, coords.y, 0, 1)).xy;
	
	coords *= uvscale;

	if(mirrorFix == 1) // GX OPENGL difference
		coords.y += 1;

    clr = texture(tex, coords);

    return clr;
}

///
/// Gets the diffuse material
///
vec4 GetDiffuseMaterial(vec3 V, vec3 N)
{
	if(useVertexColor == 1)
		return vertexColor;
		
	if(enableDiffuse == 0)
		return vec4(1, 1, 1, 1);

    float lambert = clamp(dot(N, V), 0, 1);
	vec3 clr = vec3(lambert);
	return vec4(clr, 1);
}

///
/// Gets the specular material
///
vec4 GetSpecularMaterial(vec3 V, vec3 N)
{
	if(useVertexColor == 1)
		return vec4(0, 0, 0, 1);

	if(enableSpecular == 0)
		return vec4(0, 0, 0, 1);
	
    float phong = pow(spec, shinniness);

    vec3 specularTerm = vec3(phong);

	return vec4(specularTerm, 1);
}

///
///
///
vec4 ColorMap_Pass(vec4 passColor, int operation, int alphaOperation, sampler2D tex, vec2 texCoord, mat4 uvTransform, vec2 uvscale, int coordType, int mirrorFix, float blend)
{
	vec4 pass = MixTextureColor(tex, texCoord, uvTransform, uvscale, coordType, mirrorFix);

	if(operation == 0) // Modulate
		passColor.rgb *= pass.rgb;

	if(operation == 1) // Replace
		passColor.rgb = pass.rgb;

	if(operation == 2) // Blend
		passColor.rgb = mix(passColor, pass, blend).rgb;

	if(operation == 3) // Add
		passColor.rgb += pass.rgb;

	if(operation == 4) // Subtract
		passColor.rgb -= pass.rgb;

	//if(operation == 5) // Pass
			
	// 6 Alpha Mask

	// 7 RGB Mask

	
	if(alphaOperation == 0) // Modulate
		passColor.a *= pass.a;
		
	if(alphaOperation == 1) // Replace
		passColor.a = pass.a;

	if(alphaOperation == 2) //Add
		passColor.a += pass.a;

	if(alphaOperation == 3) //Subtract
		passColor.a -= pass.a;

	//if(alphaOperation == 4) //Pass
	
	// 5 Alpha Mask

	// 6 RGB Mask

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
	vec4 extPass = vec4(1, 1, 1, 1);

	if(hasTEX0 == 1)
	{
		vec4 col = vec4(0);
		if(TEX0LightType == 0) col = ambientPass;
		if(TEX0LightType == 1) col = diffusePass;
		if(TEX0LightType == 2) col = specularPass;
		if(TEX0LightType == 3) col = extPass;

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
		if(TEX0LightType == 3) extPass = col;
	}
	if(hasTEX1 == 1)
	{
		vec4 col = vec4(0);
		if(TEX1LightType == 0) col = ambientPass;
		if(TEX1LightType == 1) col = diffusePass;
		if(TEX1LightType == 2) col = specularPass;
		if(TEX1LightType == 3) col =  extPass;

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
		if(TEX1LightType == 3) extPass = col;
	}

	fragColor.rgb = diffusePass.rgb * GetDiffuseMaterial(normalize(normal), V).rgb
					+ specularPass.rgb * GetSpecularMaterial(normalize(normal), V).rgb;

	fragColor.rgb *= extPass.rgb;

	fragColor.rgb = clamp(fragColor.rgb, ambientPass.rgb * fragColor.rgb, vec3(1));

	fragColor.a = diffusePass.a;

	if(dfNone == 0)
		fragColor.a *= alpha;
		
	fragColor.xyz *= overlayColor;

	switch(renderOverride)
	{
	case 1: fragColor = vec4(vec3(0.5) + normal / 2, 1); break;
	case 2: fragColor = vertexColor; break;
	case 3: fragColor = vec4(texcoord0, 0, 1); break;
	case 4: fragColor = vec4(texcoord1, 0, 1); break;
	case 5: fragColor = ambientPass; break;
	case 6: fragColor = diffusePass; break;
	case 7: fragColor = specularPass; break;
	case 8: fragColor = extPass; break;
	}

}