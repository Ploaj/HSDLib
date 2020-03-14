#version 330

in vec3 vertPosition;
in vec3 normal;
in vec2 texcoord0;
in vec4 vertexColor;

out vec4 fragColor;


// textures
uniform int enableTexDiffuse;
uniform sampler2D diffuseTex;
uniform int difColorType;
uniform int difAlphaType;
uniform int diffuseCoordType;
uniform vec2 diffuseUVScale;
uniform int diffuseMirrorFix;
uniform mat4 difTransform;


// material
uniform vec4 ambientColor;
uniform vec4 diffuseColor;
uniform vec4 specularColor;
uniform float shinniness;
uniform float alpha;


// flags
uniform int dfNone;
uniform int useMaterialLighting;
uniform int enableDiffuse;
uniform int enableSpecular;
uniform int useVertexColor;


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
	if(coordType == 1) //COORD_REFLECTION
		return GetSphereCoords(normal);

	//COORD_UV
	return tex0;
}

///
///
///
vec4 MixTextureColor(vec4 materialColor, vec2 texCoord, mat4 uvTransform, vec2 uvscale, int coordType, int mirrorFix, sampler2D tex, int colorMixType, int alphaMixType)
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
/// basic lambert diffuse
///
vec4 DiffusePass(vec3 N, vec3 V)
{
	vec4 diffuseTerm = vec4(1);

    float lambert = clamp(dot(N, V), 0, 1);
	if(useVertexColor == 1 || useMaterialLighting == 1)
		lambert = 1;
	
	vec4 colorPass = vec4(1);

    if (enableTexDiffuse == 1)
	{
		colorPass = MixTextureColor(diffuseTerm, texcoord0, difTransform, diffuseUVScale, diffuseCoordType, diffuseMirrorFix, diffuseTex, difColorType, difAlphaType);

		diffuseTerm.rgb = colorPass.rgb;

		diffuseTerm.a = colorPass.a;
	}
	
	if (useVertexColor == 0)
	{
		diffuseTerm.rgb = clamp(diffuseTerm.rgb, ambientColor.rgb * colorPass.rgb, vec3(1));
	
		diffuseTerm.rgb *= diffuseColor.rgb;

	}
	diffuseTerm.rgb *= lambert;

	return diffuseTerm;
}

///
/// This is usally a reflection map
///
/*vec3 ColorMapExtPass(vec3 N, vec3 V)
{
    vec4 Map = vec4(0);

    vec2 Coords = GetCoordType(extCoordType, tex0);

    if (hasExt== 1)
        Map = texture(extTex, Coords * extScale);

    return Map.rgb;
}*/

///
/// Main mixing function
///
void main()
{
	// get vectors
	vec3 V = normalize(vertPosition - cameraPos);
    vec3 N = normalize(normal);

	fragColor = vec4(0, 0, 0, 1);

	vec4 diffusePass = DiffusePass(N, V);

	fragColor.rgb += diffusePass.rgb;// * enableDiffuse;

	fragColor.a = diffusePass.a;

	if(dfNone == 0)
		fragColor.a *= alpha;

	if(useVertexColor == 1)
		fragColor *= vertexColor;

	fragColor.xyz *= overlayColor;

	if(colorOverride == 1)
		fragColor = vec4(1, 1, 1, 1);
}