#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

#include "../ShaderLibrary/Common.hlsl"
#include "../S_/SurfaceData.hlsl"
#include "../S_/S_BRDF.hlsl"
#include "../S_/CommonMaterial.hlsl"
#include "../S_/S_Shadows.hlsl"

#define MAX_DIRECTIONAL_LIGHT_COUNT 1

#define MAX_OTHER_LIGHT_COUNT 64

CBUFFER_START(_CustomLight)
	int _DirectionalLightCount;
	float4 _DirectionalLightColors;
	float4 _DirectionalLightDirections;
	float4 _DirectionalLightShadowData;
CBUFFER_END

struct Light {
	float3 color;
	float3 direction;
	float attenuation;
};

DirectionalShadowData GetDirectionalShadowData (ShadowData shadowData){
	DirectionalShadowData data;
	data.strength =
		_DirectionalLightShadowData.x * shadowData.strength;

	data.normalBias = _DirectionalLightShadowData.z;
	return data;
}

// Working with BRDF

float3 isotropicLobe(const SurfaceData surfaceData, const float3 h,
		float NoV, float NoL, float NoH, float LoH) {

	float D = distribution(surfaceData.roughness, NoH, h);
	float V = visibility(surfaceData.roughness, NoV, NoL);
	float3  F = fresnel(surfaceData.f0, LoH);

	return (D * V) * F;
}

float3 specularLobe(const SurfaceData surfaceData, const float3 lightDir, const float3 h,
		float NoV, float NoL, float NoH, float LoH) {
	return isotropicLobe(surfaceData, h, NoV, NoL, NoH, LoH);
}

float3 diffuseLobe(const SurfaceData surfaceData, float NoV, float NoL, float LoH) {
	return surfaceData.color * diffuse(surfaceData.roughness, NoV, NoL, LoH);
}

// ----

Light GetOtherLight (int index, SurfaceData surfaceWS) {
	Light light;
	light.color = _OtherLightColors[index].rgb;
	float3 ray = _OtherLightPositions[index].xyz - surfaceWS.positionWS;
	light.direction = normalize(ray);
	float distanceSqr = max(dot(ray, ray), 0.00001);
	float rangeAttenuation = Square(saturate(1.0 - Square(distanceSqr * _OtherLightPositions[index].w)));
	light.attenuation = rangeAttenuation / distanceSqr * when_lt(index, _OtherLightCount);
	return light;
}

float3 GetLighting(SurfaceData surfaceData, Light light)
{
	float3 h = normalize(surfaceData.viewDirection + light.direction);

	float NoV = clampNoV(dot(surfaceData.normal, surfaceData.viewDirection));
	float NoL = saturate(dot(surfaceData.normal, light.direction));
	float NoH = saturate(dot(surfaceData.normal, h));
	float LoH = saturate(dot(light.direction, h));
	
	float3 Fr = specularLobe(surfaceData, light.direction, h, NoV, NoL, NoH, LoH);
	float3 Fd = diffuseLobe(surfaceData, NoV, NoL, LoH);
	// Fd = dot(surfaceData.normal, -_lightDir);
	// float3 color = Fd * 0.5 + 0.5;
	float3 color = saturate((Fd + Fr) * light.color * NoL * light.attenuation);
	

	
	return color;
}

float3 GetLighting(SurfaceData surfaceData)
{
	float3 color = 0;
	
	// #ifdef _DIR_LIGHT_ON


	
	Light dirLight;
	dirLight.direction = -_DirectionalLightDirection;
	dirLight.color = 1;
	dirLight.attenuation = 1;
	color = GetLighting(surfaceData, dirLight);

	color = 1;

	ShadowData shadowData = GetShadowData(surfaceData);
	
	DirectionalShadowData dirShadowData = GetDirectionalShadowData(shadowData);
	dirLight.attenuation = GetDirectionalShadowAttenuation(dirShadowData, shadowData, surfaceData);

	color = (saturate(dot(surfaceData.normal, dirLight.direction) * dirLight.attenuation) * dirLight.color) * color;
	// color = shadowData.color;
	// color = GetLighting(surfaceData, dirLight);
	// #endif

	Light light;

	#if defined(_OTHER_LIGHT_COUNT_20) || defined(_OTHER_LIGHT_COUNT_15) || defined(_OTHER_LIGHT_COUNT_10) || defined(_OTHER_LIGHT_COUNT_5)

	light = GetOtherLight(0, surfaceData);
	color += GetLighting(surfaceData, light);

	light = GetOtherLight(1, surfaceData);
	color += GetLighting(surfaceData, light);

	light = GetOtherLight(2, surfaceData);
	color += GetLighting(surfaceData, light);

	light = GetOtherLight(3, surfaceData);
	color += GetLighting(surfaceData, light);

	light = GetOtherLight(4, surfaceData);
	color += GetLighting(surfaceData, light);

	#endif
	

	#if defined(_OTHER_LIGHT_COUNT_20) || defined(_OTHER_LIGHT_COUNT_15) || defined(_OTHER_LIGHT_COUNT_10)

	
	light = GetOtherLight(5, surfaceData);
	color += GetLighting(surfaceData, light);

	light = GetOtherLight(6, surfaceData);
	color += GetLighting(surfaceData, light);

	light = GetOtherLight(7, surfaceData);
	color += GetLighting(surfaceData, light);

	light = GetOtherLight(8, surfaceData);
	color += GetLighting(surfaceData, light);

	light = GetOtherLight(9, surfaceData);
	color += GetLighting(surfaceData, light);

	#endif

	#if defined(_OTHER_LIGHT_COUNT_20) || defined(_OTHER_LIGHT_COUNT_15)

	light = GetOtherLight(10, surfaceData);
	color += GetLighting(surfaceData, light);

	light = GetOtherLight(11, surfaceData);
	color += GetLighting(surfaceData, light);

	light = GetOtherLight(12, surfaceData);
	color += GetLighting(surfaceData, light);

	light = GetOtherLight(13, surfaceData);
	color += GetLighting(surfaceData, light);

	light = GetOtherLight(14, surfaceData);
	color += GetLighting(surfaceData, light);

	#endif

	#if defined(_OTHER_LIGHT_COUNT_20)

	light = GetOtherLight(15, surfaceData);
	color += GetLighting(surfaceData, light);

	light = GetOtherLight(16, surfaceData);
	color += GetLighting(surfaceData, light);

	light = GetOtherLight(17, surfaceData);
	color += GetLighting(surfaceData, light);

	light = GetOtherLight(18, surfaceData);
	color += GetLighting(surfaceData, light);

	light = GetOtherLight(19, surfaceData);
	color += GetLighting(surfaceData, light);

	#endif

	
	return color;
}



#endif