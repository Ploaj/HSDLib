#version 330

#define MAX_TEX 4
#define MAX_WEIGHTS 6

#define PASS_AMBIENT 1
#define PASS_DIFFUSE 2
#define PASS_SPECULAR 3
#define PASS_EXT 4
#define PASS_TOON 5

in vec3 vertPosition;
in vec3 normal;
in vec3 ntan;
in vec3 bitan;

in vec3 ambientLight;
in vec3 diffuseLight;
in vec3 specularLight;

in vec2 texcoord[MAX_TEX];
in vec4 vertexColor;
flat in int vbones[MAX_WEIGHTS];
in float vweights[MAX_WEIGHTS];
in float fogAmt;

out vec4 fragColor;

// material
uniform vec4 ambientColor;
uniform vec4 diffuseColor;
uniform vec4 specularColor;
uniform float shinniness;
uniform float alpha;




// Non rendering system

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
uniform int perPixelLighting;
void CalculateDiffuseShading(vec3 vert, vec3 N, inout vec3 amb, inout vec3 diff, inout vec3 spec);

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

	// get light values
	vec3 finalAmbLight = ambientLight;
	vec3 finalDiffLight = diffuseLight;
	vec3 finalSpecLight = specularLight;
	if (perPixelLighting == 1)
	{
		CalculateDiffuseShading(vertPosition, normalize(normal), finalAmbLight, finalDiffLight, finalSpecLight);
	}

	// get toon shading
//	if (useToonShading == 1)
//	{
//		diffMatColor = GetToonTexture().rgb;
//		specularLamb = 0;
//	}

	// color passes
	vec4 ambientPass = TexturePass(ambientColor, PASS_AMBIENT);
	vec4 diffusePass = TexturePass(vec4(diffuseColor.rgb, alpha * diffuseColor.a), PASS_DIFFUSE);
	vec4 specularPass = TexturePass(specularColor, PASS_SPECULAR);

	// calculate fragment color
	fragColor.rgb = ambientPass.rgb	* diffusePass.rgb * finalAmbLight +
					diffusePass.rgb * finalDiffLight +
					specularPass.rgb * finalSpecLight;
	

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


	// apply bump emboss map
	fragColor *= GetBumpShading(normalize(cameraPos - vertPosition));


	// gx overlay
	fragColor.xyz *= overlayColor;


	// apply fog
	if(fogAmt != 0)
		fragColor.rgb = mix(fragColor.rgb, fog.color.rgb, fogAmt);


	// debug render modes
	switch(renderOverride)
	{
	case 1: fragColor = vec4(vec3(0.5) + normal / 2, 1); break;
	case 2: fragColor = vec4(normalize(ntan), 1); break;
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
	case 14: fragColor = ambientPass * diffusePass; break;
	case 15: fragColor = diffusePass; break;
	case 16: fragColor = specularPass; break;
	case 17: fragColor = TexturePass(vec4(1), PASS_EXT); break;
	case 18: fragColor = vec4(finalAmbLight, 1); break;
	case 19: fragColor = vec4(finalDiffLight, 1); break;
	case 20: fragColor = vec4(finalSpecLight, 1); break;
	case 21: 
		fragColor = vec4(0, 0, 0, 1);
		for(int i = 0; i < MAX_WEIGHTS ; i++)
			if(vweights[i] > 0)
			{
				if (vbones[i] == selectedBone)
					fragColor.r += vweights[i];
				else
					fragColor.b += vweights[i];
			}
		if (fragColor.r <= 0)
			fragColor = vec4(0, 0, 0, 1);
		break;
	case 22: fragColor = vec4(vec3(fogAmt), 1); break;
	}


	// alpha test
	if (renderOverride == 0 && alpha_test(fragColor.a))
		discard;

	// adjust saturation if needed
	if(saturate != 1)
		fragColor.rgb = saturation(fragColor.rgb);
}