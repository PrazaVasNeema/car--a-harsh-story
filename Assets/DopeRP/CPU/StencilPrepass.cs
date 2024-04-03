using UnityEngine;
using UnityEngine.Rendering;

namespace DopeRP.CPU
{
    public class StencilPrepass
    {
        private const string BUFFER_NAME = "GBuffer";
        
        
        public void Render()
        {
            
            
            
            RAPI.ExecuteBuffer();
            Vector2 cameraWidthHeight = new Vector2(RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight);

            RAPI.Buffer.GetTemporaryRT(Shader.PropertyToID("1"), (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf);
            RAPI.Buffer.GetTemporaryRT(Shader.PropertyToID("Test"), (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 32, FilterMode.Bilinear, RenderTextureFormat.Depth);

            RenderTargetIdentifier[] colorTargets =
            {
                new RenderTargetIdentifier(Shader.PropertyToID("1")),
            };
                
            RAPI.Buffer.SetRenderTarget(colorTargets, Shader.PropertyToID("Test"));
            RAPI.Buffer.ClearRenderTarget(true, true, Color.clear);
            
            RAPI.ExecuteBuffer();

            var sortingSettings = new SortingSettings(RAPI.CurCamera)
            {
                criteria = SortingCriteria.CommonTransparent
            };

            
            var drawingSettings = new DrawingSettings(new ShaderTagId("gfg"), sortingSettings)
            {
                enableDynamicBatching = false,
                enableInstancing = true,
            };

            
            
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

            RAPI.Context.DrawRenderers(RAPI.CullingResults, ref drawingSettings, ref filteringSettings);
            
            RAPI.ExecuteBuffer();
            

            RAPI.Buffer.SetGlobalTexture("Test", Shader.PropertyToID("Test"));


            
            
            RAPI.ExecuteBuffer();
            
        }
        
    }
}