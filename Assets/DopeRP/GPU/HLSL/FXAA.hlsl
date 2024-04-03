﻿#ifndef FXAA_PASS_INCLUDED
#define FXAA_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
#include "Assets/DopeRP/GPU/HLSL/PostFXStackPasses.hlsl"



#ifndef FXAA_REDUCE_MIN
    #define FXAA_REDUCE_MIN   (1.0/ 128.0)
#endif
#ifndef FXAA_REDUCE_MUL
    #define FXAA_REDUCE_MUL   (1.0 / 8.0)
#endif
#ifndef FXAA_SPAN_MAX
    #define FXAA_SPAN_MAX     8.0
#endif

float4 fxaa(float2 fragCoord, float2 resolution,
            float2 v_rgbNW, float2 v_rgbNE, 
            float2 v_rgbSW, float2 v_rgbSE, 
            float2 v_rgbM) {
    float4 color;
    float2 inverseVP = float2(1.0 / resolution.x, 1.0 / resolution.y);
    float3 rgbNW = GetSource(v_rgbNW).xyz;
    float3 rgbNE = GetSource(v_rgbNE).xyz;
    float3 rgbSW = GetSource(v_rgbSW).xyz;
    float3 rgbSE = GetSource(v_rgbSE).xyz;
    float4 texColor = GetSource(v_rgbM);
    float3 rgbM  = texColor.xyz;
    float3 luma = float3(0.299, 0.587, 0.114);
    float lumaNW = dot(rgbNW, luma);
    float lumaNE = dot(rgbNE, luma);
    float lumaSW = dot(rgbSW, luma);
    float lumaSE = dot(rgbSE, luma);
    float lumaM  = dot(rgbM,  luma);
    float lumaMin = min(lumaM, min(min(lumaNW, lumaNE), min(lumaSW, lumaSE)));
    float lumaMax = max(lumaM, max(max(lumaNW, lumaNE), max(lumaSW, lumaSE)));
    
    float2 dir;
    dir.x = -((lumaNW + lumaNE) - (lumaSW + lumaSE));
    dir.y =  ((lumaNW + lumaSW) - (lumaNE + lumaSE));
    
    float dirReduce = max((lumaNW + lumaNE + lumaSW + lumaSE) *
                          (0.25 * FXAA_REDUCE_MUL), FXAA_REDUCE_MIN);
    
    float rcpDirMin = 1.0 / (min(abs(dir.x), abs(dir.y)) + dirReduce);
    dir = min(float2(FXAA_SPAN_MAX, FXAA_SPAN_MAX),
              max(float2(-FXAA_SPAN_MAX, -FXAA_SPAN_MAX),
              dir * rcpDirMin)) * inverseVP;
    
    float3 rgbA = 0.5 * (
        GetSource(fragCoord + dir * (1.0 / 3.0 - 0.5)).xyz +
        GetSource(fragCoord + dir * (2.0 / 3.0 - 0.5)).xyz);
    float3 rgbB = rgbA * 0.5 + 0.25 * (
        GetSource(fragCoord + dir * -0.5).xyz +
        GetSource(fragCoord + dir * 0.5).xyz);

    float lumaB = dot(rgbB, luma);
    if ((lumaB < lumaMin) || (lumaB > lumaMax))
        color = float4(rgbA, texColor.a);
    else
        color = float4(rgbB, texColor.a);
    return color;
}

void texcoords(float2 fragCoord, float2 resolution,
            out float2 v_rgbNW, out float2 v_rgbNE,
            out float2 v_rgbSW, out float2 v_rgbSE,
            out float2 v_rgbM) {
    float2 inverseVP = 1.0 / resolution.xy;
    v_rgbNW = fragCoord + float2(-1.0, -1.0) * inverseVP;
    v_rgbNE = fragCoord + float2(1.0, -1.0) * inverseVP;
    v_rgbSW = fragCoord + float2(-1.0, 1.0) * inverseVP;
    v_rgbSE = fragCoord + float2(1.0, 1.0) * inverseVP;
    v_rgbM = float2(fragCoord );
}

float4 apply(float2 fragCoord, float2 resolution) {
    float2 v_rgbNW;
    float2 v_rgbNE;
    float2 v_rgbSW;
    float2 v_rgbSE;
    float2 v_rgbM;

    //compute the texture coords
     texcoords(fragCoord, resolution, v_rgbNW, v_rgbNE, v_rgbSW, v_rgbSE, v_rgbM);

    // return float4(v_rgbNW ,0,1);

	// return GetSource(v_rgbNW);
    //compute FXAA
    return fxaa(fragCoord, resolution, v_rgbNW, v_rgbNE, v_rgbSW, v_rgbSE, v_rgbM);
}
float4 _ScreenSize;
float4 FXAAPassFragment(Varyings input) : SV_TARGET
{
    float4 color = apply(input.screenUV, float2(_ScreenSize.x, _ScreenSize.y));
    
    return color;
}

#endif