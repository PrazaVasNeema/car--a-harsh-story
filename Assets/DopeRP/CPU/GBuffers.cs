using UnityEngine;
using UnityEngine.Rendering;

namespace DopeRP.CPU
{
    public class GBuffers
    {
        private const string BUFFER_NAME = "GBuffer";
        
        
        public void Render()
        {
            
            
            RAPI.ExecuteBuffer();
            

            Vector2 cameraWidthHeight = new Vector2(RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight);
            RAPI.Buffer.GetTemporaryRT(SProps.GBuffer.GAux_TangentWorldSpaceAtlas, (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf);
            // RAPI.Buffer.GetTemporaryRT(Shader.PropertyToID("Test"), (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 32, FilterMode.Bilinear, RenderTextureFormat.Depth);
            
            RAPI.Buffer.GetTemporaryRT(SProps.GBuffer.G_AlbedoAtlas, (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            RAPI.Buffer.GetTemporaryRT(SProps.GBuffer.G_NormalWorldSpaceAtlas, (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf);
            RAPI.Buffer.GetTemporaryRT(SProps.GBuffer.GAux_ClearNormalWorldSpaceAtlas, (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf);
            RAPI.Buffer.GetTemporaryRT(SProps.GBuffer.G_SpecularAtlas, (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            RAPI.Buffer.GetTemporaryRT(SProps.GBuffer.G_BRDFAtlas, (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Point, RenderTextureFormat.ARGB32);
            
            RenderTargetIdentifier[] colorTargets = {
                new RenderTargetIdentifier(SProps.GBuffer.GAux_TangentWorldSpaceAtlas),
                
                new RenderTargetIdentifier(SProps.GBuffer.G_AlbedoAtlas),
                new RenderTargetIdentifier(SProps.GBuffer.G_NormalWorldSpaceAtlas),
                new RenderTargetIdentifier(SProps.GBuffer.GAux_ClearNormalWorldSpaceAtlas),
                new RenderTargetIdentifier(SProps.GBuffer.G_SpecularAtlas),
                new RenderTargetIdentifier(SProps.GBuffer.G_BRDFAtlas)
            };
            RAPI.Buffer.SetRenderTarget(colorTargets, SProps.Common.DepthBuffer);
            RAPI.Buffer.ClearRenderTarget(false, true, Color.clear);
            

            RAPI.ExecuteBuffer();
            

            var sortingSettings = new SortingSettings(RAPI.CurCamera)
            {
                criteria = SortingCriteria.CommonOpaque
            };

            var drawingSettings = new DrawingSettings(SProps.GBuffer.GBufferPassId, sortingSettings)
            {
                enableDynamicBatching = false,
                enableInstancing = true,
                perObjectData =
                PerObjectData.ReflectionProbes |
                PerObjectData.Lightmaps | PerObjectData.ShadowMask |
                PerObjectData.LightProbe | PerObjectData.OcclusionProbe |
                PerObjectData.LightProbeProxyVolume |
                PerObjectData.OcclusionProbeProxyVolume
            };
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

            RAPI.Context.DrawRenderers(RAPI.CullingResults, ref drawingSettings, ref filteringSettings);

            RAPI.ExecuteBuffer();


            RAPI.Buffer.SetGlobalTexture("_GAux_TangentWorldSpaceAtlas", SProps.GBuffer.GAux_TangentWorldSpaceAtlas);
            // RAPI.Buffer.SetGlobalTexture("Test", Shader.PropertyToID("Test"));
            
            RAPI.Buffer.SetGlobalTexture("_G_AlbedoAtlas", SProps.GBuffer.G_AlbedoAtlas);
            RAPI.Buffer.SetGlobalTexture("_G_NormalWorldSpaceAtlas", SProps.GBuffer.G_NormalWorldSpaceAtlas);
            RAPI.Buffer.SetGlobalTexture("_GAux_ClearNormalWorldSpaceAtlas", SProps.GBuffer.GAux_ClearNormalWorldSpaceAtlas);
            RAPI.Buffer.SetGlobalTexture("_G_SpecularAtlas", SProps.GBuffer.G_SpecularAtlas);
            RAPI.Buffer.SetGlobalTexture("_G_BRDFAtlas", SProps.GBuffer.G_BRDFAtlas);

            
            RAPI.ExecuteBuffer();
            


        }
        
    }
}