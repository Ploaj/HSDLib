#version 330

in vec4 color;
in vec2 texcoord;

out vec4 fragColor;

uniform int use_texture;
uniform sampler2D sampler0;

uniform int enablePrimEnv;
uniform vec4 primColor;
uniform vec4 envColor;

bool alpha_test(float alpha);

void main()
{
	// use texture
	if (use_texture == 1)
		fragColor = texture(sampler0, texcoord);
	else
		fragColor = vec4(1);
	
	// envionment mix
	if (enablePrimEnv == 1)
		fragColor = mix(envColor, primColor, fragColor);
	else
		fragColor = fragColor * primColor;

	// multiplier color
	fragColor *= color;

	// alpha test
	if (alpha_test(fragColor.a))
		discard;

	return;
}