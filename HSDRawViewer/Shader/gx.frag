#version 330

in vec3 vertPosition;
in vec3 normal;
in vec2 texcoord0;

out vec4 fragColor;

uniform sampler2D tex0;

uniform int enableTEX0;
uniform int diffuseCoordType;
uniform vec2 diffuseUVScale;


uniform vec3 cameraPos;
uniform int colorOverride;
uniform vec3 overlayColor;


uniform vec4 ambientColor;
uniform vec4 diffuseColor;
uniform vec4 specularColor;
uniform float shinniness;
uniform float alpha;


uniform int enableMaterial;
uniform int enableDiffuse;
uniform int enableSpecular;


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
/// color map pass for the diffuse texture
///
vec4 ColorMapDiffusePass(vec3 N, vec3 V)
{
    vec4 diffuseMap = vec4(1);

    vec2 diffuseCoords = GetCoordType(diffuseCoordType, texcoord0);

    if (enableDiffuse == 1)
        diffuseMap = texture(tex0, diffuseCoords * diffuseUVScale);

    return diffuseMap;
}

///
/// basic lambert diffuse
///
vec3 DiffusePass(vec3 N, vec3 V)
{
	vec3 colorPass = ColorMapDiffusePass(N, V).rgb;

    float lambert = clamp(dot(N, V), 0, 1);
	
    vec3 diffuseTerm = colorPass * lambert;
	
	diffuseTerm = clamp(diffuseTerm, ambientColor.rgb * colorPass, vec3(1));
	
    if (enableDiffuse == 0)
		diffuseTerm *= diffuseColor.rgb;

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


	fragColor.rgb += DiffusePass(N, V) * enableDiffuse;


	fragColor.a *= alpha;


	fragColor.xyz *= overlayColor;


	if(colorOverride == 1)
		fragColor = vec4(1, 1, 1, 1);
}