using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
    public class SSAO
    {
        private const string BUFFER_NAME = "SSAO";
        
        // public static int SSAOAtlas = Shader.PropertyToID("_SSAOAtlas");
        // public static int SSAOAtlasBlurred = Shader.PropertyToID("_SSAOAtlasBlurred");

        public void Render(SSAOSettings ssaoSettings)
        {
            RAPI.Buffer.BeginSample(BUFFER_NAME);
            RAPI.ExecuteBuffer();
            
            
            
            RAPI.Buffer.GetTemporaryRT(SProps.SSAO.SSAORawAtlas, RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat);
            RAPI.Buffer.SetRenderTarget(SProps.SSAO.SSAORawAtlas, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            RAPI.Buffer.ClearRenderTarget(true, true, Color.clear);
            
            RAPI.Buffer.SetGlobalVector(SProps.SSAO.ScreenSize, new Vector4(RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight, 0f, 0f));
            Matrix4x4 projectionMatrix = RAPI.CurCamera.projectionMatrix;
            RAPI.Buffer.SetGlobalMatrix(SProps.SSAO.LensProjection, projectionMatrix);
            RAPI.Buffer.SetGlobalVector(SProps.SSAO.NoiseScale, new Vector4(RAPI.CurCamera.pixelWidth/ssaoSettings.randomSize, RAPI.CurCamera.pixelHeight/ssaoSettings.randomSize, 0f, 0f));
            RAPI.SetKeywords(ssaoSettings.samplesCount);
            
            RAPI.ExecuteBuffer();
            
            RAPI.Buffer.Blit(null, BuiltinRenderTextureType.CurrentActive, ssaoSettings.SSAOMaterial, ssaoSettings.SSAOMaterial.FindPass(SProps.SSAO.SSAORawPassName));

            RAPI.ExecuteBuffer();
            
            RAPI.Buffer.SetGlobalTexture("_SSAORawAtlas", SProps.SSAO.SSAORawAtlas);

            RAPI.Buffer.GetTemporaryRT(SProps.SSAO.SSAOBlurAtlas, RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat);
            RAPI.Buffer.SetRenderTarget(SProps.SSAO.SSAOBlurAtlas, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            RAPI.Buffer.ClearRenderTarget(true, true, Color.clear);
            
            RAPI.ExecuteBuffer();
            
            RAPI.Buffer.Blit(null, BuiltinRenderTextureType.CurrentActive, ssaoSettings.SSAOMaterial, ssaoSettings.SSAOMaterial.FindPass(SProps.SSAO.SSAOBlurPassName));

            RAPI.ExecuteBuffer();

            RAPI.Buffer.SetGlobalTexture("_SSAOBlurAtlas", SProps.SSAO.SSAOBlurAtlas);
            
            RAPI.Buffer.EndSample(BUFFER_NAME);
            RAPI.ExecuteBuffer();
            
        }
       
    }
}
