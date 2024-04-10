#ifndef SSR_RAW_PASS_INCLUDED
#define SSR_RAW_PASS_INCLUDED

#include "Assets/DopeRP/GPU/HLSL/Common/Common.hlsl"

#define ITER_COUNT 2

TEXTURE2D(_G_AlbedoAtlas);
SAMPLER(sampler_G_AlbedoAtlas);

TEXTURE2D(_SSRRawAtlas);
SAMPLER(sampler_SSRRawAtlas);


CBUFFER_START(SSRColor)

    float4 _ScreenSize;
float2 _NearFarPlanes;
float4x4 _Matrix_V;
    float4x4 _Matrix_P;
float4x4 _Matrix_I_P;


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
  
    float4 fragColor = 0;
  
    int   size       = 6;
    float separation = 2.0;

    // float2 texSize  = textureSize(uvTexture, 0).xy;
    float2 texCoord = i.uv;

    float4 uv = SAMPLE_TEXTURE2D(_SSRRawAtlas, sampler_SSRRawAtlas, texCoord);
float4 a = i.positionSV;
    // Removes holes in the UV map.
    if (uv.b <= 0.0) {
        uv    = 0;
        float count = 0.0;

        for (int i = -size; i <= size; ++i) {
            for (int j = -size; j <= size; ++j) {
                uv    += SAMPLE_TEXTURE2D( _SSRRawAtlas, sampler_SSRRawAtlas, ( (float2(i, j) * separation)+ a.xy) / _ScreenSize.xy);
                count += 1.0;
            }
        }

        uv.xyz /= count;
    }

    if (uv.b <= 0.0) { fragColor = 0; return fragColor;}

    float4  color = SAMPLE_TEXTURE2D(_G_AlbedoAtlas, sampler_G_AlbedoAtlas, uv.xy);
    float alpha = clamp(uv.b, 0.0, 1.0);

    fragColor = float4(lerp(float3(0, 0, 0), color.rgb, alpha), alpha);

    return fragColor;
    
}

#endif