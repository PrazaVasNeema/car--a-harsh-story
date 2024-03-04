#ifndef CUSTOM_LIT_PASS_INCLUDED
#define CUSTOM_LIT_PASS_INCLUDED

#include "../ShaderLibrary/Common.hlsl"
#include "../S_/SurfaceData.hlsl"
#include "../S_/S_BRDF.hlsl"
#include "../S_/CommonMaterial.hlsl"


TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);

CBUFFER_START(UnityPerMaterial)
	float4 _BaseColor;
	float4 _BaseMap_ST; //texture scale and transform params
	float _Metallic;
	float _Roughness;
CBUFFER_END

uniform float3 _lightDir;

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

	float D = distribution(pixel.roughness, NoH, h);
	float V = visibility(pixel.roughness, NoV, NoL);
	float3  F = fresnel(pixel.f0, LoH);

	return (D * V) * F;
}

float3 diffuseLobe(const SurfaceData surfaceData, float NoV, float NoL, float LoH) {
	return surfaceData.color * diffuse(surfaceData.roughness, NoV, NoL, LoH);
}

float4 frag(Interpolators fragmentInput) : SV_TARGET
{
	float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, fragmentInput.uv);
	baseColor *= _BaseColor;

	SurfaceData surfaceData;
	surfaceData.normal = normalize(fragmentInput.normalWS);
	surfaceData.viewDirection = normalize(_WorldSpaceCameraPos - fragmentInput.positionWS);
	surfaceData.color = computeDiffuseColor(baseColor.rgb, _Metallic);
	surfaceData.alpha = baseColor.a;
	surfaceData.metallic = _Metallic;
	surfaceData.roughness = perceptualRoughnessToRoughness(_Roughness);

float3 a = _lightDir;
	float3 h = normalize(surfaceData.viewDirection + -_lightDir);

	float NoV = clampNoV(dot(surfaceData.normal, surfaceData.viewDirection));
	float NoL = saturate(dot(surfaceData.normal, -_lightDir));
	float NoH = saturate(dot(surfaceData.normal, h));
	float LoH = saturate(dot(-_lightDir, h));

	float3 Fd = diffuseLobe(surfaceData, NoV, NoL, LoH);
	// Fd = dot(surfaceData.normal, -_lightDir);
	// float3 color = Fd * 0.5 + 0.5;
	float3 color = Fd;

	
	// color = _lightDir;

	return float4(color, surfaceData.alpha);
}

#endif