using UnityEngine;
using UnityEngine.Rendering;

namespace DopeRP.CPU
{
    public class SSR
    {
        private const string BUFFER_NAME = "SSR";
        
        public void Render(SSRSettings ssrSettings)
        {
            
            RAPI.BeginSample(BUFFER_NAME);
            

            var cameraWidth = RAPI.CurCamera.pixelWidth;
            var cameraHeight = RAPI.CurCamera.pixelHeight;
            
            RAPI.Buffer.GetTemporaryRT(SProps.SSR.SSRRawAtlas, cameraWidth, cameraHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            RAPI.Buffer.SetRenderTarget(SProps.SSR.SSRRawAtlas, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            RAPI.Buffer.ClearRenderTarget(true, true, Color.clear);

            // RAPI.Buffer.SetGlobalVector(SProps.SSAO.NoiseScale, new Vector4((float)cameraWidth/ssaoSettings.randomSize, (float)cameraHeight/ssaoSettings.randomSize, 0f, 0f));
            // RAPI.SetKeywords(ssaoSettings.samplesCount);

            RAPI.ExecuteBuffer(); 
            
            RAPI.DrawFullscreenQuad(ssrSettings.SSRMaterial, 0);
            
            RAPI.ExecuteBuffer();
            
            RAPI.Buffer.SetGlobalTexture(SProps.SSR.SSRRawAtlas, SProps.SSR.SSRRawAtlas);
            
            RAPI.Buffer.GetTemporaryRT(SProps.SSR.SSRColorAtlas, cameraWidth, cameraHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            RAPI.Buffer.SetRenderTarget(SProps.SSR.SSRColorAtlas, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            RAPI.Buffer.ClearRenderTarget(true, true, Color.clear);
            
            RAPI.ExecuteBuffer();
            
            RAPI.DrawFullscreenQuad(ssrSettings.SSRMaterial, 1);
            
            RAPI.ExecuteBuffer();

            // RAPI.Buffer.SetGlobalTexture(SProps.SSAO.SSAOBlurAtlas, SProps.SSAO.SSAOBlurAtlas);
            
            
            RAPI.EndSample(BUFFER_NAME);
            
        }

    }
    
}
