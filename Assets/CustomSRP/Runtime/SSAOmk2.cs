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





        public void Render()
        {
            
            RAPI.Buffer.BeginSample(BUFFER_NAME);
            RAPI.ExecuteBuffer();
            
            RAPI.Buffer.GetTemporaryRT(SSAOAtlas, RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat);
            RAPI.Buffer.SetRenderTarget(SSAOAtlas, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            RAPI.Buffer.ClearRenderTarget(true, true, Color.clear);
            
            RAPI.Buffer.SetGlobalTexture("_NormalMapSSAO", CustomRenderPipelineAsset.instance.SSAOSettings.normalMapTexture);
            RAPI.Buffer.SetGlobalFloat("_randomSize", CustomRenderPipelineAsset.instance.SSAOSettings.randomSize);
            RAPI.Buffer.SetGlobalFloat("_gSampleRad", CustomRenderPipelineAsset.instance.SSAOSettings.gSampleRad);
            RAPI.Buffer.SetGlobalFloat("_gIntensity", CustomRenderPipelineAsset.instance.SSAOSettings.gIntensity);
            RAPI.Buffer.SetGlobalFloat("_gScale", CustomRenderPipelineAsset.instance.SSAOSettings.gScale);
            RAPI.Buffer.SetGlobalFloat("_gBias", CustomRenderPipelineAsset.instance.SSAOSettings.gBias);
            RAPI.Buffer.SetGlobalVector("_gScreenSize", new Vector4(RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight, 0f, 0f));

            
            RAPI.ExecuteBuffer();
            
            RAPI.Buffer.Blit(null, BuiltinRenderTextureType.CurrentActive, ssaoMaterial, 6);
            
            

            RAPI.ExecuteBuffer();

            
            RAPI.Buffer.SetGlobalTexture("_SSAOAtlas", SSAOAtlas);

            RAPI.Buffer.EndSample(BUFFER_NAME);
            RAPI.ExecuteBuffer();
        }

       
    }
}