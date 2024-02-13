#ifndef CUSTOM_SHADOW_CASTER_PASS_INCLUDED
#define CUSTOM_SHADOW_CASTER_PASS_INCLUDED

#include "../ShaderLibrary/Common.hlsl"

TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);

CBUFFER_START(UnityPerMaterial)
    float4 _BaseColor;
    float4 _BaseMap_ST;
CBUFFER_END

struct Attributes {
    float3 positionOS : POSITION;
    float2 baseUV : TEXCOORD0;
};

struct Varyings {
    float4 positionCS : SV_POSITION;
    float2 baseUV : VAR_BASE_UV;
};

Varyings ShadowCasterPassVertex (Attributes input)
{
    Varyings output;
    float3 positionWS = TransformObjectToWorld(input.positionOS);
    output.positionCS = TransformWorldToHClip(positionWS);

    output.baseUV = input.baseUV * _BaseMap_ST.xy + _BaseMap_ST.zw;
    return output;
}

void ShadowCasterPassFragment (Varyings input)
{
    float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.baseUV);
    float4 base = baseMap * _BaseColor;
    
}

#endif
