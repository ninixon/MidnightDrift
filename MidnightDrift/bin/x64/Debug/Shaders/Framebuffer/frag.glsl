#version 450
//----------------------------------------
layout(origin_upper_left) in vec4 gl_FragCoord;
//----------------------------------------
flat in vec4 outVertexPos;
in vec2 outTexCoord;
//----------------------------------------
out vec4 frag_color;
//----------------------------------------
uniform sampler2D renderedTexture;
uniform sampler2D depthTexture;
uniform sampler2D blurTexture;
uniform float fogStrength;
//----------------------------------------

void main() {
	frag_color = vec4(texture(renderedTexture, outTexCoord.xy));
}

