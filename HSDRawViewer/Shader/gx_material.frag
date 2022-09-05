#version 330

// settings
uniform int enableDiffuse;
uniform int enableSpecular;

// material
uniform vec4 ambientColor;
uniform vec4 diffuseColor;
uniform vec4 specularColor;
uniform float shinniness;
uniform float alpha;

///
/// Gets the diffuse material
///
float GetDiffuseMaterial(vec3 N, vec3 L)
{
	if(enableDiffuse == 0)
		return 1;

	return clamp(dot(N, L), 0, 1);
}

///
/// Gets the specular material
///
float GetSpecularMaterial(vec3 N, vec3 V, vec3 L)
{
	if(enableSpecular == 0)
		return 0;

	if (dot(N, L) < 0.0)
	{ 
		// no specular reflection
		return 0.0;
	}
	else 
	{
		// light source on the right side
		return pow(max(0.0, dot(reflect(-L, N), V)), shinniness);
	}

}
