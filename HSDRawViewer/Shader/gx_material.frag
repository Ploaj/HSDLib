#version 330

#define MAX_LIGHT 4

#define LIGHT_AMBIENT 0
#define LIGHT_INFINITE 1
#define LIGHT_POINT 2
#define LIGHT_SPOT 3

// settings
uniform int enableDiffuse;
uniform int enableSpecular;

// material
uniform vec4 ambientColor;
uniform vec4 diffuseColor;
uniform vec4 specularColor;
uniform float shinniness;
uniform float alpha;

// camera input
uniform vec3 cameraPos;

struct Light
{
	int enabled;
	int type;
	vec3 position;

	// common
	vec3 color;
};
uniform Light light[MAX_LIGHT];

///
///
///
void CalculateDiffuseShading(vec3 vert, vec3 N, inout vec3 amb, inout vec3 diff, inout vec3 spec)
{
	// calcualte view vector
	vec3 V = normalize(cameraPos - vert);

	// initialize colors
	amb = vec3(0);
	diff = vec3(0);
	spec = vec3(0);
	
	// process lights
	for (int i = 0; i < MAX_LIGHT; i++)
	{
		// check if light is enabled and not ambient
		if (light[i].enabled == 1)
		{
			if (light[i].type == LIGHT_AMBIENT)
			{
				amb += light[i].color;
			}
			else
			{
				// check light direction
				vec3 L;
				if (light[i].type == LIGHT_INFINITE)
					L = normalize(light[i].position);
				else
					L = normalize(light[i].position - vert);
			
				// calculate light color
				if (enableDiffuse == 1)
				{
					diff += vec3(clamp(dot(N, L), 0, 1)) * light[i].color;
				}

				// calculate specularColor
				if (enableSpecular == 1)
				{
					if (dot(N, L) >= 0.0)
					{
						spec += vec3(pow(max(0.0, dot(reflect(-L, N), V)), shinniness)) * light[i].color;
					}
				}
			
				// point light attenuation
//				if (light[i].type == LIGHT_POINT)
//				{
//					float dis = length(light[0].position - vert);
//
//					float attenuation = 1.0 / (light[0].constant + light[0].linear * dis + 
//  								 light[0].quadratic * (dis * dis)); 
//				 
//					diff  *= attenuation;
//				}
			}
		}
	}
}