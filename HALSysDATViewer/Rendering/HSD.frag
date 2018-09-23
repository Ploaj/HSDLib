#version 330

in vec2 UV0;

uniform sampler2D TEX0;

out vec4 color;

void main(){

	color = texture2D(TEX0, UV0);

}