#ifndef CUSTOM_SHADOWS_INCLUDED
#define CUSTOM_SHADOWS_INCLUDED

#include "../ShaderLibrary/Common.hlsl"
#include "../S_/SurfaceData.hlsl"
#include "../S_/S_BRDF.hlsl"
#include "../S_/CommonMaterial.hlsl"

#define MAX_SHADOWED_DIRECTIONAL_LIGHT_COUNT 1
#define MAX_CASCADE_COUNT 4

TEXTURE2D_SHADOW(_DirectionalShadowAtlas);
#define SHADOW_SAMPLER sampler_linear_clamp_compare
SAMPLER_CMP(SHADOW_SAMPLER);

CBUFFER_START(_CustomShadows)
    int _CascadeCount;
float4 _CascadeCullingSpheres[MAX_CASCADE_COUNT];
float4 _CascadeData[MAX_CASCADE_COUNT];
float4x4 _DirectionalShadowMatrices
    [MAX_SHADOWED_DIRECTIONAL_LIGHT_COUNT * MAX_CASCADE_COUNT];
float4 _ShadowAtlasSize;
float4 _ShadowDistanceFade;
CBUFFER_END

struct ShadowData {
    int cascadeIndex;
    float cascadeBlend;
    float strength;
    float3 color;
};

struct DirectionalShadowData {
    float strength;
    float normalBias;
};

float FadedShadowStrength (float distance, float scale, float fade) {
    return saturate((1.0 - distance * scale) * fade);
}

bool CalculateCascadeIter(out ShadowData shadowData, int i)
{
    
}

ShadowData GetShadowData (SurfaceData surfaceData) {
    ShadowData data;
    data.cascadeBlend = 1.0;
    data.strength = FadedShadowStrength(surfaceData.depth, _ShadowDistanceFade.x, _ShadowDistanceFade.y);
    
    int i = 0;
    float4 sphere;
    float distanceSqr;
    float fade;
    
    float checkIter;
    float forLoopShouldContinue = 1;

    for (i = 0; i < _CascadeCount; i++) {
        float4 sphere = _CascadeCullingSpheres[i];
        float distanceSqr = DistanceSquared(surfaceData.positionWS, sphere.xyz);
        if (distanceSqr < sphere.w) {
            float fade = FadedShadowStrength(distanceSqr, _CascadeData[i].x, _ShadowDistanceFade.z);
            if (i == _CascadeCount - 1) {
                data.strength *= fade;
            }
            else {
                data.cascadeBlend = fade;
            }
            break;
        }
    }
    
    // #if defined(CASCEDE_COUNT_4) || defined(CASCEDE_COUNT_2)
    //
    sphere = _CascadeCullingSpheres[0];
    distanceSqr = DistanceSquared(surfaceData.positionWS, sphere.xyz);
    
    checkIter = when_lt(distanceSqr, sphere.w) * forLoopShouldContinue;

    forLoopShouldContinue = checkIter;
    
    fade = checkIter * FadedShadowStrength(distanceSqr, _CascadeData[0].x, _ShadowDistanceFade.z);
    
    data.strength *= 1 + (data.strength - 1) * when_eq(0, _CascadeCount - 1);;
    data.cascadeBlend = data.cascadeBlend * checkIter + fade * when_neq(0, _CascadeCount - 1);
    
    i += 1 * checkIter;
    
    // //---
    //
    // sphere = _CascadeCullingSpheres[1];
    // distanceSqr = DistanceSquared(surfaceData.positionWS, sphere.xyz);
    //
    // checkIter = when_lt(distanceSqr, sphere.w);
    //
    // fade = FadedShadowStrength(distanceSqr, _CascadeData[1].x, _ShadowDistanceFade.z) * checkIter;
    //
    // data.strength *= 1 + (data.strength - 1) * when_eq(0, _CascadeCount - 1);;
    // data.cascadeBlend = data.cascadeBlend * checkIter + fade * when_neq(0, _CascadeCount - 1);
    //
    // i += 1 * checkIter;
    //
    //
    // #endif
    //
    // //--- ---
    //
    // #if defined(CASCEDE_COUNT_4)
    //
    // sphere = _CascadeCullingSpheres[2];
    // distanceSqr = DistanceSquared(surfaceData.positionWS, sphere.xyz);
    //
    // checkIter = when_lt(distanceSqr, sphere.w);
    //
    // fade = FadedShadowStrength(distanceSqr, _CascadeData[2].x, _ShadowDistanceFade.z) * checkIter;
    //
    // data.strength *= 1 + (data.strength - 1) * when_eq(0, _CascadeCount - 1);;
    // data.cascadeBlend = data.cascadeBlend * checkIter + fade * when_neq(0, _CascadeCount - 1);
    //
    // i += 1 * checkIter;
    // //---
    //
    // sphere = _CascadeCullingSpheres[3];
    // distanceSqr = DistanceSquared(surfaceData.positionWS, sphere.xyz);
    //
    // checkIter = when_lt(distanceSqr, sphere.w);
    //
    // fade = FadedShadowStrength(distanceSqr, _CascadeData[3].x, _ShadowDistanceFade.z) * checkIter;
    //
    // data.strength *= 1 + (data.strength - 1) * when_eq(0, _CascadeCount - 1);;
    // data.cascadeBlend = data.cascadeBlend * checkIter + fade * when_neq(0, _CascadeCount - 1);
    //
    // i += 1 * checkIter;

    // #endif

    if (i == _CascadeCount) {
        data.strength = 0.0;
    }
    // data.strength = data.strength * when_neq(i, _CascadeCount);

    // Тут идут бленды
    
    data.cascadeIndex = i;
    
    switch (i)
    {
    case 0:
        data.color = float3(1,0,0);
        break;
    case 1:
        data.color = float3(0,1,0);
        break;
    case 2:
        data.color = float3(0,0,1);
        break;
    case 3:
        data.color = float3(1,1,0);
        break;
    case 4:
        data.color = float3(1,1,1);
        break;
    }
    
    return data;
}

float SampleDirectionalShadowAtlas (float3 positionSTS) {
    return SAMPLE_TEXTURE2D_SHADOW(_DirectionalShadowAtlas, SHADOW_SAMPLER, positionSTS);
}

float GetDirectionalShadowAttenuation (
    DirectionalShadowData directional, ShadowData global, SurfaceData surfaceData
) {
    // #if !defined(_RECEIVE_SHADOWS)
    // return 1.0;
    // #endif
    if (directional.strength <= 0.0) {
        return 1.0;
    }
    float3 normalBias = surfaceData.normal * (directional.normalBias * _CascadeData[global.cascadeIndex].y);
    float3 positionSTS = mul(_DirectionalShadowMatrices[global.cascadeIndex], float4(surfaceData.positionWS + normalBias, 1.0)).xyz;
    //float shadow = FilterDirectionalShadow(positionSTS);
    float shadow = SampleDirectionalShadowAtlas(positionSTS);
    if (global.cascadeBlend < 1.0) {
        normalBias = surfaceData.normal * (directional.normalBias * _CascadeData[global.cascadeIndex + 1].y);
        positionSTS = mul(_DirectionalShadowMatrices[global.cascadeIndex + 1],float4(surfaceData.positionWS + normalBias, 1.0)).xyz;
        //shadow = lerp(FilterDirectionalShadow(positionSTS), shadow, global.cascadeBlend);
    }
    return lerp(1.0, shadow, directional.strength);
}

#endif