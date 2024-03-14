#ifndef SSAO_DEPTH_NORMALS_INCLUDED
#define SSAO_DEPTH_NORMALS_INCLUDED




//
// TEXTURE2D(_CameraDepthTexture);
// SAMPLER(sampler_CameraDepthTexture);
//
// TEXTURE2D(_BaseMap);
// SAMPLER(sampler_BaseMap);


uniform sampler2D _CameraDepthTexture;
uniform sampler2D _BaseMap;




uniform fixed _DepthLevel;



	uniform half4 _BaseMap_TexelSize;

struct MeshData {
	float4 position : POSITION;
	half2 uv : TEXCOORD0;
	float3 normalOS   : NORMAL;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Interpolators {
	float4 position : SV_POSITION;
	half2 uv : TEXCOORD0;
	float3 normalWS   : VAR_NORMAL;

	UNITY_VERTEX_INPUT_INSTANCE_ID
};

Interpolators vert(MeshData i)
{
	UNITY_SETUP_INSTANCE_ID(i);
	Interpolators o;
	UNITY_TRANSFER_INSTANCE_ID(i, o);

	o.position = UnityObjectToClipPos(i.position);
	o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, i.uv);

	#if UNITY_UV_STARTS_AT_TOP
	if (_BaseMap_TexelSize.y < 0)
		o.uv.y = 1 - o.uv.y;
	#endif
	o.normalWS = UnityObjectToWorldDir(i.normalOS);

	return o;
}


float4 frag(Interpolators i) : COLOR
{
	UNITY_SETUP_INSTANCE_ID(i);



	float3 a = normalize(i.normalWS);


	

	
	return float4(a, i.position.w / 20);
}

#endif