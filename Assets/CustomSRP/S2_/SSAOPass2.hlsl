#ifndef SSAO_PASS2_INCLUDED
#define SSAO_PASS2_INCLUDED

#include "UnityCG.cginc"
#define NUM_SAMPLES 8
#define NUM_NOISE   4
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            sampler2D _PositionViewSpace;
            sampler2D _NormalViewSpace;
            // sampler2D _NormalMapSSAO;
            float4 _ssaoSamples[NUM_SAMPLES];
            float4 _ssaoNoise[NUM_NOISE];
            float _Enabled;
            float _Radius;
            float _Bias;
            float _Magnitude;
            float _Contrast;
            float4 _ScreenSize; // Set this via script to Vector4(Screen.width, Screen.height, 1.0/Screen.width, 1.0/Screen.height)
            float4x4 lensProjection;
            float2 _ScreenSize_TexelSize;


            float4 frag (v2f i) : SV_Target
            {
                // if (_Enabled < 1) { discard; }

                float2 texCoord = i.vertex / _ScreenSize;

                float3 position = tex2D(_PositionViewSpace, i.uv).xyz;
                
                float3 normal = normalize(tex2D(_NormalViewSpace, i.uv).xyz);


                // float4 a = mul(lensProjection, position.xyz) / mul(lensProjection, position.xyz).w;
                // return float4(a.xy *0.5 + 0.5, a.z, 1);
                // return float4(position.zzz, 1.0);
                // return float4(i.uv+1, 0,1.0);
                // return float4(normal.xyz, 1.0);
                // return float4(texCoord, 0,1.0);
                int noiseS = int(sqrt(NUM_NOISE));
                int noiseX = int(i.vertex.x - 0.5) % noiseS;
                int noiseY = int(i.vertex.y - 0.5) % noiseS;
                float3 random = _ssaoNoise[noiseX + (noiseY * noiseS)];
                // return float4(random,1.0);
                
                float3 tangent = normalize(random - normal * dot(random, normal));
                float3 binormal = cross(normal, tangent);
                float3x3 tbn = float3x3(tangent, binormal, normal);

                float occlusion = NUM_SAMPLES;
                
                for (int j = 0; j < NUM_SAMPLES; j++)
                {
                    float3 samplePosition = mul(tbn, _ssaoSamples[j].xy);
                    samplePosition = position + samplePosition * _Radius;
                    // return float4(samplePosition.zzz    ,1);

                    // float4 offset = float4(samplePosition, 1.0);

                    // Transform from view space to clip space and then to UV space
                    float4 offset = mul(lensProjection, position.xyz); // Projection
                                        // return float4(offset.xy, 0,1);

                    offset.xyz /= offset.w; // Perspective divide
                    offset.xy = offset.xy * 0.5 + 0.5; // UV space
                    // return float4(offset.xy, offset.z,1.0);
// return float4(samplePosition.xy -10,0    ,1);
// return float4(-offset.xy, 0,1);
                    float4 offsetPosition = tex2D(_PositionViewSpace, offset.xy);
                    // return float4(-offsetPosition.zzz/20, 1);
                    // float sampleDepth = tex2D(_PositionViewSpace, offset.xy).z;

                    // return float4(-position.zzz/20, 1);

                    float occluded = 0;
                    if   (samplePosition.z + _Bias <= offsetPosition.z)
                    { occluded = 0; }
                    else { occluded = 1; }
                    
                    // float rangeCheck = smoothstep(0.0, 1.0, _Radius / abs(position.z - sampleDepth));
                    // occlusion += (samplePosition.z >= sampleDepth + _Bias) ? 0.0 : rangeCheck;

                    float intensity =smoothstep( 0, 1,   _Radius/ abs(position.z - offsetPosition.z));
                    occluded  *= intensity;
                    occlusion -= occluded;
                }

                // occlusion = 1.0 - (occlusion / float(NUM_SAMPLES));
                // occlusion = pow(occlusion, _Magnitude);
                // occlusion = _Contrast * (occlusion - 0.5) + 0.5;

                occlusion /= NUM_SAMPLES;
                occlusion  = pow(occlusion, _Magnitude);
                occlusion  = _Contrast * (occlusion - 0.5) + 0.5;
                // occlusion = 1.0 - (occlusion / NUM_SAMPLES);
                // float4 fragColor = occlusion;  
                float4 fragColor = float4(occlusion.xxx, 1);

                // return float4(random,1.0);

                // return tex2D(_NormalViewSpace, i.uv).xyzw;
                return fragColor;
                return float4(occlusion, occlusion, occlusion, 1.0);
            }

#endif