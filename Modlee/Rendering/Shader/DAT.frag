#version 330

in vec3 FragPos;
in vec3 N;
in vec2 UV0;

uniform vec3 DIF;
uniform vec3 AMB;
uniform vec3 SPC;
uniform sampler2D TEX_DIF;

uniform int HasTexture;

out vec4 color;

const vec3 lightPos = vec3(10, 30, 40);

void main(){

vec3 lightDir = normalize(lightPos - FragPos);

float lambertian = max(dot(lightDir, N), 0.0);
float specular = 0.0;

vec3 reflectDir = reflect(-lightDir, N);
vec3 viewDir = normalize(-FragPos);

float specAngle = max(dot(reflectDir, viewDir), 0.0);
specular = pow(specAngle, 4.0);

vec3 DIFCOL = vec3(1);
if(HasTexture == 1)
	DIFCOL = texture2D(TEX_DIF, UV0).rgb;
vec3 Final = DIFCOL;//AMB*0.2 * DIFCOL + DIF * lambertian * DIFCOL + SPC * specular;// + lambertian * DIFCOL + SPC * specular;

color = vec4(Final, 1);
}