#version 330

#define COORD_UV 0
#define COORD_REFLECTION 1
#define COORD_HIGHLIGHT 2
#define COORD_SHADOW 3
#define COORD_TOON 4
#define COORD_GRADATION 5

uniform mat4 sphereMatrix;

///
/// Gets spherical UV coords, this is used for reflection effects
///
vec2 GetSphereCoords(vec3 N)
{
    vec3 viewNormal = mat3(sphereMatrix) * N;

	vec2 cord = viewNormal.xy * 0.5 + 0.5;

	cord.y = 1 - cord.y;

    return cord;
}

///
/// Returns Coords for specified coord type
///
vec2 GetCoordType(int coordType, vec2 tex0, vec3 N)
{
	//COORD_HIGHLIGHT
    //COORD_SHADOW
    //COORD_TOON
    //COORD_GRADATION

	switch(coordType)
	{
		case COORD_REFLECTION:
		return GetSphereCoords(N);
			break;
		case COORD_UV:
		default:
			return tex0;
			break;
	}

	return tex0;
}