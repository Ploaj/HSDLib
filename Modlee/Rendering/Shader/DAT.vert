#version 330

in vec3 in_pos;
in vec3 in_nrm;
in vec2 in_uv0;
in vec4 in_node;
in vec4 in_weight;

out vec3 FragPos;
out vec3 N;
out vec2 UV0;

uniform vec3 BONEPOS;

uniform float TWScale;
uniform float THScale;

uniform mat4 mvp;
uniform mat4 bones[100];

void main(){

vec4 Pos = vec4(in_pos, 1);
vec3 Nrm = in_nrm;

//Single Bind Fix
if(in_node.y == -1 && in_node.x != -1) 
{
	Pos = bones[int(in_node.x)] * Pos;
	Nrm = transpose(inverse(mat3(bones[int(in_node.x)]))) * Nrm;
}

Pos.xyz += BONEPOS;

UV0 = vec2(in_uv0.x*TWScale, in_uv0.y*THScale);
//UV0 = in_uv0;
N = Nrm;

FragPos = Pos.xyz;
gl_Position = mvp * Pos;

}