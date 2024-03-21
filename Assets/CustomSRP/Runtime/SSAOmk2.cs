using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
    public class SSAOmk2
    {
        private const string BUFFER_NAME = "SSAOmk2";
        private const int SSAO_SAMPLES = 64;
        private const int SSAO_NOISE = 16;
        

        private static ShaderTagId SSAOPassId = new ShaderTagId("SSAOPass");
        
        public static int SSAOAtlas = Shader.PropertyToID("_SSAOAtlas");
        
        public static int SSAOAtlasBlurred = Shader.PropertyToID("_SSAOAtlasBlurred");

        
        
        Material ssaoMaterial = new Material(Shader.Find("CustomSRP/S_Lit"));


        
        public static int ssaoSamplesId = Shader.PropertyToID("_ssaoSamples");
        private static Vector4[] ssaoSamples = new Vector4[SSAO_SAMPLES];
        
        public static int ssaoNoiseId = Shader.PropertyToID("_ssaoNoise");
        private static Vector4[] ssaoNoise = new Vector4[SSAO_NOISE];


        public void Render()
        {
            
            RAPI.Buffer.BeginSample(BUFFER_NAME);
            RAPI.ExecuteBuffer();
            
            RAPI.Buffer.GetTemporaryRT(SSAOAtlas, RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat);
            RAPI.Buffer.SetRenderTarget(SSAOAtlas, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            RAPI.Buffer.ClearRenderTarget(true, true, Color.clear);
            
            RAPI.Buffer.SetGlobalTexture("_NormalMapSSAO", CustomRenderPipelineAsset.instance.SSAOSettings.noiseTexture);
            RAPI.Buffer.SetGlobalFloat("_randomSize", CustomRenderPipelineAsset.instance.SSAOSettings.randomSize);
            RAPI.Buffer.SetGlobalFloat("_Radius", CustomRenderPipelineAsset.instance.SSAOSettings.sampleRadius);
            RAPI.Buffer.SetGlobalFloat("_Contrast", CustomRenderPipelineAsset.instance.SSAOSettings.contrast);
            RAPI.Buffer.SetGlobalFloat("_Magnitude", CustomRenderPipelineAsset.instance.SSAOSettings.magnitude);
            RAPI.Buffer.SetGlobalFloat("_Bias", CustomRenderPipelineAsset.instance.SSAOSettings.bias);
            RAPI.Buffer.SetGlobalVector("_ScreenSize", new Vector4(RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight, 0f, 0f));
            GetSSAOSamples();
            SetNoise();
            RAPI.Buffer.SetGlobalVectorArray(ssaoSamplesId, ssaoSamples);
            RAPI.Buffer.SetGlobalVectorArray(ssaoNoiseId, ssaoNoise);
            
            Matrix4x4 projectionMatrix = RAPI.CurCamera.projectionMatrix;
            
            RAPI.Buffer.SetGlobalMatrix(Shader.PropertyToID("lensProjection"), projectionMatrix);
            
            RAPI.ExecuteBuffer();
            
            RAPI.Buffer.Blit(null, BuiltinRenderTextureType.CurrentActive, ssaoMaterial, 6);
            
            

            RAPI.ExecuteBuffer();

            
            RAPI.Buffer.SetGlobalTexture("_SSAOAtlas", SSAOAtlas);

            RAPI.Buffer.GetTemporaryRT(SSAOAtlasBlurred, RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat);
            RAPI.Buffer.SetRenderTarget(SSAOAtlasBlurred, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            RAPI.Buffer.ClearRenderTarget(true, true, Color.clear);
            
            RAPI.ExecuteBuffer();

            
            RAPI.Buffer.Blit(null, BuiltinRenderTextureType.CurrentActive, ssaoMaterial, 7);

            RAPI.ExecuteBuffer();

            RAPI.Buffer.SetGlobalTexture("_SSAOAtlasBlurred", SSAOAtlasBlurred);
            
            RAPI.Buffer.EndSample(BUFFER_NAME);
            RAPI.ExecuteBuffer();
        }
        
        private void GetSSAOSamples()
        {
            System.Random generator = new System.Random(); // Consider seeding with a specific value if you want repeatable results
            float randomFloats(System.Random gen) => (float)gen.NextDouble();


            int numberOfSamples = SSAO_SAMPLES;
            
            for (int i = 0; i < numberOfSamples; ++i) {
                Vector3 sample = new Vector3(
                    randomFloats(generator) * 2.0f - 1.0f, // X
                    randomFloats(generator) * 2.0f - 1.0f, // Y
                    randomFloats(generator) // Z
                ).normalized;

                float rand = randomFloats(generator);
                sample *= rand; // Multiplies each component of the vector by rand

                float scale = (float)i / (float)numberOfSamples;
                scale = Mathf.Lerp(0.1f, 1.0f, scale * scale); // Linearly interpolates between 0.1 and 1.0 based on scale * scale
                // sample *= scale; // Scales the sample

                ssaoSamples[i] = sample * scale;
            }
            
            // ssaoSamples[0] = new Vector4(-0.0125591f, -0.0224597f, 0.0023051f, 0.0000000f);
            // ssaoSamples[1] = new Vector4(0.0613070f, 0.0722663f, 0.0401883f, 0.0000000f);
            // ssaoSamples[2] = new Vector4(-0.0021777f, 0.0098662f, 0.0149186f, 0.0000000f);
            // ssaoSamples[3] = new Vector4(0.0176409f, -0.0247834f, 0.0241677f, 0.0000000f);
            // ssaoSamples[4] = new Vector4(-0.0559420f, 0.1956171f, 0.0894427f, 0.0000000f);
            // ssaoSamples[5] = new Vector4(0.0590212f, -0.0783390f, 0.1016940f, 0.0000000f);
            // ssaoSamples[6] = new Vector4(-0.3004070f, -0.1294822f, 0.1787521f, 0.0000000f);
            // ssaoSamples[7] = new Vector4(-0.1339805f, 0.0668470f, 0.4099129f, 0.0000000f);
            //
            return;
            ssaoSamples[0] = new Vector4(0.0375745f, -0.0225461f, 0.0207023f, 0.0000000f);

            // ssaoSamples[0] = new Vector4(0.0375745f, -0.0225461f, 0.0207023f, 0.0000000f);
            ssaoSamples[1] = new Vector4(0.0283229f, 0.0581917f, 0.0654839f, 0.0000000f);
            ssaoSamples[2] = new Vector4(0.0480149f, -0.0462350f, 0.0006761f, 0.0000000f);
            ssaoSamples[3] = new Vector4(0.0097071f, 0.0170045f, 0.0094413f, 0.0000000f);
            ssaoSamples[4] = new Vector4(-0.0022931f, -0.0008539f, 0.0004168f, 0.0000000f);
            ssaoSamples[5] = new Vector4(-0.0174059f, -0.0496512f, 0.0254581f, 0.0000000f);
            ssaoSamples[6] = new Vector4(0.0286942f, -0.0545384f, 0.0181226f, 0.0000000f);
            ssaoSamples[7] = new Vector4(0.0112565f, -0.0125308f, 0.0182492f, 0.0000000f);
            ssaoSamples[8] = new Vector4(0.0891868f, 0.0525064f, 0.0329906f, 0.0000000f);
            ssaoSamples[9] = new Vector4(-0.0495881f, 0.0988519f, 0.0052914f, 0.0000000f);
            ssaoSamples[10] = new Vector4(0.0166541f, 0.0283606f, 0.0099082f, 0.0000000f);
            ssaoSamples[11] = new Vector4(0.0047773f, -0.0662068f, 0.0717516f, 0.0000000f);
            ssaoSamples[12] = new Vector4(0.0173159f, 0.0270746f, 0.0707474f, 0.0000000f);
            ssaoSamples[13] = new Vector4(-0.0483492f, 0.0938815f, 0.0327544f, 0.0000000f);
            ssaoSamples[14] = new Vector4(0.0510324f, 0.0583218f, 0.0535065f, 0.0000000f);
            ssaoSamples[15] = new Vector4(0.0077954f, -0.0846078f, 0.0292854f, 0.0000000f);
            ssaoSamples[16] = new Vector4(-0.0008219f, 0.0074872f, 0.0107956f, 0.0000000f);
            ssaoSamples[17] = new Vector4(0.0300664f, 0.0117305f, 0.0528191f, 0.0000000f);
            ssaoSamples[18] = new Vector4(-0.0283952f, -0.0598936f, 0.0242383f, 0.0000000f);
            ssaoSamples[19] = new Vector4(0.0428084f, -0.0553624f, 0.0032783f, 0.0000000f);
            ssaoSamples[20] = new Vector4(0.0026063f, 0.0006062f, 0.0057860f, 0.0000000f);
            ssaoSamples[21] = new Vector4(-0.0424357f, -0.1223815f, 0.1359208f, 0.0000000f);
            ssaoSamples[22] = new Vector4(-0.0619585f, -0.0521398f, 0.0471463f, 0.0000000f);
            ssaoSamples[23] = new Vector4(-0.1069987f, -0.0313618f, 0.0510750f, 0.0000000f);
            ssaoSamples[24] = new Vector4(0.0755970f, 0.0672650f, 0.1289571f, 0.0000000f);
            ssaoSamples[25] = new Vector4(0.0005117f, -0.0037229f, 0.0001704f, 0.0000000f);
            ssaoSamples[26] = new Vector4(0.0288700f, 0.0030673f, 0.0112250f, 0.0000000f);
            ssaoSamples[27] = new Vector4(0.0608572f, 0.1776431f, 0.0041684f, 0.0000000f);
            ssaoSamples[28] = new Vector4(0.1541043f, -0.0709406f, 0.2046907f, 0.0000000f);
            ssaoSamples[29] = new Vector4(-0.0697187f, -0.0785035f, 0.0297309f, 0.0000000f);
            ssaoSamples[30] = new Vector4(0.0536569f, 0.0546040f, 0.0479745f, 0.0000000f);
            ssaoSamples[31] = new Vector4(-0.0155425f, -0.1544447f, 0.2602409f, 0.0000000f);
            ssaoSamples[32] = new Vector4(0.0142489f, 0.0103536f, 0.0180407f, 0.0000000f);
            ssaoSamples[33] = new Vector4(-0.0657320f, -0.0627757f, 0.1490029f, 0.0000000f);
            ssaoSamples[34] = new Vector4(0.0149647f, 0.2831507f, 0.0670478f, 0.0000000f);
            ssaoSamples[35] = new Vector4(0.1513126f, 0.1306099f, 0.0733813f, 0.0000000f);
            ssaoSamples[36] = new Vector4(-0.0634051f, -0.0992622f, 0.2553480f, 0.0000000f);
            ssaoSamples[37] = new Vector4(-0.0715028f, -0.0730536f, 0.2007045f, 0.0000000f);
            ssaoSamples[38] = new Vector4(-0.0211809f, -0.0448673f, 0.2745099f, 0.0000000f);
            ssaoSamples[39] = new Vector4(0.0271176f, 0.1124115f, 0.1321233f, 0.0000000f);
            ssaoSamples[40] = new Vector4(-0.2880448f, -0.2402613f, 0.1867923f, 0.0000000f);
            ssaoSamples[41] = new Vector4(-0.1374824f, 0.1887842f, 0.0880364f, 0.0000000f);
            ssaoSamples[42] = new Vector4(0.0365745f, -0.0225461f, 0.0207023f, 0.0000000f);
            ssaoSamples[43] = new Vector4(0.0283229f, 0.0581917f, 0.0654839f, 0.0000000f);
            ssaoSamples[44] = new Vector4(0.0480149f, -0.0462350f, 0.0006761f, 0.0000000f);
            ssaoSamples[45] = new Vector4(0.0680149f, -0.0762350f, 0.006761f, 0.0000000f);
            ssaoSamples[46] = new Vector4(-0.1547538f, -0.3724611f, 0.2931141f, 0.0000000f);
            ssaoSamples[47] = new Vector4(-0.0303488f, -0.2584694f, 0.3902751f, 0.0000000f);
            ssaoSamples[48] = new Vector4(-0.0452204f, 0.2504474f, 0.2368452f, 0.0000000f);
            ssaoSamples[49] = new Vector4(-0.0844901f, -0.1275178f, 0.2640623f, 0.0000000f);
            ssaoSamples[50] = new Vector4(0.0068778f, -0.0062482f, 0.0017688f, 0.0000000f);
            ssaoSamples[51] = new Vector4(0.0210396f, -0.0169679f, 0.0132043f, 0.0000000f);
            ssaoSamples[52] = new Vector4(0.2639030f, 0.1956469f, 0.0437231f, 0.0000000f);
            ssaoSamples[53] = new Vector4(0.3435872f, -0.0821374f, 0.2974682f, 0.0000000f);
            ssaoSamples[54] = new Vector4(0.0194747f, -0.0395784f, 0.0247225f, 0.0000000f);
            ssaoSamples[55] = new Vector4(-0.2262188f, 0.0354950f, 0.0455336f, 0.0000000f);
            ssaoSamples[56] = new Vector4(-0.0096651f, 0.0379737f, 0.1012541f, 0.0000000f);
            ssaoSamples[57] = new Vector4(0.0545868f, 0.0791594f, 0.0536336f, 0.0000000f);
            ssaoSamples[58] = new Vector4(0.5047569f, -0.3034810f, 0.1880293f, 0.0000000f);
            ssaoSamples[59] = new Vector4(-0.1355794f, 0.0185082f, 0.1430759f, 0.0000000f);
            ssaoSamples[60] = new Vector4(0.0057807f, 0.0789871f, 0.5953924f, 0.0000000f);
            ssaoSamples[61] = new Vector4(-0.0158339f, -0.0005985f, 0.0169315f, 0.0000000f);
            ssaoSamples[62] = new Vector4(-0.1589250f, 0.2581905f, 0.2382025f, 0.0000000f);
            ssaoSamples[63] = new Vector4(0.3723478f, -0.2029504f, 0.0933076f, 0.0000000f);


            //
            //
            //  [45]                                         0,1330598          0,0343873          0,1165653          0,0000000
            // [46]                                        -0,1547538         -0,3724611          0,2931141          0,0000000
            // [47]                                        -0,0303488         -0,2584694          0,3902751          0,0000000
            // [48]                                        -0,0452204          0,2504474          0,2368452          0,0000000
            // [49]                                        -0,0844901         -0,1275178          0,2640623          0,0000000
            // [50]                                         0,0068778         -0,0062482          0,0017688          0,0000000
            // [51]                                         0,0210396         -0,0169679          0,0132043          0,0000000
            // [52]                                         0,2639030          0,1956469          0,0437231          0,0000000
            // [53]                                         0,3435872         -0,0821374          0,2974682          0,0000000
            // [54]                                         0,0194747         -0,0395784          0,0247225          0,0000000
            // [55]                                        -0,2262188          0,0354950          0,0455336          0,0000000
            // [56]                                        -0,0096651          0,0379737          0,1012541          0,0000000
            // [57]                                         0,0545868          0,0791594          0,0536336          0,0000000
            // [58]                                         0,5047569         -0,3034810          0,1880293          0,0000000
            // [59]                                        -0,1355794          0,0185082          0,1430759          0,0000000
            // [60]                                         0,0057807          0,0789871          0,5953924          0,0000000
            // [61]                                        -0,0158339         -0,0005985          0,0169315          0,0000000
            // [62]                                        -0,1589250          0,2581905          0,2382025          0,0000000
            // [63]                                         0,3723478         -0,2029504          0,0933076          0,0000000



        }

        private void SetNoise()
        {
            System.Random generator = new System.Random(); // You might want to seed this for reproducible results
            float randomFloats(System.Random gen) => (float)gen.NextDouble();


            for (int i = 0; i < SSAO_NOISE; ++i) {
                Vector3 noise = new Vector3(
                    randomFloats(generator) * 2.0f - 1.0f, // X
                    randomFloats(generator) * 2.0f - 1.0f, // Y
                    0.0f // Z
                );
                
                ssaoNoise[i] = noise;
                    
            }
            // return;

            
            ssaoNoise[0] = new Vector4(-0.5886875f, -0.9517453f, 0.0f, 0.0f);
            ssaoNoise[1] = new Vector4(-0.2297425f, -0.7205612f, 0.0f, 0.0f);
            ssaoNoise[2] = new Vector4(0.2505707f, 0.2372119f, 0.0f, 0.0f);
            ssaoNoise[3] = new Vector4(-0.0032178f, 0.8208930f, 0.0f, 0.0f);
            ssaoNoise[4] = new Vector4(-0.4771163f, 0.7774535f, 0.0f, 0.0f);
            ssaoNoise[5] = new Vector4(-0.8491086f, 0.7518646f, 0.0f, 0.0f);
            ssaoNoise[6] = new Vector4(0.2614864f, -0.6622627f, 0.0f, 0.0f);
            ssaoNoise[7] = new Vector4(0.8165096f, 0.5928583f, 0.0f, 0.0f);
            ssaoNoise[8] = new Vector4(-0.7946346f, 0.4451184f, 0.0f, 0.0f);
            ssaoNoise[9] = new Vector4(0.1703247f, 0.3859808f, 0.0f, 0.0f);
            ssaoNoise[10] = new Vector4(-0.0840252f, -0.5506573f, 0.0f, 0.0f);
            ssaoNoise[11] = new Vector4(-0.9255096f, -0.4691492f, 0.0f, 0.0f);
            ssaoNoise[12] = new Vector4(0.6157835f, -0.9520890f, 0.0f, 0.0f);
            ssaoNoise[13] = new Vector4(-0.1643844f, 0.5771966f, 0.0f, 0.0f);
            ssaoNoise[14] = new Vector4(0.2676045f, -0.3357510f, 0.0f, 0.0f);
            ssaoNoise[15] = new Vector4(0.3092835f, 0.0557748f, 0.0f, 0.0f);

        }
        
        

       
    }
}
