#version 330

// settings
uniform int enableDiffuse;
uniform int enableSpecular;
uniform int perPixelLighting;

// material
uniform vec4 ambientColor;
uniform vec4 diffuseColor;
uniform vec4 specularColor;
uniform float shinniness;
uniform float alpha;

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
vec4 GetSpecularMaterial(vec3 V, vec3 N, float specular)
{
	if(enableSpecular == 0)
		return vec4(0, 0, 0, 1);

	float spc = specular;
	
	if(perPixelLighting == 1)
		spc = clamp(dot(N, V), 0, 1);
	
    float phong = pow(spc, shinniness);

	return vec4(vec3(phong), 1);
}
