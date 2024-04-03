﻿#ifndef LIT_PASS_INCLUDED
#define LIT_PASS_INCLUDED


#include "Assets/DopeRP/GPU/HLSL/Common/Common.hlsl"
#include "Assets/DopeRP/GPU/HLSL/SurfaceData.hlsl"
#include "Assets/DopeRP/GPU/HLSL/GI.hlsl"
#include "Assets/DopeRP/GPU/HLSL/Lighting.hlsl"




CBUFFER_START(LitMain)

	float4 _ScreenSize;

float2 _nearFarPlanes;
float4x4 _INVERSE_P;
float4x4 adfgdgf_CameraToWorldMatrix;

CBUFFER_END

TEXTURE2D(_AlbedoMap);
TEXTURE2D(_EmissionMap);
SAMPLER(sampler_AlbedoMap);

TEXTURE2D(_atlas1);
SAMPLER(sampler_atlas1);

TEXTURE2D(_PositionViewSpace);
SAMPLER(sampler_PositionViewSpace);

TEXTURE2D(_NormalViewSpace);
SAMPLER(sampler_NormalViewSpace);

TEXTURE2D(_SSAOAtlas);
SAMPLER(sampler_SSAOAtlas);

TEXTURE2D(_SSAOAtlasBlurred);
SAMPLER(sampler_SSAOAtlasBlurred);

TEXTURE2D(_DecalsAlbedoAtlas);
SAMPLER(sampler_DecalsAlbedoAtlas);

TEXTURE2D(_DecalsNormalAtlas);
SAMPLER(sampler_DecalsNormalAtlas);

TEXTURE2D(_NormalMap);






TEXTURE2D(_SSAORawAtlas);
SAMPLER(sampler_SSAORawAtlas);

TEXTURE2D(_SSAOBlurAtlas);
SAMPLER(sampler_SSAOBlurAtlas);

TEXTURE2D(_G_AlbedoAtlas);
SAMPLER(sampler_G_AlbedoAtlas);

TEXTURE2D(_G_NormalWorldSpaceAtlas);
SAMPLER(sampler_G_NormalWorldSpaceAtlas);

TEXTURE2D(_G_SpecularAtlas);
SAMPLER(sampler_G_SpecularAtlas);

TEXTURE2D(_G_BRDFAtlas);
SAMPLER(sampler_G_BRDFAtlas);

TEXTURE2D(Test);
SAMPLER(samplerTest);


struct MeshData {
	float3 positionOS : POSITION;
	float3 normalOS   : NORMAL;
	float2 uv         : TEXCOORD0;
	float4 tangentOS : TANGENT;
	
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Interpolators {
	float4 positionCS : SV_POSITION;
	float3 positionWS : VAR_POSITION;
	float3 normalWS   : VAR_NORMAL;
	float2 uv         : TEXCOORD0;
	float4 tangentWS : VAR_TANGENT;
	
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

Interpolators vert(MeshData i)
{
	UNITY_SETUP_INSTANCE_ID(i);
	Interpolators o;
	UNITY_TRANSFER_INSTANCE_ID(i, o);
	
	o.positionWS = TransformObjectToWorld(i.positionOS.xyz);
	o.positionCS = TransformWorldToHClip(o.positionWS);
	o.normalWS = TransformObjectToWorldNormal(i.normalOS);
	
	o.uv = i.uv;
	o.tangentWS = float4(TransformObjectToWorldDir(i.tangentOS.xyz), i.tangentOS.w);
	
	return o;
}





float4 frag(Interpolators i) : SV_TARGET
{
	
	UNITY_SETUP_INSTANCE_ID(i);
	float4 baseColor = SAMPLE_TEXTURE2D(_G_AlbedoAtlas, sampler_G_AlbedoAtlas, i.uv);
	
	clip(baseColor.a-0.0001);

	#if defined(SSAO_ON)
	
	float ssao = SAMPLE_TEXTURE2D(_SSAOBlurAtlas, sampler_SSAOBlurAtlas, i.uv).r;
	if (ssao > 0)
		baseColor *= ssao;

	#endif
	


	
	SurfaceData surfaceData;
	surfaceData.normal = SAMPLE_TEXTURE2D(_G_NormalWorldSpaceAtlas, sampler_G_NormalWorldSpaceAtlas, i.uv).xyz;

	float depth = SAMPLE_TEXTURE2D(Test, samplerTest, i.uv).r;

    
	depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, depth);
	
	float sceneZ =CalcLinearZ(depth, _nearFarPlanes.x, _nearFarPlanes.y);

	float4 clipSpacePosition = float4((i.uv * 2.0 - 1.0) * sceneZ/depth, sceneZ, 1.0 * sceneZ/depth);

	float4 viewSpacePosition = mul(_INVERSE_P, clipSpacePosition);

	viewSpacePosition /= viewSpacePosition.w;
	
	surfaceData.positionWS = mul(adfgdgf_CameraToWorldMatrix, viewSpacePosition).xyz;

	surfaceData.viewDirection = normalize(_WorldSpaceCameraPos - surfaceData.positionWS);
	surfaceData.depth = -TransformWorldToView(surfaceData.positionWS).z;

	float3 brdfAtlas = SAMPLE_TEXTURE2D(_G_BRDFAtlas, sampler_G_BRDFAtlas, i.uv).xyz;
	
	surfaceData.metallic = brdfAtlas.x;
	#ifdef _PREMULTIPLY_ALPHA
		surfaceData.color = computeDiffuseColor(baseColor.rgb, surfaceData.metallic) * baseColor.a;
	#else
		surfaceData.color = computeDiffuseColor(baseColor.rgb, surfaceData.metallic);
	#endif
	surfaceData.alpha = baseColor.a;
	surfaceData.roughness = brdfAtlas.y;
	surfaceData.f0 = computeReflectance(baseColor, surfaceData.metallic, brdfAtlas.z);

	float4 specularAtlas = SAMPLE_TEXTURE2D(_G_SpecularAtlas, sampler_G_SpecularAtlas, i.uv);

	surfaceData.specular = specularAtlas.xyz;

	// surfaceData.dither = InterleavedGradientNoise(i.positionCS.xy, 0);
// return float4(surfaceData.f0.xyz, 1);
	// const float airIor = 1.0;
	// float materialor = f0ToIor(surfaceData.f0.g);
	// surfaceData.etaIR = airIor / materialor;  // air -> material
	// surfaceData.etaRI = materialor / airIor;  // material -> air

	// #if defined(MATERIAL_HAS_TRANSMISSION)
	// surfaceData.transmission = saturate(material.transmission);
	// #else
	// surfaceData.transmission = 1.0;
	// #endif
	//
	// #if defined(MATERIAL_HAS_ABSORPTION)
	// #if defined(MATERIAL_HAS_THICKNESS) || defined(MATERIAL_HAS_MICRO_THICKNESS)
	// surfaceData.absorption = max(0.0, material.absorption);
	// #else
	// surfaceData.absorption = saturate(material.absorption);
	// #endif
	// #else
	// surfaceData.absorption = 0.0;
	// #endif
	// #if defined(MATERIAL_HAS_THICKNESS)
	// pixel.thickness = max(0.0, material.thickness);
	// #endif
	// #if defined(MATERIAL_HAS_MICRO_THICKNESS) && (REFRACTION_TYPE == REFRACTION_TYPE_THIN)
	// pixel.uThickness = max(0.0, material.microThickness);
	// #else
	// surfaceData.uThickness = 0.0;
	// #endif
	// #endif
	
	float3 fragColor = 0;

	float3 ambientSafetyNet = baseColor * 0.02;
	
	// GI gi = GetGI(GI_FRAGMENT_DATA(input), surfaceData);
	fragColor += IndirectBRDF(surfaceData, surfaceData.specular)* 0.1;

	fragColor += GetLighting(surfaceData);
	// fragColor += GetEmission(i.uv) ;
	
	return float4(max(fragColor, ambientSafetyNet), surfaceData.alpha);





	/*
	float4 baseColor = SAMPLE_TEXTURE2D(_AlbedoMap, sampler_AlbedoMap, i.uv);
	// return float4(i.uv, 0, 1);
	// return baseColor;
	baseColor *= UNITY_ACCESS_INSTANCED_PROP(LitBasePerMaterial, _BaseColor);

	float2 screenSpaceCoordinates;
	#if UNITY_REVERSED_Z
		screenSpaceCoordinates = float2((i.positionCS.x * _ScreenSize.z), (1 - i.positionCS.y * _ScreenSize.w));
	#else
		screenSpaceCoordinates = i.positionCS * _ScreenSize.zw;
	#endif
	screenSpaceCoordinates = i.positionCS * _ScreenSize.zw;

	#if defined(SSAO_ON)
	
	float ssao = SAMPLE_TEXTURE2D(_SSAOBlurAtlas, sampler_SSAOBlurAtlas, screenSpaceCoordinates).r;
	if (ssao > 0)
		baseColor *= ssao;

	#endif

	// float3 ssao2 = SAMPLE_TEXTURE2D(_SSAOBlurAtlas, sampler_SSAOBlurAtlas, screenSpaceCoordinates).rgb;
	// return float4(ssao2,1);

	// baseColor *= 1;


	
	SurfaceData surfaceData;
	surfaceData.normal = NormalTangentToWorld(GetNormalTS(i.uv), normalize(i.normalWS), normalize(i.tangentWS));
	// surfaceData.normal = normalize(i.normalWS);

	#if defined(DECALS_ON)
	
	float4 decals = SAMPLE_TEXTURE2D(_DecalsAlbedoAtlas, sampler_DecalsAlbedoAtlas, screenSpaceCoordinates);
	if (decals.a > 0)
		baseColor = decals;
	float4 decalsNormals = SAMPLE_TEXTURE2D(_DecalsNormalAtlas, sampler_DecalsNormalAtlas, screenSpaceCoordinates);
	if (decalsNormals.a >0)
	{
		surfaceData.normal = decalsNormals;
	}

	#endif

	surfaceData.viewDirection = normalize(_WorldSpaceCameraPos - i.positionWS);
	surfaceData.depth = -TransformWorldToView(i.positionWS).z;
	surfaceData.metallic = UNITY_ACCESS_INSTANCED_PROP(LitBasePerMaterial, _Metallic);
	#ifdef _PREMULTIPLY_ALPHA
		surfaceData.color = computeDiffuseColor(baseColor.rgb, surfaceData.metallic) * baseColor.a;
	#else
		surfaceData.color = computeDiffuseColor(baseColor.rgb, surfaceData.metallic);
	#endif
	surfaceData.positionWS = i.positionWS;
	surfaceData.alpha = baseColor.a;
	surfaceData.roughness = perceptualRoughnessToRoughness(UNITY_ACCESS_INSTANCED_PROP(LitBasePerMaterial, _Roughness));
	surfaceData.f0 = computeReflectance(baseColor, surfaceData.metallic, UNITY_ACCESS_INSTANCED_PROP(LitBasePerMaterial, _Reflectance));
	// surfaceData.dither = InterleavedGradientNoise(i.positionCS.xy, 0);
// return float4(surfaceData.f0.xyz, 1);
	// const float airIor = 1.0;
	// float materialor = f0ToIor(surfaceData.f0.g);
	// surfaceData.etaIR = airIor / materialor;  // air -> material
	// surfaceData.etaRI = materialor / airIor;  // material -> air

	// #if defined(MATERIAL_HAS_TRANSMISSION)
	// surfaceData.transmission = saturate(material.transmission);
	// #else
	// surfaceData.transmission = 1.0;
	// #endif
	//
	// #if defined(MATERIAL_HAS_ABSORPTION)
	// #if defined(MATERIAL_HAS_THICKNESS) || defined(MATERIAL_HAS_MICRO_THICKNESS)
	// surfaceData.absorption = max(0.0, material.absorption);
	// #else
	// surfaceData.absorption = saturate(material.absorption);
	// #endif
	// #else
	// surfaceData.absorption = 0.0;
	// #endif
	// #if defined(MATERIAL_HAS_THICKNESS)
	// pixel.thickness = max(0.0, material.thickness);
	// #endif
	// #if defined(MATERIAL_HAS_MICRO_THICKNESS) && (REFRACTION_TYPE == REFRACTION_TYPE_THIN)
	// pixel.uThickness = max(0.0, material.microThickness);
	// #else
	// surfaceData.uThickness = 0.0;
	// #endif
	// #endif
	
	float3 fragColor = 0;;
	
	GI gi = GetGI(GI_FRAGMENT_DATA(input), surfaceData);
	fragColor += IndirectBRDF(surfaceData, gi.specular)* 0.1;

	fragColor += GetLighting(surfaceData);
	fragColor += GetEmission(i.uv) ;
	
	return float4(fragColor, surfaceData.alpha);
	 */
	
}

#endif