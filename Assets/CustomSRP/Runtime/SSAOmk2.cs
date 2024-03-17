using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
    public class SSAOmk2
    {
        private const string BUFFER_NAME = "SSAOmk2";

        private static ShaderTagId SSAOPassId = new ShaderTagId("SSAOPass");
        
        public static int SSAOAtlas = Shader.PropertyToID("_SSAOAtlas");
        
        
        Material ssaoMaterial = new Material(Shader.Find("CustomSRP/S_Lit"));


        public static int ssaoSamplesId = Shader.PropertyToID("_ssaoSamples");
        private static Vector4[] ssaoSamples = new Vector4[8];
        
        public static int ssaoNoiseId = Shader.PropertyToID("_ssaoNoise");
        private static Vector4[] ssaoNoise = new Vector4[4];


        public void Render()
        {
            
            RAPI.Buffer.BeginSample(BUFFER_NAME);
            RAPI.ExecuteBuffer();
            
            RAPI.Buffer.GetTemporaryRT(SSAOAtlas, RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat);
            RAPI.Buffer.SetRenderTarget(SSAOAtlas, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            RAPI.Buffer.ClearRenderTarget(true, true, Color.clear);
            
            RAPI.Buffer.SetGlobalTexture("_NormalMapSSAO", CustomRenderPipelineAsset.instance.SSAOSettings.normalMapTexture);
            RAPI.Buffer.SetGlobalFloat("_randomSize", CustomRenderPipelineAsset.instance.SSAOSettings.randomSize);
            RAPI.Buffer.SetGlobalFloat("_Radius", CustomRenderPipelineAsset.instance.SSAOSettings.gSampleRad);
            RAPI.Buffer.SetGlobalFloat("_Contrast", CustomRenderPipelineAsset.instance.SSAOSettings.gIntensity);
            RAPI.Buffer.SetGlobalFloat("_Magnitude", CustomRenderPipelineAsset.instance.SSAOSettings.gScale);
            RAPI.Buffer.SetGlobalFloat("_Bias", CustomRenderPipelineAsset.instance.SSAOSettings.gBias);
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

            RAPI.Buffer.EndSample(BUFFER_NAME);
            RAPI.ExecuteBuffer();
        }
        
        private void GetSSAOSamples()
        {
            System.Random generator = new System.Random(); // Consider seeding with a specific value if you want repeatable results
            float randomFloats(System.Random gen) => (float)gen.NextDouble();


            int numberOfSamples = 8;
            
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
                sample *= scale; // Scales the sample

                ssaoSamples[i] = sample;
            }

        }

        private void SetNoise()
        {
            System.Random generator = new System.Random(); // You might want to seed this for reproducible results
            float randomFloats(System.Random gen) => (float)gen.NextDouble();


            for (int i = 0; i < 4; ++i) {
                Vector3 noise = new Vector3(
                    randomFloats(generator) * 2.0f - 1.0f, // X
                    randomFloats(generator) * 2.0f - 1.0f, // Y
                    0.0f // Z
                );

                ssaoNoise[i] = noise;
            }
        }

       
    }
}