#ifndef SSAO_PASS2_INCLUDED
#define SSAO_PASS2_INCLUDED

#include "UnityCG.cginc"
#define NUM_SAMPLES 64
#define NUM_NOISE   16



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
            sampler2D _NormalMapSSAO;

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
float _randomSize;


float3 getRandom(in float2 uv) {
    // return float2(1,1);
    return float3(normalize(tex2D(_NormalMapSSAO, _ScreenSize.xy * uv / _randomSize).xy * 2.0f - 1.0f),0); 
}

            float4 frag (v2f i) : SV_Target
            {
                // if (_Enabled < 1) { discard; }

// return float4(i.uv, 0,1);
    float3 sample_sphere[64];
    sample_sphere[0] = float3(0.04977, -0.04471, 0.04996);
    sample_sphere[1] = float3(0.01457, 0.01653, 0.00224);
    sample_sphere[2] = float3(-0.04065, -0.01937, 0.03193);
    sample_sphere[3] = float3(0.01378, -0.09158, 0.04092);
    sample_sphere[4] = float3(0.05599, 0.05979, 0.05766);
    sample_sphere[5] = float3(0.09227, 0.04428, 0.01545);
    sample_sphere[6] = float3(-0.00204, -0.0544, 0.06674);
    sample_sphere[7] = float3(-0.00033, -0.00019, 0.00037);
    sample_sphere[8] = float3(0.05004, -0.04665, 0.02538);
    sample_sphere[9] = float3(0.03813, 0.0314, 0.03287);
    sample_sphere[10] = float3(-0.03188, 0.02046, 0.02251);
    sample_sphere[11] = float3(0.0557, -0.03697, 0.05449);
    sample_sphere[12] = float3(0.05737, -0.02254, 0.07554);
    sample_sphere[13] = float3(-0.01609, -0.00377, 0.05547);
    sample_sphere[14] = float3(-0.02503, -0.02483, 0.02495);
    sample_sphere[15] = float3(-0.03369, 0.02139, 0.0254);
    sample_sphere[16] = float3(-0.01753, 0.01439, 0.00535);
    sample_sphere[17] = float3(0.07336, 0.11205, 0.01101);
    sample_sphere[18] = float3(-0.04406, -0.09028, 0.08368);
    sample_sphere[19] = float3(-0.08328, -0.00168, 0.08499);
    sample_sphere[20] = float3(-0.01041, -0.03287, 0.01927);
    sample_sphere[21] = float3(0.00321, -0.00488, 0.00416);
    sample_sphere[22] = float3(-0.00738, -0.06583, 0.0674);
    sample_sphere[23] = float3(0.09414, -0.008, 0.14335);
    sample_sphere[24] = float3(0.07683, 0.12697, 0.107);
    sample_sphere[25] = float3(0.00039, 0.00045, 0.0003);
    sample_sphere[26] = float3(-0.10479, 0.06544, 0.10174);
    sample_sphere[27] = float3(-0.00445, -0.11964, 0.1619);
    sample_sphere[28] = float3(-0.07455, 0.03445, 0.22414);
    sample_sphere[29] = float3(-0.00276, 0.00308, 0.00292);
    sample_sphere[30] = float3(-0.10851, 0.14234, 0.16644);
    sample_sphere[31] = float3(0.04688, 0.10364, 0.05958);
    sample_sphere[32] = float3(0.13457, -0.02251, 0.13051);
    sample_sphere[33] = float3(-0.16449, -0.15564, 0.12454);
    sample_sphere[34] = float3(-0.18767, -0.20883, 0.05777);
    sample_sphere[35] = float3(-0.04372, 0.08693, 0.0748);
    sample_sphere[36] = float3(-0.00256, -0.002, 0.00407);
    sample_sphere[37] = float3(-0.0967, -0.18226, 0.29949);
    sample_sphere[38] = float3(-0.22577, 0.31606, 0.08916);
    sample_sphere[39] = float3(-0.02751, 0.28719, 0.31718);
    sample_sphere[40] = float3(0.20722, -0.27084, 0.11013);
    sample_sphere[41] = float3(0.0549, 0.10434, 0.32311);
    sample_sphere[42] = float3(-0.13086, 0.11929, 0.28022);
    sample_sphere[43] = float3(0.15404, -0.06537, 0.22984);
    sample_sphere[44] = float3(0.05294, -0.22787, 0.14848);
    sample_sphere[45] = float3(-0.18731, -0.04022, 0.01593);
    sample_sphere[46] = float3(0.14184, 0.04716, 0.13485);
    sample_sphere[47] = float3(-0.04427, 0.05562, 0.05586);
    sample_sphere[48] = float3(-0.02358, -0.08097, 0.21913);
    sample_sphere[49] = float3(-0.14215, 0.19807, 0.00519);
    sample_sphere[50] = float3(0.15865, 0.23046, 0.04372);
    sample_sphere[51] = float3(0.03004, 0.38183, 0.16383);
    sample_sphere[52] = float3(0.08301, -0.30966, 0.06741);
    sample_sphere[53] = float3(0.22695, -0.23535, 0.19367);
    sample_sphere[54] = float3(0.38129, 0.33204, 0.52949);
    sample_sphere[55] = float3(-0.55627, 0.29472, 0.3011);
    sample_sphere[56] = float3(0.42449, 0.00565, 0.11758);
    sample_sphere[57] = float3(0.3665, 0.00359, 0.0857);
    sample_sphere[58] = float3(0.32902, 0.0309, 0.1785);
    sample_sphere[59] = float3(-0.08294, 0.51285, 0.05656);
    sample_sphere[60] = float3(0.86736, -0.00273, 0.10014);
    sample_sphere[61] = float3(0.45574, -0.77201, 0.00384);
    sample_sphere[62] = float3(0.41729, -0.15485, 0.46251);
    sample_sphere[63] = float3 (-0.44272, -0.67928, 0.1865);
                
                float2 texCoord = i.vertex / _ScreenSize;

                float4 position = tex2D(_PositionViewSpace, texCoord).xyzw;
                // position = float4(position.xy, -position.w, position.w);
                // return float4(1,1,1,1);
                if(position.a <= 0)
                    return 0;
                
                float3 normal = normalize(tex2D(_NormalViewSpace, i.uv).xyz);
                // normal = float3(-normal.r, normal.gb);
                // return float4(normal,1);

                // float4 a = mul(lensProjection, position.xyz) / mul(lensProjection, position.xyz).w;
                // return float4(a.xy *0.5 + 0.5, a.z, 1);
                // return float4(position.zzz, 1.0);
                // return float4(i.uv+1, 0,1.0);
                // return float4(normal.xyz, 1.0);
                // return float4(texCoord, 0,1.0);
                int noiseS = int(sqrt(NUM_NOISE));
                int noiseX = int(i.vertex.x - 0.5) % noiseS;
                int noiseY = int(i.vertex.y - 0.5) % noiseS;
                float3 random = _ssaoNoise[noiseX + noiseY];
                random = getRandom(float2(noiseX ,noiseY));
                // return float4(random,1.0);
                random = normalize(random);
                float3 tangent = normalize(random - normal * dot(random, normal));
                float3 binormal = cross(normal, tangent);
                float3x3 tbn = float3x3(tangent, binormal, normal);

                float occlusion = 0;
                
                for (int j = 0; j < NUM_SAMPLES; j++)
                {
                    // _ssaoSamples[j] = normalize(_ssaoSamples[j]);
                    float3 samplePosition = mul(tbn, float3(sample_sphere[j].xyz));
                    samplePosition = position + float3(samplePosition.xy, samplePosition.z) * _Radius;
                    // return float4(-samplePosition.zz/20, -position.z/20    ,1);
                    
                    // float4 offset = float4(samplePosition, 1.0);

                    // Transform from view space to clip space and then to UV space
                    float4 offset = mul(lensProjection, samplePosition); // Projection
                                        // return float4(offset.xy, 0,1);

                    offset.xyz /= offset.w; // Perspective divide
                    offset.xy = offset.xy * 0.5 + 0.5; // UV space
                    // return float4(occlusion, i.uv.x, offset.x, 1);
                    // float4 newPos = position + float4(0.1,0.1,0,0);
                    // offset = mul(lensProjection, newPos);
                    // offset = offset /offset.w *0.5 + 0.5;
                    // return float4(offset.x, i.uv.x, 0,1);

                    // return float4(offset.x> i.uv.x+_Bias, offset.y > i.uv.y +_Bias, 0,1);
                    // return float4(offset.xy, offset.z,1.0);
// return float4(samplePosition.xy -10,0    ,1);
// return float4(-offset.xy, 0,1);
                    
                    float4 offsetPosition = tex2D(_PositionViewSpace, offset.xy);
                    // float offsetDepth = -tex2D(_PositionViewSpace, offset.xy).w;

                    // return float4(position.x, offsetPosition.x,0,1);
                    // return float4(offsetPosition.xy, -offsetPosition.z/20, offsetPosition.w);
                    // return float4(-offsetPosition.zzz/20, 1);
                    // float sampleDepth = tex2D(_PositionViewSpace, offset.xy).z;
                    // return float4(offsetPosition.yy*_Bias, samplePosition.y,1);
                    // return float4(offsetPosition.xx*_Bias, samplePosition.x,1);
                    //
                    // float4 b = offsetPosition.z>samplePosition.z+_Bias;
                    // return float4(b.xxx,1);  
                    
// float4 a = offsetPosition.y>samplePosition.y+_Bias;
//            return float4(a.xxx,1);         
//                     return float4(offsetPosition.x>samplePosition.x+_Bias, offsetPosition.x>samplePosition.x+_Bias
//                         , offsetPosition.x>samplePosition.x+_Bias, 1);
//                     
//                     return float4(offsetPosition.x>samplePosition.x+_Bias, offsetPosition.x>samplePosition.x+_Bias
//                         , offsetPosition.x>samplePosition.x+_Bias, 1);
//                     
//                     
//                     return float4(offsetPosition.x>samplePosition.x+_Bias, offsetPosition.y > samplePosition.y
//                         , offsetPosition.z > samplePosition.z, 1);

                    // return float4(-position.zzz/20, 1);

                    // float occluded = 0;
                    // if   (samplePosition.z + _Bias <= offsetPosition.z)
                    // { occluded = 0; }
                    // else { occluded = 1; }
                    
                    // float rangeCheck = smoothstep(0.0, 1.0, _Radius / abs(position.z - sampleDepth));
                    // occlusion += (samplePosition.z >= sampleDepth + _Bias) ? 0.0 : rangeCheck;

                    // float intensity =smoothstep( 0, 1,   _Radius/ abs(position.z - offsetPosition.z));
                    // occluded  *= intensity;
                    // occlusion -= occluded;
                    // return float4(-position.z/100, -offsetPosition.z/100, -samplePosition.z/100, 1);
                    // return float4(position.z, offsetPosition.z, samplePosition.z, 1);

                    
                    float intensity =
                      smoothstep
                        ( 0
                        , 1
                        ,   _Radius
                          / abs(position.z - offsetPosition.z)
                        );
                    float rangeCheck = smoothstep(0.0, 1.0, _Radius / abs(position.z - offsetPosition.z));
                    occlusion += (offsetPosition.z >= samplePosition.z + _Bias ? 1.0 : 0.0) * rangeCheck ;
                    // return float4(occlusion, offsetPosition.x, samplePosition.x, 1) * rangeCheck;

                    // return float4(occlusion.xxx,1);
                }

                // occlusion = 1.0 - (occlusion / float(NUM_SAMPLES));
                // occlusion = pow(occlusion, _Magnitude);
                // occlusion = _Contrast * (occlusion - 0.5) + 0.5;

                // occlusion /= NUM_SAMPLES;
                // occlusion  = pow(occlusion, _Magnitude);
                // occlusion  = _Contrast * (occlusion - 0.5) + 0.5;
                // occlusion = 1.0 - (occlusion / NUM_SAMPLES);
                // float4 fragColor = occlusion;  
                // float4 fragColor = float4(occlusion.xxx, 1);


                
                occlusion = 1.0 - (occlusion / NUM_SAMPLES);
                
                occlusion  = pow(occlusion, _Magnitude);
                occlusion  = _Contrast * (occlusion - 0.5) + 0.5;
                
                float4 fragColor = occlusion;  
                // return float4(random,1.0);

                // return tex2D(_NormalViewSpace, i.uv).xyzw;
                return fragColor;
                return float4(occlusion, occlusion, occlusion, 1.0);
            }

#endif