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
uniform int TEX0ColorOperation;
uniform int TEX0AlphaOperation;
uniform int TEX0CoordType;
uniform vec2 TEX0UVScale;
uniform int TEX0MirrorFix;
uniform mat4 TEX0Transform;

uniform int hasTEX1;
uniform sampler2D TEX1;
uniform int TEX1ColorOperation;
uniform int TEX1AlphaOperation;
uniform int TEX1CoordType;
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
uniform int useMaterialLighting;
uniform int useVertexColor;
uniform int enableDiffuse;
uniform int enableSpecular;


// Non rendering system

uniform vec3 cameraPos;
uniform int colorOverride;
uniform vec3 overlayColor;
uniform mat4 sphereMatrix;

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
		
	if(useMaterialLighting == 1 || enableDiffuse == 0)
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

	if(useMaterialLighting == 1 || enableSpecular == 0)
		return vec4(0, 0, 0, 1);
	
    float phong = pow(spec, shinniness);

    vec3 specularTerm = vec3(phong);

	return vec4(specularTerm, 1);
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

	vec4 diffusePass = diffuseColor;
	vec4 specularPass = specularColor;

	if(hasTEX0 == 1)
	{
		vec4 pass = MixTextureColor(TEX0, texcoord0, TEX0Transform, TEX0UVScale, TEX0CoordType, TEX0MirrorFix);

		if(TEX0ColorOperation == 0) // Blend
		{
			diffusePass.rgb = mix(diffusePass, pass, pass.a).rgb;
		}

		if(TEX0ColorOperation == 1 && pass.a > 0) // Replace
			diffusePass.rgb = pass.rgb;
			

		if(TEX0AlphaOperation == 0) // Blend
			diffusePass.a *= pass.a;

		if(TEX0AlphaOperation == 1) //Add
			diffusePass.a = pass.a;
	}
	if(hasTEX1 == 1)
	{
		vec4 pass = MixTextureColor(TEX1, texcoord1, TEX1Transform, TEX1UVScale, TEX1CoordType, TEX1MirrorFix);
		
		if(TEX1ColorOperation == 1 && pass.a > 0) // Replace
			diffusePass = pass;

		if(TEX1ColorOperation == 2) // Add
			diffusePass += pass;

		if(TEX1ColorOperation == 3) // Subtract
			diffusePass -= pass;
			

		if(TEX1AlphaOperation == 0) // Blend
			diffusePass.a *= pass.a;

		if(TEX1AlphaOperation == 1) //A dd
			diffusePass.a = pass.a;
	}

	fragColor = vec4(0, 0, 0, 1);

	fragColor.rgb += diffusePass.rgb * GetDiffuseMaterial(normalize(normal), V).rgb
					+ specularPass.rgb * GetSpecularMaterial(normalize(normal), V).rgb;

	fragColor.rgb = clamp(fragColor.rgb, ambientColor.rgb * fragColor.rgb, vec3(1));

	fragColor.a = diffusePass.a;

	if(dfNone == 0)
		fragColor.a *= alpha;

	fragColor.xyz *= overlayColor;
}