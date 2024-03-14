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



float3 CalculateSSAO(float2 uv, float2 fragCoofrd)
{
    float radius = 0.6;
    float bias = 0.005;
    float magnitude = 0.5;
    float contrast = 0.5;

    
    float4 position = SAMPLE_TEXTURE2D(_SSAODepthNormalsAtlas, sampler_SSAODepthNormalsAtlas, uv);

    
    if (position.w <= 0) return float3(1, 1, 1);
    

    float3 normal = normalize(position.xyz);

     int  noiseS = int(sqrt(NUM_NOISE));
    int  noiseX = int(fragCoofrd.x - 0.5) % noiseS;
    int  noiseY = int(fragCoofrd.y - 0.5) % noiseS;
    float3 random = _ssaoNoise[noiseX + (noiseY * noiseS)];

    // float3 random = _ssaoNoise[int(fragCoord.x * _SSAODepthNormalsAtlas_TexelSize.x) % NUM_NOISE];

    float3 tangent = normalize(random - normal * dot(random, normal));
    float3 binormal = cross(normal, tangent);
    float3x3 tbn = float3x3(tangent, binormal, normal);

    float occlusion = NUM_SAMPLES;
    
    for (int i = 0; i < NUM_SAMPLES; ++i) {
        float3 sample = mul(tbn, _ssaoSamples[i]);
        sample = sample * radius + float3(uv, position.w);



        float4 offsetUV      = float4(sample, 1.0);
        offsetUV      = mul(lensProjection, offsetUV);
        offsetUV.xyz /= offsetUV.w;
        offsetUV.xy   = offsetUV.xy * 0.5 + 0.5;

        float4 offsetPosition = SAMPLE_TEXTURE2D(_SSAODepthNormalsAtlas, sampler_SSAODepthNormalsAtlas, offsetUV.xy);
        
        float occluded = 0;
        if   (sample.z + bias <= offsetPosition.z)
        { occluded = 0; }
        else { occluded = 1; }

        float intensity = smoothstep( 0, 1,   radius/ abs(position.z - offsetPosition.z));
        occluded  *= intensity;
        occlusion -= occluded;
    }

    occlusion /= NUM_SAMPLES;
    occlusion  = pow(occlusion, magnitude);
    occlusion  = contrast * (occlusion - 0.5) + 0.5;
    
    float4 fragColor = float4(float3(occlusion.xxx), position.a);
    return fragColor;
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
    float3 occlusionColor = CalculateSSAO(i.uv, i.positionCS);
    return float4(occlusionColor, 1.0);
    // return float4(CalculateSSAO(i.uv).xyz,1);
    
}

#endif