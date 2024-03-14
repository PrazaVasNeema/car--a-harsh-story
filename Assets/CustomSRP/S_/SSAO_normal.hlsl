#ifndef NORMAL_BUFFER_INCLUDED
#define NORMAL_BUFFER_INCLUDED


#include "../S_/SurfaceData.hlsl"
#include "../S_/S_BRDF.hlsl"
#include "../S_/CommonMaterial.hlsl"

UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
				UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
				UNITY_DEFINE_INSTANCED_PROP(float4, _BaseMap_ST)
				UNITY_DEFINE_INSTANCED_PROP(float, _Metallic)
				UNITY_DEFINE_INSTANCED_PROP(float, _Roughness)
				UNITY_DEFINE_INSTANCED_PROP(float, _Reflectance)

				

			UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)


struct MeshData {
	float4 positionOS : POSITION;
	float3 normalOS   : NORMAL;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Interpolators {
	float4 positionCS : SV_POSITION;
	float3 normalWS   : VAR_NORMAL;

	UNITY_VERTEX_INPUT_INSTANCE_ID
};

Interpolators vert(MeshData i)
{
	UNITY_SETUP_INSTANCE_ID(i);
	Interpolators o;
	UNITY_TRANSFER_INSTANCE_ID(i, o);
	o.positionCS = TransformWorldToHClip(TransformObjectToWorld(i.positionOS.xyz));

	o.normalWS = TransformObjectToWorldNormal(i.normalOS);
	return o;
}


float4 frag(Interpolators i) : SV_TARGET
{
	UNITY_SETUP_INSTANCE_ID(i);
	float4 baseColor = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseColor);


	float4 color = i.positionCS * 0.5 - 10.5;

	float3 a = normalize(i.normalWS);
	
	return float4(a.xyz, 1);
}

#endif