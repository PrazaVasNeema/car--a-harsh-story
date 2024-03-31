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

    UNITY_DEFINE_INSTANCED_PROP(float, _SampleRadius)
    UNITY_DEFINE_INSTANCED_PROP(float, _Bias)
    UNITY_DEFINE_INSTANCED_PROP(float, _Magnitude)
    UNITY_DEFINE_INSTANCED_PROP(float, _Contrast)

UNITY_INSTANCING_BUFFER_END(PerMaterialSSAO)


CBUFFER_START(SSAORaw)

    float4 _ScreenSize;
    float4x4 _LensProjection;
    float2 _NoiseScale;

    float2 _nearFarPlanes;
    float4x4 adfgdgf_WorldToCameraMatrix;
float4x4 adfgdgf_CameraToWorldMatrix;

float4x4 _INVERSE_P;

float4 SAMPLES[64];

CBUFFER_END


TEXTURE2D(_NoiseTexture);
SAMPLER(sampler_NoiseTexture);

TEXTURE2D(_PositionViewSpace);
SAMPLER(sampler_PositionViewSpace);

TEXTURE2D(_NormalViewSpace);
SAMPLER(sampler_NormalViewSpace);

TEXTURE2D(Test);
SAMPLER(samplerTest);

struct MeshData
{
    float4 position : POSITION;
    float2 uv : TEXCOORD0;

    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Interpolators
{
    float4 positionSV : SV_POSITION;
    float2 uv : TEXCOORD0;

    float3 camRelativeWorldPos : TEXCOORD1;

    UNITY_VERTEX_INPUT_INSTANCE_ID
};


Interpolators vert (MeshData i)
{
    UNITY_SETUP_INSTANCE_ID(i);
    Interpolators o;
    UNITY_TRANSFER_INSTANCE_ID(i, o);

    o.uv = i.uv;
    o.positionSV = TransformObjectToHClip(i.position);

    o.camRelativeWorldPos = mul(unity_ObjectToWorld, float4(i.position.xyz, 1.0)).xyz - _WorldSpaceCameraPos;
    return o;
}

// This function reconstructs the view space position from the depth value
float3 ReconstructViewSpacePosition(float2 uv, float depth)
{
    // Convert UV and depth into a homogeneous clip space position
    float4 clipSpacePosition = float4(uv, depth, 1.0);
    // Transform clip space position back to view space
    // float4 viewSpacePosition = mul(clipSpacePosition, _LensProjection);
    // Perspective division
    // viewSpacePosition /= viewSpacePosition.w;
    // return viewSpacePosition.xyz;
    return clipSpacePosition.xyz;
}

// Linearizes a Z buffer value
float CalcLinearZ(float depth, float zNear, float zFar) {


    // bias it from [0, 1] to [-1, 1]
    float lineara = zNear / (zFar - depth * (zFar - zNear)) * zFar;

    return lineara;
}

// this is supposed to get the world position from the depth buffer
float3 WorldPosFromDepth(float depth, float2 TexCoord) {
    float z = depth -1000000;

    float4 clipSpacePosition = float4(TexCoord * 2.0 - 1.0, z, 1.0);

    // return clipSpacePosition;
    float4 viewSpacePosition = mul(_INVERSE_P, clipSpacePosition);
    viewSpacePosition.z = viewSpacePosition.z;
    // Perspective division
    viewSpacePosition /= viewSpacePosition.w;
    // return viewSpacePosition.xyz;
    
    float4 worldSpacePosition = mul(adfgdgf_CameraToWorldMatrix, viewSpacePosition);
    
    return worldSpacePosition.xyz;
}

float4 frag (Interpolators i) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(i);

    float4 fragPositionVS = SAMPLE_TEXTURE2D(_PositionViewSpace, sampler_PositionViewSpace, i.uv);
    float3 normalVSf = normalize(SAMPLE_TEXTURE2D(_NormalViewSpace, sampler_NormalViewSpace, i.uv).xyz);
    float4 normalVSNOT = SAMPLE_TEXTURE2D(_NormalViewSpace, sampler_NormalViewSpace, i.uv);
    // return normalVSNOT;
    // return mul(adfgdgf_CameraToWorldMatrix, fragPositionVS);
    // return float4(TransformViewToWorld(fragPositionVS), 1);
// return fragPositionVS;
    float depth = SAMPLE_TEXTURE2D(Test, samplerTest, i.uv).r;

    depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, depth);

    // zBufferParam = { (f-n)/n, 1, (f-n)/n*f, 1/f }
    float n = _nearFarPlanes.x;
    float f = _nearFarPlanes.y;
    
    float4 zBufferParam = float4((f-n)/n, 1, (f-n)/n*f, 1/f);

    float sceneZ = LinearEyeDepth(depth, zBufferParam);
    sceneZ =CalcLinearZ(depth, n, f);

    float4 clipSpacePosition = float4((i.uv * 2.0 - 1.0) * sceneZ/depth, sceneZ, 1.0 * sceneZ/depth);

    float4 viewSpacePosition = mul(_INVERSE_P, clipSpacePosition);

    viewSpacePosition /= viewSpacePosition.w;

    float4 worldSpacePosition = mul(adfgdgf_CameraToWorldMatrix, viewSpacePosition);

    // return worldSpacePosition;


    // return viewSpacePosition;


    // return float4(clipSpacePosition.xyz,1);

    float3 viewPlane = i.camRelativeWorldPos.xyz / dot(i.camRelativeWorldPos.xyz, adfgdgf_WorldToCameraMatrix._m20_m21_m22);

    float3 worldPos = viewPlane * sceneZ + _WorldSpaceCameraPos;
    worldPos = mul(adfgdgf_CameraToWorldMatrix, float4(worldPos, 1.0));

    float4 col = 0;
    col.rgb = saturate(2.0 - abs(frac(worldPos) * 2.0 - 1.0) * 100.0);

    float3 wp2 = ComputeWorldSpacePosition(i.uv, depth, mul(Inverse(GetViewToHClipMatrix()), UNITY_MATRIX_I_V));

    // return mul(UNITY_MATRIX_I_V, fragPositionVS);

    // return float4(wp2, 1);
    
    // return col;
    // return float4(worldPos, 1);

    float3 ab = WorldPosFromDepth(depth, i.uv);

    // return float4(ab, 1);


    float3 testReconstruct = ReconstructViewSpacePosition(i.uv, depth);
    // return float4(testReconstruct,1);
    // return float4(depth.xxx, 1);

    // float4 fragPositionVS = SAMPLE_TEXTURE2D(_PositionViewSpace, sampler_PositionViewSpace, i.uv);
// return fragPositionVS;
    fragPositionVS = viewSpacePosition;
// return fragPositionVS;  
    // clip(fragPositionVS.a);

    float3 normalVS = normalize(SAMPLE_TEXTURE2D(_NormalViewSpace, sampler_NormalViewSpace, i.uv).xyz);

    float2 noiseUV = float2(float(100)/float(_NoiseScale.x),
                    float(100)/float(_NoiseScale.y))
                    * i.uv * 1;
    float3 randomVec = float3(normalize(SAMPLE_TEXTURE2D(_NoiseTexture, sampler_NoiseTexture, i.uv * _NoiseScale) * 2 - 1).xy,0);
    
    // return float4(normalVS, 1);
    
    float3 tangent = normalize(randomVec - normalVS * dot(randomVec, normalVS));
    float3 binormal = cross(normalVS, tangent);
    float3x3 tbn = float3x3(tangent, binormal, normalVS);

    half occlusion = HALF_ZERO;

    UNITY_UNROLL
    for (int j = HALF_ZERO; j < SAMPLES_COUNT; j++)
    {
        float3 samplePositionVS = mul(tbn, samples[j]);
        samplePositionVS = fragPositionVS + samplePositionVS * _SampleRadius ;
        // samplePositionVS = fragPositionVS +  mul(tbn, float3(0.05,0.05,0.1));

        float4 offsetUV = mul(_LensProjection, samplePositionVS);
        // return float4(offsetUV);

        offsetUV.xyz /= offsetUV.w;
        offsetUV.xy = offsetUV.xy * 0.5 + 0.5;


        float4 fragPositionVS2 = SAMPLE_TEXTURE2D(_PositionViewSpace, sampler_PositionViewSpace, offsetUV.xy);

        float offsetPositionDEPTH = SAMPLE_TEXTURE2D(Test, samplerTest, offsetUV.xy).r;
        float3 sampleNormalVS = normalize(SAMPLE_TEXTURE2D(_NormalViewSpace, sampler_NormalViewSpace, offsetUV.xy).xyz);

        if(dot(sampleNormalVS, normalVS) > 0.99)
            continue;
        
        offsetPositionDEPTH = lerp(UNITY_NEAR_CLIP_VALUE, 1, offsetPositionDEPTH);
        sceneZ =CalcLinearZ(offsetPositionDEPTH, n, f);

        


        clipSpacePosition = float4((offsetUV.xy * 2.0 - 1.0) * sceneZ/offsetPositionDEPTH, sceneZ, 1.0 * sceneZ/offsetPositionDEPTH);

        float4 viewSpacePosition2 = mul(_INVERSE_P, clipSpacePosition);

        viewSpacePosition2 /= viewSpacePosition2.w;
        // return fragPositionVS;
        // return float4(samplePositionVS.xy,viewSpacePosition2.z ,1);
        // return float4(samplePositionVS.xyz,1);
        // return float4(fragPositionVS2.xyz,1);
        // return float4( samplePositionVS.x - viewSpacePosition2.x, viewSpacePosition2.y   > samplePositionVS.y ,samplePositionVS.z ,1);

        // return float4(viewSpacePosition2.xyz, 1);
        // return viewSpacePosition2;

        float intensity = smoothstep(HALF_ZERO, HALF_ONE, _SampleRadius / abs(samplePositionVS.z - viewSpacePosition2.z));
        occlusion += when_ge(viewSpacePosition2.z, samplePositionVS.z + _Bias) * intensity;

        // occlusion += when_ge(offsetPositionDEPTH , offsetUV.z + _Bias);
        // return float4(occlusion.xxx,1);
    }




    float y;
    float x;
    float sum;
    float2 offset;
    for (y = -0.5 * 3; y <= 0.5 * 3; y +=1)
        for (x = -0.5 * 3; x <= 0.5 * 3; x +=1)
        {
            offset = float2(1400 * x, 700 * y);
            float3 normalVS2 = normalize(SAMPLE_TEXTURE2D(_NormalViewSpace, sampler_NormalViewSpace, i.uv + offset).xyz);
            if (abs(normalVS.z - normalVS2.z) < 0.01)
                sum++;
        }

sum *= 0.0625;
    
    occlusion /= SAMPLES_COUNT;
    occlusion  = pow(occlusion, _Magnitude);
    occlusion  = _Contrast * (occlusion - 0.5) + 0.5;
                
    float4 fragColor = HALF_ONE - max(occlusion, 0);  
    
    return fragColor;
     
}

#endif