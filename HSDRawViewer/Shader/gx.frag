#version 330

#define MAX_TEX 4

#define PASS_AMBIENT 1
#define PASS_DIFFUSE 2
#define PASS_SPECULAR 3
#define PASS_EXT 4
#define PASS_TOON 5

in vec3 vertPosition;
in vec3 normal;
in vec3 tan;
in vec3 bitan;
in float spec;
in vec2 texcoord[MAX_TEX];
in vec4 vertexColor;
flat in vec4 vbones;
in vec4 vweights;
in float fogAmt;

out vec4 fragColor;

// material
uniform vec4 ambientColor;
uniform vec4 diffuseColor;
uniform vec4 specularColor;
uniform float shinniness;
uniform float alpha;




// Non rendering system

uniform struct Light
{
	int useCamera;
	vec3 position;
	vec4 ambient;
	vec4 diffuse;
	float ambientPower;
	float diffusePower;
} light;

uniform struct Fog
{
	int type;
	float start;
	float end;
	vec4 color;
} fog;

// camera
uniform vec3 cameraPos;

// flags
uniform int useToonShading;
uniform int useVertexColor;
uniform int renderOverride;
uniform int selectedBone;

uniform int colorOverride;
uniform vec3 overlayColor;
uniform float saturate;


// pixel processing
bool alpha_test(float alpha);

// textures
vec4 GetBumpShading(vec3 V);
vec4 GetTextureFragment(int index);
vec4 TexturePass(vec4 color, int pass_type);
vec4 GetToonTexture();

// material
vec4 GetDiffuseMaterial(vec3 V, vec3 N);
vec4 GetSpecularMaterial(vec3 V, vec3 N, float specular);

///
/// Algorithm from Chapter 16 of OpenGL Shading Language
///
vec3 saturation(vec3 rgb)
{
    const vec3 W = vec3(0.2125, 0.7154, 0.0721);
    vec3 intensity = vec3(dot(rgb, W));
    return mix(intensity, rgb, saturate);
}


///
/// Main mixing function
///
void main()
{
	if(colorOverride == 1)
	{
		fragColor = vec4(1, 1, 1, 1);
		return;
	}

	// color passes
	vec4 ambientPass = TexturePass(ambientColor, PASS_AMBIENT);
	vec4 diffusePass = TexturePass(vec4(diffuseColor.rgb, alpha * diffuseColor.a), PASS_DIFFUSE);
	vec4 specularPass = TexturePass(specularColor, PASS_SPECULAR);

	// calculate material
	vec3 V = vertPosition - cameraPos;

	if(light.useCamera == 0)
		V = light.position;

	V = normalize(V);
	vec3 N = normalize(normal);

	// get light values
	vec4 diffuseMaterial = GetDiffuseMaterial(N, V);
	vec4 specularMaterial = GetSpecularMaterial(N, V, spec);

	if (useToonShading == 1)
	{
		diffuseMaterial = GetToonTexture();
		specularMaterial = vec4(0);
	}

	// calculate fragment color
	fragColor.rgb =  ambientPass.rgb * diffusePass.rgb * light.ambient.rgb * vec3(light.ambientPower)
					+ diffusePass.rgb * diffuseMaterial.rgb * light.diffuse.rgb * vec3(light.diffusePower)
					+ specularPass.rgb * specularMaterial.rgb;

	fragColor.rgb = clamp(fragColor.rgb, ambientPass.rgb * fragColor.rgb, vec3(1));
	

	// ext light map
	fragColor = TexturePass(fragColor, PASS_EXT);


	// diffuse alpha
	fragColor.a = diffusePass.a;


	// vertex color
	if(useVertexColor == 1)
	{
		fragColor.rgb *= vertexColor.rgb * vertexColor.aaa;
		fragColor.a *= vertexColor.a;
	}
	else
	{
		// material alpha can only be used without vertex color
		//fragColor.a *= alpha;
	}


	// apply bump emboss map
	fragColor *= GetBumpShading(V);


	// gx overlay
	fragColor.xyz *= overlayColor;


	// apply fog
	if(fogAmt != 0)
		fragColor.rgb = mix(fragColor.rgb, fog.color.rgb, fogAmt);


	// debug render modes
	switch(renderOverride)
	{
	case 1: fragColor = vec4(vec3(0.5) + N / 2, 1); break;
	case 2: fragColor = vec4(normalize(tan), 1); break;
	case 3: fragColor = vec4(normalize(bitan), 1); break;
	case 4: fragColor = vertexColor; break;
	case 5: fragColor = vec4(fragColor.aaa, 1); break;
	case 6: fragColor = vec4(texcoord[0].x, 0, texcoord[0].y, 1); break;
	case 7: fragColor = vec4(texcoord[1].x, 0, texcoord[1].y, 1); break;
	case 8: fragColor = vec4(texcoord[2].x, 0, texcoord[2].y, 1); break;
	case 9: fragColor = vec4(texcoord[3].x, 0, texcoord[3].y, 1); break;
	case 10: fragColor = GetTextureFragment(0); break;
	case 11: fragColor = GetTextureFragment(1); break;
	case 12: fragColor = GetTextureFragment(2); break;
	case 13: fragColor = GetTextureFragment(3); break;
	case 14: fragColor = ambientPass; break;
	case 15: fragColor = diffusePass; break;
	case 16: fragColor = specularPass; break;
	case 17: fragColor = TexturePass(vec4(1), PASS_EXT); break;
	case 18: fragColor = diffusePass * diffuseMaterial; break;
	case 19: fragColor = specularPass * specularMaterial; break;
	case 20: 
		fragColor = vec4(0, 0, 0, 1);
		for(int i = 0; i < 4 ; i++)
			if(vweights[i] > 0)
			{
				if (int(vbones[i]) == selectedBone)
					fragColor.r += vweights[i];
				else
					fragColor.b += vweights[i];
			}
		if (fragColor.r <= 0)
			fragColor = vec4(0, 0, 0, 1);
		break;
	case 21: fragColor = vec4(vec3(fogAmt), 1); break;
	}


	// alpha test
	if (renderOverride == 0 && alpha_test(fragColor.a))
		discard;


	// adjust saturation if needed
	if(saturate != 1)
		fragColor.rgb = saturation(fragColor.rgb);
}