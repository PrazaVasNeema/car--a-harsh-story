using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
    public class GBuffers
    {
        private const string BUFFER_NAME = "GBuffer";
        
        
        public void Render()
        {
            
            
            RAPI.ExecuteBuffer();
            

            Vector2 cameraWidthHeight = new Vector2(RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight);
            RAPI.Buffer.GetTemporaryRT(SProps.GBuffer.PositionViewSpaceAtlas, (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat);
            RAPI.Buffer.GetTemporaryRT(SProps.GBuffer.NormalViewSpaceAtlas, (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat);
            RAPI.Buffer.GetTemporaryRT(SProps.GBuffer.TangentViewSpaceAtlas, (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat);

            RenderTargetIdentifier[] colorTargets = {
                new RenderTargetIdentifier(SProps.GBuffer.PositionViewSpaceAtlas),
                new RenderTargetIdentifier(SProps.GBuffer.NormalViewSpaceAtlas),
                new RenderTargetIdentifier(SProps.GBuffer.TangentViewSpaceAtlas)

            };
            
            RAPI.Buffer.SetRenderTarget(colorTargets, BuiltinRenderTextureType.CameraTarget);
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
            };
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

            RAPI.Context.DrawRenderers(RAPI.CullingResults, ref drawingSettings, ref filteringSettings);
            
            RAPI.ExecuteBuffer();
            

            RAPI.Buffer.SetGlobalTexture("_PositionViewSpace", SProps.GBuffer.PositionViewSpaceAtlas);
            RAPI.Buffer.SetGlobalTexture("_NormalViewSpace", SProps.GBuffer.NormalViewSpaceAtlas);
            RAPI.Buffer.SetGlobalTexture("_TangentViewSpace", SProps.GBuffer.TangentViewSpaceAtlas);
            
            
            RAPI.ExecuteBuffer();
            
        }
        
    }
}