#ifndef CUSTOM_SSAO_INCLUDED
#define CUSTOM_SSAO_INCLUDED

#define NUM_SAMPLES 8
#define NUM_NOISE   4

CBUFFER_START(_SSAO)

    float4 _ssaoSamples[NUM_SAMPLES];
    float4 _ssaoNoise[NUM_NOISE];

    float4x4 lensProjection;

CBUFFER_END

TEXTURE2D(_ColorBufferAtlas);
SAMPLER(sampler_ColorBufferAtlas);
float2 _ColorBufferAtlas_TexelSize;

TEXTURE2D(_NormalBufferAtlas);
SAMPLER(sampler_NormalBufferAtlas);



float3 CalculateSSAO(float4 fragCoord)
{
    float radius    = 0.6;
    float bias      = 0.005;
    float magnitude = 1.1;
    float contrast  = 1.1;

    float4 fragColor = 1;

    float2 texSize = _ColorBufferAtlas_TexelSize;
    float2 texCoord = fragCoord.xy / texSize;

    float4 position = SAMPLE_TEXTURE2D(_ColorBufferAtlas, sampler_ColorBufferAtlas, texCoord);

    if (position.a <= 0) { return 1; }

    float3 normal = normalize(SAMPLE_TEXTURE2D(_NormalBufferAtlas, sampler_NormalBufferAtlas, texCoord).xyz);

    
    int  noiseS = int(sqrt(NUM_NOISE));
    int  noiseX = int(texCoord.x - 0.5) % noiseS;
    int  noiseY = int(texCoord.y - 0.5) % noiseS;
    float3 random = _ssaoNoise[noiseX + (noiseY * noiseS)];

    float3 tangent = normalize(random - normal * dot(random, normal));
    float3 binormal = cross(normal, tangent);

    float3x3 tbn = float3x3(tangent, binormal, normal);


    float occlusion = NUM_SAMPLES;

    for (int i = 0; i < NUM_SAMPLES; ++i)
    {

        float3 samplePosition = mul(tbn, _ssaoSamples[i]);
        samplePosition = position.xyz + samplePosition * radius;

        float4 offsetUV = float4(samplePosition, 1);
        // offsetUV      = lensProjection * offsetUV;
        offsetUV      = mul(lensProjection, offsetUV);
        offsetUV.xyz /= offsetUV.w;
        offsetUV.xy   = offsetUV.xy * 0.5 + 0.5;

        float4 offsetPosition = SAMPLE_TEXTURE2D(_ColorBufferAtlas, sampler_ColorBufferAtlas, offsetUV.xy);

        float occluded = 0;
        if   (samplePosition.y + bias <= offsetPosition.y)
        { occluded = 0; }
        else { occluded = 1; }

        float intensity =
        smoothstep( 0, 1,   radius/ abs(position.y - offsetPosition.y));

        occluded  *= intensity;
        occlusion -= occluded;
    }

    occlusion /= NUM_SAMPLES;
    occlusion  = pow(occlusion, magnitude);
    occlusion  = contrast * (occlusion - 0.5) + 0.5;

    float3 oc = occlusion;
    fragColor = float4(oc, position.a);

    return fragColor;
}

#endif