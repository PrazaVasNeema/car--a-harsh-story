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
            RAPI.Buffer.GetTemporaryRT(SProps.GBuffer.PositionViewSpaceAtlas, (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat);
            RAPI.Buffer.GetTemporaryRT(SProps.GBuffer.NormalViewSpaceAtlas, (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Point, RenderTextureFormat.ARGBFloat);
            RAPI.Buffer.GetTemporaryRT(SProps.GBuffer.TangentViewSpaceAtlas, (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat);
            RAPI.Buffer.GetTemporaryRT(Shader.PropertyToID("Test"), (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 32, FilterMode.Bilinear, RenderTextureFormat.Depth);
            
            RAPI.Buffer.GetTemporaryRT(SProps.GBuffer.G_AlbedoAtlas, (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            RAPI.Buffer.GetTemporaryRT(SProps.GBuffer.G_NormalWorldSpaceAtlas, (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf);
            RAPI.Buffer.GetTemporaryRT(SProps.GBuffer.G_SpecularAtlas, (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            RAPI.Buffer.GetTemporaryRT(SProps.GBuffer.G_BRDFAtlas, (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Point, RenderTextureFormat.ARGB32);
            
            RenderTargetIdentifier[] colorTargets = {
                new RenderTargetIdentifier(SProps.GBuffer.PositionViewSpaceAtlas),
                new RenderTargetIdentifier(SProps.GBuffer.NormalViewSpaceAtlas),
                new RenderTargetIdentifier(SProps.GBuffer.TangentViewSpaceAtlas),
                
                new RenderTargetIdentifier(SProps.GBuffer.G_AlbedoAtlas),
                new RenderTargetIdentifier(SProps.GBuffer.G_NormalWorldSpaceAtlas),
                new RenderTargetIdentifier(SProps.GBuffer.G_SpecularAtlas),
                new RenderTargetIdentifier(SProps.GBuffer.G_BRDFAtlas)
            };
            RAPI.Buffer.SetRenderTarget(colorTargets, Shader.PropertyToID("Test"));
            RAPI.Buffer.ClearRenderTarget(true, true, Color.clear);
            
            RAPI.Buffer.SetGlobalFloat(SProps.GBuffer.CameraNearPlane, RAPI.CurCamera.nearClipPlane);
            RAPI.Buffer.SetGlobalFloat(SProps.GBuffer.CameraFarPlane, RAPI.CurCamera.farClipPlane);

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
            

            RAPI.Buffer.SetGlobalTexture("_PositionViewSpace", SProps.GBuffer.PositionViewSpaceAtlas);
            RAPI.Buffer.SetGlobalTexture("_NormalViewSpace", SProps.GBuffer.NormalViewSpaceAtlas);
            RAPI.Buffer.SetGlobalTexture("_TangentViewSpace", SProps.GBuffer.TangentViewSpaceAtlas);
            RAPI.Buffer.SetGlobalTexture("Test", Shader.PropertyToID("Test"));
            
            RAPI.Buffer.SetGlobalTexture("_G_AlbedoAtlas", SProps.GBuffer.G_AlbedoAtlas);
            RAPI.Buffer.SetGlobalTexture("_G_NormalWorldSpaceAtlas", SProps.GBuffer.G_NormalWorldSpaceAtlas);
            RAPI.Buffer.SetGlobalTexture("_G_SpecularAtlas", SProps.GBuffer.G_SpecularAtlas);
            RAPI.Buffer.SetGlobalTexture("_G_BRDFAtlas", SProps.GBuffer.G_BRDFAtlas);

            
            
            RAPI.ExecuteBuffer();
            
        }
        
    }
}