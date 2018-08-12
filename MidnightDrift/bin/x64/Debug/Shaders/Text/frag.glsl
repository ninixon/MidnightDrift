#version 450
//----------------------------------------
in vec4 outVertexPos;
in vec2 outTexCoord;
//----------------------------------------
layout(location = 0) out vec4 frag_color;
//----------------------------------------
uniform sampler2D AlbedoTexture;
uniform vec2 fontCharPosition;
uniform vec2 fontCharSize;
uniform vec2 fontTexSize;
//----------------------------------------

void main() {
	vec2 modifiedTexCoord = outTexCoord;
	modifiedTexCoord.x += (modifiedTexCoord.x + fontCharSize.x) / fontTexSize.x;
	modifiedTexCoord.y += (modifiedTexCoord.y + fontCharSize.y) / fontTexSize.y;
	frag_color = texture(AlbedoTexture, modifiedTexCoord);
}