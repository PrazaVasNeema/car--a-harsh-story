#ifndef LIGHTING_INCLUDED
#define LIGHTING_INCLUDED

#include "Assets/DopeRP/GPU/HLSL/Common/Common.hlsl"
#include "Assets/DopeRP/GPU/HLSL/SurfaceData.hlsl"
#include "Assets/DopeRP/GPU/HLSL/BRDF.hlsl"
// #include "Assets/DopeRP/GPU/HLSL/Shadows.hlsl"
// #include "Assets/DopeRP/GPU/HLSL/GI.hlsl"

#define MAX_OTHER_LIGHT_COUNT 20


CBUFFER_START(_LightingMain)

	// #if defined(_DIR_LIGHT_ON)
		float3 _DirLightDirection;
		float3 _DirLightColor;
	// #endif

	// #if defined(MAX_OTHER_LIGHT_COUNT)
		int _OtherLightCount;
		float4 _OtherLightPositions[MAX_OTHER_LIGHT_COUNT];
		float4 _OtherLightColors[MAX_OTHER_LIGHT_COUNT];
		float4 _OtherLightDirections[MAX_OTHER_LIGHT_COUNT];
		float4 _OtherLightSpotAngles[MAX_OTHER_LIGHT_COUNT];
	// #endif

CBUFFER_END


struct Light {
	
	float3 direction;
	float3 color;
	float attenuation;
	
};

// DirectionalShadowData GetDirectionalShadowData (ShadowData shadowData){
// 	DirectionalShadowData data;
// 	data.strength =
// 		_DirectionalLightShadowData.x * shadowData.strength;
//
// 	data.normalBias = _DirectionalLightShadowData.z;
// 	return data;
// }


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

#if defined(MAX_OTHER_LIGHT_COUNT)

Light GetOtherLight (int index, SurfaceData surfaceWS) {
	Light light;
	light.color = _OtherLightColors[index].rgb;
	float3 ray = _OtherLightPositions[index].xyz - surfaceWS.positionWS;
	light.direction = normalize(ray);
	float distanceSqr = max(dot(ray, ray), 0.00001);
	float rangeAttenuation = Square(saturate(1.0 - Square(distanceSqr * _OtherLightPositions[index].w)));
	float4 spotAngles = _OtherLightSpotAngles[index];
	float spotAttenuation = Square(saturate(dot(_OtherLightDirections[index].xyz, light.direction) * spotAngles.x + spotAngles.y));
	light.attenuation = spotAttenuation * rangeAttenuation / distanceSqr * when_lt(index, _OtherLightCount);
	return light;
}

#endif

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

	Light light;
	
	#ifdef _DIR_LIGHT_ON

	light.direction = -_DirLightDirection;
	light.color = _DirLightColor;
	light.attenuation = 1;
	
	// ShadowData shadowData = GetShadowData(surfaceData);
	// DirectionalShadowData dirShadowData = GetDirectionalShadowData(shadowData);
	// dirLight.attenuation = GetDirectionalShadowAttenuation(dirShadowData, shadowData, surfaceData);

	color = GetLighting(surfaceData, light);
	// color = 1;
	#endif	


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