#ifndef POST_FX_PASSES_INCLUDED
#define POST_FX_PASSES_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
#include "Assets/DopeRP/GPU/HLSL/PostFXStackPasses.hlsl"

TEXTURE2D(_PostFXSource);
TEXTURE2D(_PostFXSource2);
SAMPLER(sampler_linear_clamp);

float4 _PostFXSource_TexelSize;

float4 GetSourceTexelSize () {
    return _PostFXSource_TexelSize;
}

float4 GetSource(float2 screenUV) {
    return SAMPLE_TEXTURE2D_LOD(_PostFXSource, sampler_linear_clamp, screenUV, 0);
}

float4 GetSource2(float2 screenUV) {
    return SAMPLE_TEXTURE2D_LOD(_PostFXSource2, sampler_linear_clamp, screenUV, 0);
}

struct Varyings {
    float4 positionCS : SV_POSITION;
    float2 screenUV : VAR_SCREEN_UV;
};

Varyings DefaultPassVertex (uint vertexID : SV_VertexID) {
    Varyings output;
    output.positionCS = float4(
        vertexID <= 1 ? -1.0 : 3.0,
        vertexID == 1 ? 3.0 : -1.0,
        0.0, 1.0
    );
    output.screenUV = float2(
        vertexID <= 1 ? 0.0 : 2.0,
        vertexID == 1 ? 2.0 : 0.0
    );
    if (_ProjectionParams.x < 0.0) {
        output.screenUV.y = 1.0 - output.screenUV.y;
    }
    return output;
}



float4 CopyPassFragment (Varyings input) : SV_TARGET {
    return GetSource(input.screenUV);
}

// float3 ColorGrade (float3 color) {
//     color = min(color, 60.0);
//     return color;
// }
//
// float4 ToneMappingNonePassFragment (Varyings input) : SV_TARGET {
//     float4 color = GetSource(input.screenUV);
//     color.rgb = ColorGrade(color.rgb);
//     return color;
// }

float4 _ColorAdjustments;
float4 _ColorFilter;

float3 ColorGradePostExposure (float3 color) {
    return color * _ColorAdjustments.x;
}

float3 ColorGrade (float3 color) {
    color = min(color, 60.0);
    color = ColorGradePostExposure(color);
    return color;
}

float4 ToneMappingNonePassFragment (Varyings input) : SV_TARGET {
    float4 color = GetSource(input.screenUV);
    color.rgb = ColorGrade(color.rgb);
    return color;
}


float4 ToneMappingACESPassFragment (Varyings input) : SV_TARGET {
    float4 color = GetSource(input.screenUV);
    color.rgb = ColorGrade(color.rgb);
    color.rgb = AcesTonemap(unity_to_ACES(color.rgb));
    return color;
}

float4 ToneMappingNeutralPassFragment (Varyings input) : SV_TARGET {
    float4 color = GetSource(input.screenUV);
    color.rgb = ColorGrade(color.rgb);
    color.rgb = NeutralTonemap(color.rgb);
    return color;
}

float4 ToneMappingReinhardPassFragment (Varyings input) : SV_TARGET {
    float4 color = GetSource(input.screenUV);
    color.rgb = ColorGrade(color.rgb);
    color.rgb /= color.rgb + 1.0;
    return color;
}

#endif