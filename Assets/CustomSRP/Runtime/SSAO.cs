using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
    public class SSAO
    {
        private const string BUFFER_NAME = "SSAO";

        private static ShaderTagId colorBufferID = new ShaderTagId("PositionBufferSpace");
        
        public static int colorBufferAtlasId = Shader.PropertyToID("_ColorBufferAtlas");
        
        
        
        private static ShaderTagId normalBufferID = new ShaderTagId("NormalBufferPass");

        public static int NormalBufferAtlasId = Shader.PropertyToID("_NormalBufferAtlas");
        
        
        public static int ssaoSamplesId = Shader.PropertyToID("_ssaoSamples");
        private static Vector4[] ssaoSamples = new Vector4[8];
        
        public static int ssaoNoiseId = Shader.PropertyToID("_ssaoNoise");
        private static Vector4[] ssaoNoise = new Vector4[4];
        
        
        public static int lensProjection = Shader.PropertyToID("lensProjection");






        public void Render()
        {

            RAPI.Buffer.BeginSample(BUFFER_NAME);

            RAPI.Buffer.GetTemporaryRT(colorBufferAtlasId, RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight, 32, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            RAPI.Buffer.SetRenderTarget(colorBufferAtlasId, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            RAPI.Buffer.ClearRenderTarget(true, true, Color.clear);

            RAPI.Context.SetupCameraProperties(RAPI.CurCamera);

            RAPI.ExecuteBuffer();
            
            var sortingSettings = new SortingSettings(RAPI.CurCamera)
            {
                criteria = SortingCriteria.CommonOpaque
            };

            var drawingSettings = new DrawingSettings(colorBufferID, sortingSettings)
            {
                enableDynamicBatching = false,
                enableInstancing = true,
            };
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

            RAPI.Context.DrawRenderers(RAPI.CullingResults, ref drawingSettings, ref filteringSettings);
            
            RAPI.Buffer.EndSample(BUFFER_NAME);
            RAPI.ExecuteBuffer();
            
            RAPI.Buffer.GetTemporaryRT(NormalBufferAtlasId, RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight, 32, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            RAPI.Buffer.SetRenderTarget(NormalBufferAtlasId, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            RAPI.Buffer.ClearRenderTarget(true, true, Color.clear);
            
            
            drawingSettings.SetShaderPassName(0, normalBufferID);

            RAPI.Context.DrawRenderers(RAPI.CullingResults, ref drawingSettings, ref filteringSettings);
            RAPI.ExecuteBuffer();


            int i = 0;
            foreach (var sampleVec3 in GetSSAOSamples())
            {
                ssaoSamples[i] = sampleVec3;
                i++;
            }
            
            RAPI.Buffer.SetGlobalVectorArray(ssaoSamplesId, ssaoSamples);
            RAPI.Buffer.SetGlobalVectorArray(ssaoNoiseId, ssaoNoise);

            
            Matrix4x4 projectionMatrix = RAPI.CurCamera.projectionMatrix;
            
            RAPI.Buffer.SetGlobalMatrix(lensProjection, projectionMatrix);
            
        }

        private List<Vector3> GetSSAOSamples()
        {
            System.Random generator = new System.Random(); // Consider seeding with a specific value if you want repeatable results
            float randomFloats(System.Random gen) => (float)gen.NextDouble();

            List<Vector3> ssaoSamples = new List<Vector3>();

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

                ssaoSamples.Add(sample);
            }

            return ssaoSamples;
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