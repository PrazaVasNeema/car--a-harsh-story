#ifndef CUSTOM_LIT_PASS_INCLUDED
#define CUSTOM_LIT_PASS_INCLUDED

#include "../ShaderLibrary/Common.hlsl"
#include "../S_/SurfaceData.hlsl"
#include "../S_/S_BRDF.hlsl"
#include "../S_/CommonMaterial.hlsl"


TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);

#define l _DirectionalLightDirection
#define MAX_OTHER_LIGHT_COUNT 64

CBUFFER_START(UnityPerMaterial)
	float4 _BaseColor;
	float4 _BaseMap_ST; //texture scale and transform params
	float _Metallic;
	float _Roughness;
	float _Reflectance;
	float3 l;

	int _DirLightCount;

	int _OtherLightCount;
	float4 _OtherLightColors[MAX_OTHER_LIGHT_COUNT];
	float4 _OtherLightPositions[MAX_OTHER_LIGHT_COUNT];
CBUFFER_END

struct MeshData {
	float3 positionOS : POSITION;
	float3 normalOS   : NORMAL;
	float2 uv         : TEXCOORD0;
};

struct Interpolators {
	float4 positionCS : SV_POSITION;
	float3 positionWS : VAR_POSITION;
	float3 normalWS   : VAR_NORMAL;
	float2 uv         : TEXCOORD0;
	float4 fragPosLight : TEXCOORD1;
};

Interpolators vert(MeshData i)
{
	Interpolators o;
	o.positionWS = TransformObjectToWorld(i.positionOS.xyz);
	o.positionCS = TransformWorldToHClip(o.positionWS);
	o.normalWS = TransformObjectToWorldNormal(i.normalOS);
	o.uv = i.uv * _BaseMap_ST.xy + _BaseMap_ST.zw;
	
	
	return o;
}

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

struct Light {
	float3 color;
	float3 direction; //to light source
	float attenuation;
};

int GetOtherLightCount () {
	return _OtherLightCount;
}

Light GetOtherLight (int index, SurfaceData surfaceWS) {
	Light light;
	light.color = _OtherLightColors[index].rgb;
	float3 ray = _OtherLightPositions[index].xyz - surfaceWS.positionWS;
	light.direction = normalize(ray);
	float distanceSqr = max(dot(ray, ray), 0.00001);
	float rangeAttenuation = Square(saturate(1.0 - Square(distanceSqr * _OtherLightPositions[index].w)));
	light.attenuation = rangeAttenuation / distanceSqr;
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

float4 frag(Interpolators i) : SV_TARGET
{
	float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
	baseColor *= _BaseColor;

	SurfaceData surfaceData;
	surfaceData.normal = normalize(i.normalWS);
	surfaceData.viewDirection = normalize(_WorldSpaceCameraPos - i.positionWS);
	surfaceData.color = computeDiffuseColor(baseColor.rgb, _Metallic);
	surfaceData.positionWS = i.positionWS;
	surfaceData.alpha = baseColor.a;
	surfaceData.metallic = _Metallic;
	surfaceData.roughness = perceptualRoughnessToRoughness(_Roughness);
	surfaceData.f0 = computeReflectance(baseColor, _Metallic, _Reflectance);

	float3 a = l;



	float3 color;

	if (_DirLightCount > 0)
	{
		Light dirLight;
		dirLight.direction = -l;
		dirLight.color = 1;
		dirLight.attenuation = 1;

		color = GetLighting(surfaceData, dirLight);

	}

	
	
	for (int j = 0; j < GetOtherLightCount(); j++) {
		Light light = GetOtherLight(j, surfaceData);
		color += GetLighting(surfaceData, light);
	}
	
	// color = _lightDir;
	return float4(color, surfaceData.alpha);
}

#endif