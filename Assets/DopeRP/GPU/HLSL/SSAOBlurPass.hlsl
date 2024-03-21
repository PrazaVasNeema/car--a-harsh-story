#ifndef SSAO_PASS2_BLUR_INCLUDED
#define SSAO_PASS2_BLUR_INCLUDED

#include "Assets/DopeRP/GPU/HLSL/Common/Common.hlsl"

TEXTURE2D(_SSAORawAtlas);
SAMPLER(sampler_SSAORawAtlas);


CBUFFER_START(SSAOBlur)

    float4 _ScreenSize;

CBUFFER_END


struct MeshData
{
    float4 position : POSITION;
    float2 uv : TEXCOORD0;
};

struct Interpolators
{
    float4 positionSV : SV_POSITION;
    float2 uv : TEXCOORD0;
};


Interpolators vert (MeshData i)
{
    Interpolators o;
    o.uv = i.uv;
    o.positionSV = TransformObjectToHClip(i.position);
    return o;
}


float4 frag (Interpolators i) : SV_Target
{
    float2 texelSize = 1/_ScreenSize.xy;
    float result = 0;
    for (int x = -2; x < 2; ++x) 
    {
        for (int y = -2; y < 2; ++y) 
        {
            float2 offset = float2(float(x), float(y)) * texelSize.xy;
            result += SAMPLE_TEXTURE2D(_SSAORawAtlas, sampler_SSAORawAtlas, i.uv + offset).r;
        }
    }
    float4 FragColor = result / (4.0 * 4.0);
    return FragColor;
    
}

#endif