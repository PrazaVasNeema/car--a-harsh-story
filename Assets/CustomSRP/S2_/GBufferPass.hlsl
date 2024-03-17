#ifndef GBUFFER_PASS_INCLUDED
#define GBUFFER_PASS_INCLUDED

#include "../ShaderLibrary/Common.hlsl"

CBUFFER_START(GBuffer)

    float _CameraNearPlane;
    float _CameraFarPlane;

CBUFFER_END

struct MeshData {
    float4 position : POSITION;
    float3 normal   : NORMAL;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Interpolators {
    float4 position : SV_POSITION;
    float3 positionVS   : TEXCOORD0;
    float3 normalVS   : TEXCOORD1;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct fragOutput
{
    float4 positionViewSpace : SV_Target0;
    float4 normalViewSpace : SV_Target1;
};

Interpolators vert(MeshData i)
{
    UNITY_SETUP_INSTANCE_ID(i);
    Interpolators o;
    UNITY_TRANSFER_INSTANCE_ID(i, o);
   
    o.position = TransformObjectToHClip(i.position);
    o.positionVS = TransformWorldToView(TransformObjectToWorld(i.position));
    o.normalVS = TransformWorldToViewNormal(TransformObjectToWorldNormal(i.normal));
    // o.position = float4(TransformWorldToView(TransformObjectToWorld(i.position)), 1);
    // o.normalVS = TransformObjectToWorldNormal(i.normal);
    // o.positionVS = mul(UNITY_MATRIX_V, i.position).xyz;
    // o.positionVS = mul(UNITY_MATRIX_P, TransformWorldToView(TransformObjectToWorld(i.position)));
    return o;
}

fragOutput frag(Interpolators i) 
{
    UNITY_SETUP_INSTANCE_ID(i);

    fragOutput o;
    
    // i.normalVS = normalize(i.normalVS);
    float depth = (i.position.z - _CameraNearPlane) / (_CameraFarPlane - _CameraNearPlane);
    float4 a = mul(UNITY_MATRIX_P, i.positionVS.xyz);
    float zDepth = a.z / a.w;

    #if !defined(UNITY_REVERSED_Z) // basically only OpenGL
    zDepth = zDepth * 0.5 + 0.5; // remap -1 to 1 range to 0.0 to 1.0
    #endif
    // o.positionViewSpace = float4(i.positionVS.xy/i.positionVS.w *0.5 + 0.5, 0, 1);
    // o.positionViewSpace = float4(i.positionVS.xy, zDepth, 1);
    // o.positionViewSpace = float4(a.xyz/a.w, 1);
    //
    o.normalViewSpace = float4(i.normalVS, 1);
    // zDepth = a.z/a.w;
    // zDepth = zDepth * 0.5 + 0.5;
    o.positionViewSpace = float4(i.positionVS.xy, i.positionVS.z, 1);

    // o.positionViewSpace = mul(UNITY_MATRIX_P, o.positionViewSpace);

    // o.positionViewSpace /= o.positionViewSpace.w;
    // o.positionViewSpace = float4(o.positionViewSpace.xy * 0.5 + 0.5, o.positionViewSpace.z, 1);
    // o.positionViewSpace = float4(a.xy/a.w, zDepth /20,1);

    // o.positionViewSpace = float4(i.position);
    // o.positionViewSpace = float4(1,1,1,1);
    return o;
    
}

#endif