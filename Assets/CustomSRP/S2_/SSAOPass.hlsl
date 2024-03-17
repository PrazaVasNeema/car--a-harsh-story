#ifndef SSAO_PASS_INCLUDED
#define SSAO_PASS_INCLUDED

#include "../ShaderLibrary/Common.hlsl"

TEXTURE2D(_PositionViewSpace);
SAMPLER(sampler_PositionViewSpace);

TEXTURE2D(_NormalViewSpace);
SAMPLER(sampler_NormalViewSpace);

TEXTURE2D(_NormalMapSSAO);
SAMPLER(sampler_NormalMapSSAO);

CBUFFER_START(SSAO)
    float _randomSize;
    float _Radius;
    float _Contrast;
    float _Magnitude;
    float _Bias;
    float2 _ScreenSize;
CBUFFER_END

struct MeshData {
    float4 position : POSITION;
    float2 uv   : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Interpolators {
    float4 position : SV_POSITION;
    float2 uv   : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};


Interpolators vert(MeshData i)
{
    UNITY_SETUP_INSTANCE_ID(i);
    Interpolators o;
    UNITY_TRANSFER_INSTANCE_ID(i, o);

    o.uv = i.uv;
    o.position = TransformWorldToHClip(TransformObjectToWorld(i.position.xyz));;
    
    return o;
}

float3 getPosition(in float2 uv) {
    return  SAMPLE_TEXTURE2D(_PositionViewSpace, sampler_PositionViewSpace, uv).xyz;
}

float3 getNormal(in float2 uv) {
    return normalize(SAMPLE_TEXTURE2D(_NormalViewSpace, sampler_NormalViewSpace, uv).xyz * 2.0f - 1.0f);
}

float2 getRandom(in float2 uv) {
    // return float2(1,1);
    return normalize(SAMPLE_TEXTURE2D(_NormalMapSSAO, sampler_NormalMapSSAO, _ScreenSize.xy * uv / _randomSize).xy * 2.0f - 1.0f); 
}

float doAmbientOcclusion(in float2 tcoord,in float2 uv, in float3 p, in float3 cnorm) 
{float3 diff = getPosition(tcoord + uv) - p; 
    const float3 v = normalize(diff); 
    const float d = length(diff)*_Magnitude;
    // return p.z > getPosition(tcoord + uv);
    return max(0.0,dot(cnorm,v)-_Bias)*(1.0/(1.0+d))*_Contrast;
}

float4 frag(Interpolators i) : SV_TARGET 
{
    UNITY_SETUP_INSTANCE_ID(i);
// return 1;
    float4 color = 1;

    const float2 vec[8] = {float2(1,0),float2(-1,0), float2(0,1),float2(0,-1),
    float2(1,1),float2(-1,-1), float2(0.5,1),float2(1,0.5)};
    float3 p = getPosition(i.uv); 
    float3 n = getNormal(i.uv); 
    float2 rand = getRandom(i.uv); 
    float ao = 0; 
    float rad = _Radius/p.z;
    // return float4(rad.xxx,1);
    // return float4(getNormal(i.uv).xyz,1);
    // return float4(i.uv.x, i.uv.y, 0, 1);
    int iterations = 8;
    for (int j = 0; j < iterations; ++j) 
    {
        float2 coord1 = reflect(vec[j],rand)*rad; 
        float2 coord2 = float2(coord1.x*0.707 - coord1.y*0.707, coord1.x*0.707 + coord1.y*0.707); 

        // return float4(coord1,coord2);
        
        ao += doAmbientOcclusion(i.uv,coord1*0.25, p, n); 
        ao += doAmbientOcclusion(i.uv,coord2*0.5, p, n); 
        ao += doAmbientOcclusion(i.uv,coord1*0.75, p, n); 
        ao += doAmbientOcclusion(i.uv,coord2, p, n); 
    }
    // return float4(getNormal(i.uv),1);
    // return float4(SAMPLE_TEXTURE2D(_NormalMapSSAO, sampler_NormalMapSSAO, _gScreenSize.xy * i.uv / _randomSize).xy ,1,1);
    // return getNormal(i.uv).xyzx;
// return ao;
    ao/=(float)iterations*4.0;
    // return float4(getRandom(i.uv),0,1);
    return float4(color - ao.xxx, 1);
}

#endif