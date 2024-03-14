#ifndef CUSTOM_SSAO_INCLUDED
#define CUSTOM_SSAO_INCLUDED

#define NUM_SAMPLES 8
#define NUM_NOISE   4



CBUFFER_START(_SSAO)

    float4 _ssaoSamples[NUM_SAMPLES];
    float4 _ssaoNoise[NUM_NOISE];

    float4x4 lensProjection;

CBUFFER_END

TEXTURE2D(_SSAODepthNormalsAtlas);
SAMPLER(sampler_SSAODepthNormalsAtlas);
float2 _SSAODepthNormalsAtlas_TexelSize;



float3 CalculateSSAO(float2 fragCoord)
{
    float radius = 0.6;
    float bias = 0.005;
    float magnitude = 1.1;
    float contrast = 1.1;

    float4 position = SAMPLE_TEXTURE2D(_SSAODepthNormalsAtlas, sampler_SSAODepthNormalsAtlas, fragCoord);
    if (position.a <= 0) return float3(0, 0, 0);

    float3 normal = normalize(position.xyz);
    float3 random = _ssaoNoise[int(fragCoord.x * _SSAODepthNormalsAtlas_TexelSize.x) % NUM_NOISE];

    float3 tangent = normalize(random - normal * dot(random, normal));
    float3 binormal = cross(normal, tangent);
    float3x3 tbn = float3x3(tangent, binormal, normal);

    float occlusion = 0.0;
    for (int i = 0; i < NUM_SAMPLES; ++i) {
        float3 sample = mul(tbn, _ssaoSamples[i].xyz);
        sample = sample * radius + position.xyz;

        float4 offset = mul(lensProjection, float4(sample, 1.0));
        offset.xyz /= offset.w;
        offset.xy = offset.xy * 0.5 + 0.5;

        float sampleDepth = SAMPLE_TEXTURE2D(_SSAODepthNormalsAtlas, sampler_SSAODepthNormalsAtlas, offset.xy).w;
        float rangeCheck = smoothstep(0.0, 1.0, radius / abs(position.w - sampleDepth));
        occlusion += (sampleDepth >= sample.z + bias ? 1.0 : 0.0) * rangeCheck;
    }

    occlusion = 1.0 - (occlusion / NUM_SAMPLES);
    occlusion = pow(occlusion, magnitude);

    float3 a =occlusion * contrast + (1.0 - contrast) * occlusion;
    return a;
}

struct MeshData {
    float3 positionOS : POSITION;
    float2 uv         : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Interpolators {
    float4 positionCS : SV_POSITION;
    float2 uv         : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};


Interpolators vert(MeshData i)
{
    UNITY_SETUP_INSTANCE_ID(i);
    Interpolators o;
    UNITY_TRANSFER_INSTANCE_ID(i, o);
    o.positionCS = TransformWorldToHClip(TransformObjectToWorld(i.positionOS.xyz));
    o.uv = i.uv;
	
	
    return o;
}

float4 frag(Interpolators i) : SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(i);

    
    // return float4(1,1,0,1);
    // return float4(i.uv.xxx,1);
    float3 occlusionColor = CalculateSSAO(i.uv);
    return float4(occlusionColor, 1.0);
    return float4(CalculateSSAO(i.uv).xyz,1);
    
}

#endif