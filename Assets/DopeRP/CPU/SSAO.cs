using UnityEngine;
using UnityEngine.Rendering;

namespace DopeRP.CPU
{
    public class SSAO
    {
        private const string BUFFER_NAME = "SSAO";
        
        public void Render(SSAOSettings ssaoSettings)
        {
            
            RAPI.Buffer.name = "ssao";
            RAPI.Buffer.BeginSample("ssao");
            RAPI.ExecuteBuffer();

            var cameraWidth = RAPI.CurCamera.pixelWidth;
            var cameraHeight = RAPI.CurCamera.pixelHeight;
            
            RAPI.Buffer.GetTemporaryRT(SProps.SSAO.SSAORawAtlas, cameraWidth / 2, cameraHeight / 2, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat);
            RAPI.Buffer.SetRenderTarget(SProps.SSAO.SSAORawAtlas, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            RAPI.Buffer.ClearRenderTarget(true, true, Color.clear);

            RAPI.Buffer.SetGlobalVector(SProps.SSAO.NoiseScale, new Vector4((float)cameraWidth/ssaoSettings.randomSize, (float)cameraHeight/ssaoSettings.randomSize, 0f, 0f));
            RAPI.SetKeywords(ssaoSettings.samplesCount);

            RAPI.ExecuteBuffer(); 
            
            RAPI.DrawFullscreenQuad(ssaoSettings.SSAOMaterial, 0);
            // RAPI.Buffer.DrawProcedural(Matrix4x4.identity, settings.Material, (int)pass, MeshTopology.Triangles, 3);
            // RAPI.Buffer.SetViewProjectionMatrices(RAPI.CurCamera.worldToCameraMatrix, RAPI.CurCamera.projectionMatrix);
            
            RAPI.ExecuteBuffer();
            
            RAPI.Buffer.SetGlobalTexture("_SSAORawAtlas", SProps.SSAO.SSAORawAtlas);

            RAPI.Buffer.GetTemporaryRT(SProps.SSAO.SSAOBlurAtlas, cameraWidth, cameraHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat);
            RAPI.Buffer.SetRenderTarget(SProps.SSAO.SSAOBlurAtlas, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            RAPI.Buffer.ClearRenderTarget(true, true, Color.clear);
            
            RAPI.ExecuteBuffer();
            
            
            RAPI.DrawFullscreenQuad(ssaoSettings.SSAOMaterial, SProps.SSAO.SSAOBlurPassName);
            
            
            RAPI.ExecuteBuffer();

            RAPI.Buffer.SetGlobalTexture(SProps.SSAO.SSAOBlurAtlas, SProps.SSAO.SSAOBlurAtlas);
            
            RAPI.Buffer.EndSample("ssao");
            RAPI.ExecuteBuffer();
            
        }

    }
    
}
