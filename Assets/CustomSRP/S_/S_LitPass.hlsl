#ifndef CUSTOM_LIT_PASS_INCLUDED
#define CUSTOM_LIT_PASS_INCLUDED


#include "../S_/SurfaceData.hlsl"
#include "../S_/S_BRDF.hlsl"
#include "../S_/CommonMaterial.hlsl"
#include "../S_/S_Lighting.hlsl"


TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);




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


float4 frag(Interpolators i) : SV_TARGET
{
	float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
	baseColor *= _BaseColor;

	SurfaceData surfaceData;
	surfaceData.normal = normalize(i.normalWS);
	surfaceData.viewDirection = normalize(_WorldSpaceCameraPos - i.positionWS);
	surfaceData.depth = -TransformWorldToView(i.positionWS).z;
	surfaceData.color = computeDiffuseColor(baseColor.rgb, _Metallic);
	surfaceData.positionWS = i.positionWS;
	surfaceData.alpha = baseColor.a;
	surfaceData.metallic = _Metallic;
	surfaceData.roughness = perceptualRoughnessToRoughness(_Roughness);
	surfaceData.f0 = computeReflectance(baseColor, _Metallic, _Reflectance);

float3 color = 0;


	color = GetLighting(surfaceData);

	
	// bool dirLightExist = when_gt(_DirLightCount, 0);
	//
	// Light dirLight;
	//
	// dirLight.direction = -l;
	// dirLight.color = 1;
	// dirLight.attenuation = 1 * dirLightExist;
	//
	// color = GetLighting(surfaceData, dirLight);


	// color = b;
	// color = _lightDir;
	return float4(color, surfaceData.alpha);
}

#endif