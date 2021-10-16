#version 330

#define COORD_UV 0
#define COORD_REFLECTION 1
#define COORD_HIGHLIGHT 2
#define COORD_SHADOW 3
#define COORD_TOON 4
#define COORD_GRADATION 5

in vec3 normal;
in vec3 vertPosition;

uniform mat4 sphereMatrix;
uniform vec3 cameraPos;

uniform struct Light
{
	int useCamera;
	vec3 position;
	vec4 ambient;
	vec4 diffuse;
	float ambientPower;
	float diffusePower;
} light;

///
/// Gets spherical UV coords, this is used for reflection effects
///
vec2 GetSphereCoords()
{
    vec3 viewNormal = mat3(sphereMatrix) * normal;

	vec2 cord = viewNormal.xy * 0.5 + 0.5;

	cord.y = 1 - cord.y;

    return cord;
}

///
/// Gets TOON coords
///
vec2 GetToonCoords()
{
	vec3 V = vertPosition - cameraPos;

	if(light.useCamera == 0)
		V = light.position;

	V = normalize(V);

    float lambert = clamp(dot(normal, V) + 0.4, 0, 1);

    return vec2(lambert, lambert);
}

///
/// Returns Coords for specified coord type
///
vec2 GetCoordType(int coordType, vec2 tex0)
{
	//COORD_HIGHLIGHT
    //COORD_SHADOW
    //COORD_GRADATION

	switch(coordType)
	{
		case COORD_REFLECTION:
			return GetSphereCoords();
			break;
		case COORD_TOON:
			return GetToonCoords();
			break;
		case COORD_UV:
		default:
			return tex0;
			break;
	}

	return tex0;
}