﻿#version 450
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
uniform sampler2D DecalTexture;
uniform sampler2D ReflectionTexture;
uniform mat4 Model;
uniform mat4 View;
uniform mat4 Projection;
uniform mat4 MVP;
uniform vec3 MainLightPos;
uniform vec4 MainLightTint;
uniform float MainLightConstant;
uniform float MainLightLinear;
uniform float MainLightQuadratic;
uniform float AmbientLightStrength;
uniform vec3 Position;
uniform vec3 Rotation;
//----------------------------------------

void main() {
	float ambientLightStrength = 1/2;
	vec3 ambientBase = ambientLightStrength * MainLightTint.xyz;
	vec3 normalizedNormal = normalize(outNormal);
	vec3 lightDirection = normalize(MainLightPos - vec3(Model * vec4(outFragPos, 1)));
	float diff = dot(normalizedNormal, lightDirection);
	float distance = length(MainLightPos - outFragPos);
	float attenuation = 1.0 / (MainLightConstant + MainLightLinear * distance + MainLightQuadratic * (distance * distance));
	vec3 diffuseBase = diff * attenuation * MainLightTint.xyz;

	vec4 mvpTex = MVP * vec4(outTexCoord + Rotation.xy, 1.0, 1.0);
	
	vec3 result = texture(AlbedoTexture, outTexCoord).xyz;
	result = mix(result, texture(ReflectionTexture, vec2(mvpTex.x + (Position.z / 500), mvpTex.y + (-Position.x / 500) + 5) * 1.0).xyz, 0.5);
	vec4 decalCol = texture(DecalTexture, outTexCoord);
	if (decalCol.w > 0)
		result = mix(result.xyz, decalCol.xyz, decalCol.w);

	result = max((ambientBase + diffuseBase), 0.2) * result;

	frag_color = vec4(result, 1.0);
	//frag_color = texture(AlbedoTexture, outTexCoord);
}

