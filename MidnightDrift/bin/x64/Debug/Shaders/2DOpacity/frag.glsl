#version 450
//----------------------------------------
in vec4 outVertexPos;
in vec2 outTexCoord;
//----------------------------------------
layout(location = 0) out vec4 frag_color;
//----------------------------------------
uniform sampler2D AlbedoTexture;
uniform float Opacity;
uniform vec4 Tint;
//----------------------------------------

void main() {
	frag_color = texture(AlbedoTexture, outTexCoord);
	frag_color *= Tint;
	frag_color.w = Opacity;
}