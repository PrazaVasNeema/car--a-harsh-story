#ifndef GBUFFER_PASS_INCLUDED
#define GBUFFER_PASS_INCLUDED

#include "Assets/DopeRP/GPU/HLSL/Common/Common.hlsl"

CBUFFER_START(GBuffer)

    float _CameraNearPlane;
    float _CameraFarPlane;

CBUFFER_END

struct MeshData {
    float4 position : POSITION;
    float3 normal   : NORMAL;
    float4 tangentOS : TANGENT;

    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Interpolators {
    float4 position : SV_POSITION;
    float3 positionVS   : TEXCOORD0;
    float3 normalVS   : TEXCOORD1;
    float3 positionWS : TEXCOORD2;
    float4 tangentWS : VAR_TANGENT;
    float3 normalWS : TEXCOORD3;
    float4 positionCS: TEXCOORD4;

    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct fragOutput
{
    float4 positionViewSpace : SV_Target0;
    float4 normalViewSpace : SV_Target1;
    float4 tangentViewSpace : SV_Target2;
};

Interpolators vert(MeshData i)
{
    UNITY_SETUP_INSTANCE_ID(i);
    Interpolators o;
    UNITY_TRANSFER_INSTANCE_ID(i, o);
   
    o.position = TransformObjectToHClip(i.position);
    o.positionVS = TransformWorldToView(TransformObjectToWorld(i.position));
    o.normalVS = TransformWorldToViewNormal(TransformObjectToWorldNormal(i.normal));
    o.positionWS = TransformObjectToWorld(i.position);
    o.tangentWS = float4(TransformObjectToWorldDir(i.tangentOS.xyz), i.tangentOS.w);
    o.normalWS = TransformObjectToWorldNormal(i.normal);

    o.positionCS = TransformObjectToHClip(i.position);
    return o;
}

fragOutput frag(Interpolators i) 
{
    UNITY_SETUP_INSTANCE_ID(i);

    fragOutput o;
    
    o.normalViewSpace = mul(Inverse(GetViewToHClipMatrix()), i.position/i.position.w);

    // o.normalViewSpace = float4(i.position.xy-100,0,1);

    o.normalViewSpace = i.position;

    o.normalViewSpace = i.positionCS;

    o.normalViewSpace = float4(i.normalVS, 1);
    
    // o.normalViewSpace = i.positionCS/i.positionCS.w;

    // o.normalViewSpace = mul(Inverse(GetViewToHClipMatrix()), i.positionCS/i.positionCS.w);


    // o.normalViewSpace = float4(mul(Inverse(GetViewToWorldMatrix()), float4(i.positionVS, 1)).xyz, 1);

    

    o.positionViewSpace = float4(i.positionVS.xyz, 1);

    // o.positionViewSpace = float4(mul(GetViewToWorldMatrix(), float4(i.positionVS, 1)).xyz, 1);

    // o.positionViewSpace = float4(i.positionWS, 1);

    // o.positionViewSpace = float4(mul(UNITY_MATRIX_I_V, i.normalVS).xyz, 1);

    // o.positionViewSpace = float4(i.normalVS, 1);

    
    // o.positionViewSpace = float4(TransformViewToWorld(i.positionVS), 1);


    o.tangentViewSpace = float4(i.positionWS, 1);
    
    return o;
    
}

#endif