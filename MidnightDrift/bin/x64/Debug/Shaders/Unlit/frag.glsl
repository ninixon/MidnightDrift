#version 450
//----------------------------------------
layout(origin_upper_left) in vec4 gl_FragCoord;
//----------------------------------------
flat in vec4 outVertexPos;
in vec3 outNormal;
in vec3 outFragPos;
in vec2 outTexCoord;
in vec4 debugCol;
//----------------------------------------
layout(location = 0) out vec4 frag_color;
//----------------------------------------
uniform sampler2D AlbedoTexture;
uniform sampler2D ReflectionTexture;
uniform mat4 Model;
uniform mat4 View;
uniform mat4 Projection;
uniform mat4 MVP;
uniform vec3 Position;
//----------------------------------------

void main() {
	frag_color = texture(AlbedoTexture, outTexCoord);
}

