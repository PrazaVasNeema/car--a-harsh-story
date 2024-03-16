#ifndef GBUFFER_PASS_INCLUDED
#define GBUFFER_PASS_INCLUDED

#include "../ShaderLibrary/Common.hlsl"



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
    float4 positionViewSpace : COLOR0;
    float4 normalViewSpace : COLOR1;
};

Interpolators vert(MeshData i)
{
    UNITY_SETUP_INSTANCE_ID(i);
    Interpolators o;
    UNITY_TRANSFER_INSTANCE_ID(i, o);
   
    o.position = TransformObjectToHClip(i.position);
    o.positionVS = TransformWorldToView(TransformObjectToWorld(i.position));
    o.normalVS = TransformWorldToViewNormal(TransformObjectToWorldNormal(i.normal));
    return o;
}

fragOutput frag(Interpolators i) 
{
    UNITY_SETUP_INSTANCE_ID(i);

    fragOutput o;

    i.normalVS = normalize(i.normalVS);
    
    o.positionViewSpace = float4(i.positionVS,1);
    o.normalViewSpace = float4(i.normalVS, 1);
    return o;
    
}

#endif