#ifndef SSAO_RAW_PASS_INCLUDED
#define SSAO_RAW_PASS_INCLUDED

#include "Assets/DopeRP/GPU/HLSL/Common/Common.hlsl"

#if defined(_SAMPLES_COUNT16)
    #define SAMPLES_COUNT 16
#elif defined(_SAMPLES_COUNT32)
    #define SAMPLES_COUNT 32
#elif defined(_SAMPLES_COUNT64)
    #define SAMPLES_COUNT 64
#endif

static float3 samples[64] =
{
    float3(0.04977, -0.04471, 0.04996),
    float3(0.01457, 0.01653, 0.00224),
    float3(-0.04065, -0.01937, 0.03193),
    float3(0.01378, -0.09158, 0.04092),
    float3(0.05599, 0.05979, 0.05766),
    float3(0.09227, 0.04428, 0.01545),
    float3(-0.00204, -0.0544, 0.06674),
    float3(-0.00033, -0.00019, 0.00037),
    float3(0.05004, -0.04665, 0.02538),
    float3(0.03813, 0.0314, 0.03287),
    float3(-0.03188, 0.02046, 0.02251),
    float3(0.0557, -0.03697, 0.05449),
    float3(0.05737, -0.02254, 0.07554),
    float3(-0.01609, -0.00377, 0.05547),
    float3(-0.02503, -0.02483, 0.02495),
    float3(-0.03369, 0.02139, 0.0254),
    float3(-0.01753, 0.01439, 0.00535),
    float3(0.07336, 0.11205, 0.01101),
    float3(-0.04406, -0.09028, 0.08368),
    float3(-0.08328, -0.00168, 0.08499),
    float3(-0.01041, -0.03287, 0.01927),
    float3(0.00321, -0.00488, 0.00416),
    float3(-0.00738, -0.06583, 0.0674),
    float3(0.09414, -0.008, 0.14335),
    float3(0.07683, 0.12697, 0.107),
    float3(0.00039, 0.00045, 0.0003),
    float3(-0.10479, 0.06544, 0.10174),
    float3(-0.00445, -0.11964, 0.1619),
    float3(-0.07455, 0.03445, 0.22414),
    float3(-0.00276, 0.00308, 0.00292),
    float3(-0.10851, 0.14234, 0.16644),
    float3(0.04688, 0.10364, 0.05958),
    float3(0.13457, -0.02251, 0.13051),
    float3(-0.16449, -0.15564, 0.12454),
    float3(-0.18767, -0.20883, 0.05777),
    float3(-0.04372, 0.08693, 0.0748),
    float3(-0.00256, -0.002, 0.00407),
    float3(-0.0967, -0.18226, 0.29949),
    float3(-0.22577, 0.31606, 0.08916),
    float3(-0.02751, 0.28719, 0.31718),
    float3(0.20722, -0.27084, 0.11013),
    float3(0.0549, 0.10434, 0.32311),
    float3(-0.13086, 0.11929, 0.28022),
    float3(0.15404, -0.06537, 0.22984),
    float3(0.05294, -0.22787, 0.14848),
    float3(-0.18731, -0.04022, 0.01593),
    float3(0.14184, 0.04716, 0.13485),
    float3(-0.04427, 0.05562, 0.05586),
    float3(-0.02358, -0.08097, 0.21913),
    float3(-0.14215, 0.19807, 0.00519),
    float3(0.15865, 0.23046, 0.04372),
    float3(0.03004, 0.38183, 0.16383),
    float3(0.08301, -0.30966, 0.06741),
    float3(0.22695, -0.23535, 0.19367),
    float3(0.38129, 0.33204, 0.52949),
    float3(-0.55627, 0.29472, 0.3011),
    float3(0.42449, 0.00565, 0.11758),
    float3(0.3665, 0.00359, 0.0857),
    float3(0.32902, 0.0309, 0.1785),
    float3(-0.08294, 0.51285, 0.05656),
    float3(0.86736, -0.00273, 0.10014),
    float3(0.45574, -0.77201, 0.00384),
    float3(0.41729, -0.15485, 0.46251),
    float3 (-0.44272, -0.67928, 0.1865),
};

UNITY_INSTANCING_BUFFER_START(PerMaterialSSAO)

    // UNITY_DEFINE_INSTANCED_PROP(float, _RandomSize)
    UNITY_DEFINE_INSTANCED_PROP(float, _SampleRadius)
    UNITY_DEFINE_INSTANCED_PROP(float, _Bias)
    UNITY_DEFINE_INSTANCED_PROP(float, _Magnitude)
    UNITY_DEFINE_INSTANCED_PROP(float, _Contrast)

UNITY_INSTANCING_BUFFER_END(PerMaterialSSAO)


CBUFFER_START(SSAORaw)

    float4 _ScreenSize;
    float4x4 _LensProjection;
    float2 _NoiseScale;

CBUFFER_END


TEXTURE2D(_NoiseTexture);
SAMPLER(sampler_NoiseTexture);

TEXTURE2D(_PositionViewSpace);
SAMPLER(sampler_PositionViewSpace);

TEXTURE2D(_NormalViewSpace);
SAMPLER(sampler_NormalViewSpace);


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

    float4 fragPositionVS = SAMPLE_TEXTURE2D(_PositionViewSpace, sampler_PositionViewSpace, i.uv);

    // return fragPositionVS;
    // clip(fragPositionVS.a);

    float3 normalVS = normalize(SAMPLE_TEXTURE2D(_NormalViewSpace, sampler_NormalViewSpace, i.uv).xyz);
    // return float4(normalVS,1);

    float3 randomVec = float3(normalize(SAMPLE_TEXTURE2D(_NoiseTexture, sampler_NoiseTexture, i.uv * _NoiseScale) * 2.0f - 1.0f).xy,0);

    // return float4(randomVec,1);
    
    float3 tangent = normalize(randomVec - normalVS * dot(randomVec, normalVS));
    float3 binormal = cross(normalVS, tangent);
    float3x3 tbn = float3x3(tangent, binormal, normalVS);

    half occlusion = HALF_ZERO;

    UNITY_UNROLL
    for (int j = HALF_ZERO; j < SAMPLES_COUNT; j++)
    {
        float3 samplePositionVS = mul(tbn, samples[j]);
        samplePositionVS = fragPositionVS + samplePositionVS * _SampleRadius;
        // return float4(samplePositionVS,1);

        float4 offsetUV = mul(_LensProjection, samplePositionVS);
        offsetUV.xyz /= offsetUV.w;
        offsetUV.xy = offsetUV.xy * 0.5 + 0.5;

        float4 offsetPositionVS = SAMPLE_TEXTURE2D(_PositionViewSpace, sampler_PositionViewSpace, offsetUV.xy);

        // return float4(i.uv, 0,1);
        // return float4(offsetUV.xy, 0,1);
        // return fragPositionVS;

        float intensity = smoothstep(HALF_ZERO, HALF_ONE, _SampleRadius / abs(fragPositionVS.z - offsetPositionVS.z));
        occlusion += when_ge(offsetPositionVS.z, samplePositionVS.z + _Bias) * intensity;
        // occlusion += (offsetPositionVS.z >= samplePositionVS.z + _Bias ? 1.0 : 0.0) * intensity ;

        
    }

    occlusion /= SAMPLES_COUNT;
    // return occlusion;
    occlusion  = pow(occlusion, _Magnitude);
    occlusion  = _Contrast * (occlusion - 0.5) + 0.5;
                
    float4 fragColor = HALF_ONE - occlusion;  
    // return float4(random,1.0);

    // return tex2D(_NormalViewSpace, i.uv).xyzw;
    return fragColor;
    return occlusion;
    occlusion  = pow(occlusion, _Magnitude);
    occlusion  = _Contrast * (occlusion - HALF_HALF) + HALF_HALF;

    // float4 fragColor = HALF_ONE - occlusion;

    return fragColor;

}

#endif