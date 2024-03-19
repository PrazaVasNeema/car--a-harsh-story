#ifndef CUSTOM_LIT_PASS_INCLUDED
#define CUSTOM_LIT_PASS_INCLUDED


#include "../S_/SurfaceData.hlsl"
#include "../S_/S_BRDF.hlsl"
#include "../S_/CommonMaterial.hlsl"
#include "../S_/S_Lighting.hlsl"

#include "../S_/S_SSAO.hlsl"




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

TEXTURE2D(_DecalsAtlas);
SAMPLER(sampler_DecalsAtlas);

struct MeshData {
	float3 positionOS : POSITION;
	float3 normalOS   : NORMAL;
	float2 uv         : TEXCOORD0;
	GI_ATTRIBUTE_DATA
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Interpolators {
	float4 positionCS : SV_POSITION;
	float3 positionWS : VAR_POSITION;
	float3 normalWS   : VAR_NORMAL;
	float2 uv         : TEXCOORD0;
	float4 fragPosLight : TEXCOORD1;
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
	float4 baseMap_ST =  UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseMap_ST);
	o.uv = i.uv * baseMap_ST.xy + baseMap_ST.zw;
	
	
	return o;
}


float4 frag(Interpolators i) : SV_TARGET
{
	UNITY_SETUP_INSTANCE_ID(i);
	float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
	baseColor *= UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseColor);
	float ssao = SAMPLE_TEXTURE2D(_SSAOAtlasBlurred, sampler_SSAOAtlasBlurred, i.positionCS / _ScreenSize).r;
	float4 decals = SAMPLE_TEXTURE2D(_DecalsAtlas, sampler_DecalsAtlas, i.positionCS / _ScreenSize);
	baseColor *= ssao;
	if (decals.a >0)
		baseColor = decals;
	SurfaceData surfaceData;
	surfaceData.normal = normalize(i.normalWS);
	surfaceData.viewDirection = normalize(_WorldSpaceCameraPos - i.positionWS);
	surfaceData.depth = -TransformWorldToView(i.positionWS).z;
	surfaceData.metallic = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Metallic);
	#ifdef _PREMULTIPLY_ALPHA
	surfaceData.color = computeDiffuseColor(baseColor.rgb, surfaceData.metallic) * baseColor.a;
	#else
	surfaceData.color = computeDiffuseColor(baseColor.rgb, surfaceData.metallic);
	#endif
	surfaceData.positionWS = i.positionWS;
	surfaceData.alpha = baseColor.a;
	surfaceData.roughness = perceptualRoughnessToRoughness(UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Roughness));
	surfaceData.f0 = computeReflectance(baseColor, surfaceData.metallic, UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Reflectance));
float3 color = 0;;
	
	GI gi = GetGI(GI_FRAGMENT_DATA(input), surfaceData);

	color += IndirectBRDF(surfaceData, gi.specular);

	color += GetLighting(surfaceData);
	// return float4(ssao.xxx, 1);
	return float4(color * 1, surfaceData.alpha);
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

	// color = CalculateSSAO(i.positionCS);

	// return float4(color,1);
	
	float4 frag = SAMPLE_TEXTURE2D(_PositionViewSpace, sampler_PositionViewSpace, i.uv);

	// float4 frag3 = SAMPLE_TEXTURE2D(_SSAOAtlas, sampler_SSAOAtlas, i.positionCS / _ScreenSize);

	float4 frag3 = SAMPLE_TEXTURE2D(_SSAOAtlasBlurred, sampler_SSAOAtlasBlurred, i.positionCS / _ScreenSize);
	
	return frag3;
}

#endif