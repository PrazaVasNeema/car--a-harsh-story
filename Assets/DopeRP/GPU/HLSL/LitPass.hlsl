#ifndef LIT_PASS_INCLUDED
#define LIT_PASS_INCLUDED


#include "Assets/DopeRP/GPU/HLSL/Common/Common.hlsl"
#include "Assets/DopeRP/GPU/HLSL/SurfaceData.hlsl"
#include "Assets/DopeRP/GPU/HLSL/GI.hlsl"
#include "Assets/DopeRP/GPU/HLSL/Lighting.hlsl"


UNITY_INSTANCING_BUFFER_START(LitBasePerMaterial)

	UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
	UNITY_DEFINE_INSTANCED_PROP(float4, _EmissionColor)
	UNITY_DEFINE_INSTANCED_PROP(float4, _AlbedoMap_ST)
	UNITY_DEFINE_INSTANCED_PROP(float, _NormalScale)

	UNITY_DEFINE_INSTANCED_PROP(float, _Metallic)
	UNITY_DEFINE_INSTANCED_PROP(float, _Roughness)
	UNITY_DEFINE_INSTANCED_PROP(float, _Reflectance)	

UNITY_INSTANCING_BUFFER_END(LitBasePerMaterial)

CBUFFER_START(LitMain)

	float4 _ScreenSize;

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

struct MeshData {
	float3 positionOS : POSITION;
	float3 normalOS   : NORMAL;
	float2 uv         : TEXCOORD0;
	float4 tangentOS : TANGENT;
	
	GI_ATTRIBUTE_DATA
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Interpolators {
	float4 positionCS : SV_POSITION;
	float3 positionWS : VAR_POSITION;
	float3 normalWS   : VAR_NORMAL;
	float2 uv         : TEXCOORD0;
	float4 tangentWS : VAR_TANGENT;
	
	GI_VARYINGS_DATA
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

Interpolators vert(MeshData i)
{
	UNITY_SETUP_INSTANCE_ID(i);
	Interpolators o;
	UNITY_TRANSFER_INSTANCE_ID(i, o);
	TRANSFER_GI_DATA(input, output);
	
	o.positionWS = TransformObjectToWorld(i.positionOS.xyz);
	o.positionCS = TransformWorldToHClip(o.positionWS);
	o.normalWS = TransformObjectToWorldNormal(i.normalOS);
	float4 baseMap_ST =  UNITY_ACCESS_INSTANCED_PROP(LitBasePerMaterial, _AlbedoMap_ST);
	o.uv = i.uv * baseMap_ST.xy + baseMap_ST.zw;
	o.tangentWS = float4(TransformObjectToWorldDir(i.tangentOS.xyz), i.tangentOS.w);
	
	return o;
}

float3 GetEmission (float2 baseUV) {
	float4 map = SAMPLE_TEXTURE2D(_EmissionMap, sampler_AlbedoMap, baseUV);
	float4 color = UNITY_ACCESS_INSTANCED_PROP(LitBasePerMaterial, _EmissionColor);
	return map.rgb * color.rgb;
}

float3 GetNormalTS (float2 baseUV) {
	float4 map = SAMPLE_TEXTURE2D(_NormalMap, sampler_AlbedoMap, baseUV);
	float scale = UNITY_ACCESS_INSTANCED_PROP(LitBasePerMaterial, _NormalScale);
	float3 normal = DecodeNormal(map, scale);
	
	return normal;
}

float3 NormalTangentToWorld (float3 normalTS, float3 normalWS, float4 tangentWS) {
	float3x3 tangentToWorld = CreateTangentToWorld(normalWS, tangentWS.xyz, tangentWS.w);
	
	return TransformTangentToWorld(normalTS, tangentToWorld);
}

float4 frag(Interpolators i) : SV_TARGET
{
	
	UNITY_SETUP_INSTANCE_ID(i);
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

	#if defined(SSAO_ON)
	
	float ssao = SAMPLE_TEXTURE2D(_SSAOBlurAtlas, sampler_SSAOBlurAtlas, screenSpaceCoordinates).r;
	if (ssao > 0)
		baseColor *= ssao;

	#endif

	float4 decals = SAMPLE_TEXTURE2D(_DecalsAlbedoAtlas, sampler_DecalsAlbedoAtlas, screenSpaceCoordinates);

	// baseColor *= 1;

	if (decals.a > 0)
		baseColor = decals;
	
	SurfaceData surfaceData;
	surfaceData.normal = NormalTangentToWorld(GetNormalTS(i.uv), normalize(i.normalWS), normalize(i.tangentWS));
	// surfaceData.normal = normalize(i.normalWS);
	
	float4 decalsNormals = SAMPLE_TEXTURE2D(_DecalsNormalAtlas, sampler_DecalsNormalAtlas, screenSpaceCoordinates);
	if (decalsNormals.a >0)
	{
		surfaceData.normal = decalsNormals;
	}

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
	surfaceData.dither = InterleavedGradientNoise(i.positionCS.xy, 0);
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
	
}

#endif