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
	int flags;
	vec3 position;
	vec3 direction;
	vec3 color;
	
	int atten_enabled;
	float a0;
	float a1;
	float a2;
	float k0;
	float k1;
	float k2;
};
uniform Light light[MAX_LIGHT];


float Atten(Light light, vec3 vert)
{
	if (light.atten_enabled == 0)
		return 1.0;

	vec3 direction = light.direction;
	vec3 ldir = light.position - vert;
	float dist2 = dot(ldir, ldir);
	float dist = sqrt(dist2);
	ldir /= dist;

	float att = max(0.0, dot(ldir, direction));
	float att2 = att * att;
	
	float a = max(0.0, light.a2 * att2 + light.a1 * att + light.a0);
	float dnom = light.k2 * dist2 + light.k1 * dist + light.k0;

	return a / dnom;
}

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
			// check if ambient light
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

				float atten = Atten(light[i], vert);
			
				// calculate light color
				if (enableDiffuse == 1 && (light[i].flags & 0x1) != 0)
				{
					diff += vec3(max(0.0, dot(N, L))) * light[i].color * atten;
				}

				// calculate specularColor
				if (enableSpecular == 1 && (light[i].flags & 0x2) != 0)
				{
					if (dot(N, L) >= 0.0)
					{
						spec += vec3(pow(max(0.0, dot(reflect(-L, N), V)), shinniness)) * light[i].color * atten;
					}
				}
			}
		}
	}
}