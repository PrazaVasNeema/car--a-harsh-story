using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
    public class GBuffers
    {
        private const string BUFFER_NAME = "GBuffer";
        
        private static ShaderTagId GBufferPassId = new ShaderTagId("GBufferPass");
        
        public static int positionViewSpaceAtlas = Shader.PropertyToID("_PositionViewSpace");
        public static int normalViewSpaceAtlas = Shader.PropertyToID("_NormalViewSpace");

        
        public void Render()
        {
            RAPI.Buffer.BeginSample(BUFFER_NAME);
            RAPI.ExecuteBuffer();

            Vector2 cameraWidthHeight = new Vector2(RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight);
            RAPI.Buffer.GetTemporaryRT(positionViewSpaceAtlas, (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat);
            RAPI.Buffer.GetTemporaryRT(normalViewSpaceAtlas, (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBHalf);

            RenderTargetIdentifier[] colorTargets = {
                new RenderTargetIdentifier(positionViewSpaceAtlas),
                new RenderTargetIdentifier(normalViewSpaceAtlas)
            };
            
            // RAPI.Buffer.SetRenderTarget(positionViewSpaceAtlas, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            RAPI.Buffer.SetRenderTarget(colorTargets, BuiltinRenderTextureType.CameraTarget);
            RAPI.Buffer.ClearRenderTarget(true, true, Color.clear);

            RAPI.ExecuteBuffer();

            var sortingSettings = new SortingSettings(RAPI.CurCamera)
            {
                criteria = SortingCriteria.CommonOpaque
            };

            var drawingSettings = new DrawingSettings(GBufferPassId, sortingSettings)
            {
                enableDynamicBatching = false,
                enableInstancing = true,
            };
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

            RAPI.Context.DrawRenderers(RAPI.CullingResults, ref drawingSettings, ref filteringSettings);
            
            
            RAPI.ExecuteBuffer();

            RAPI.Buffer.SetGlobalTexture("_PositionViewSpace", positionViewSpaceAtlas);
            
            RAPI.Buffer.EndSample(BUFFER_NAME);
            RAPI.ExecuteBuffer();


            //
            //
            // // Material customMaterial = new Material(Shader.Find("Custom/GeometryPass"));
            // int width = RAPI.CurCamera.pixelWidth;
            // int height = RAPI.CurCamera.pixelHeight;
            // RenderTexture albedoTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            // RenderTexture depthTexture = new RenderTexture(width, height, 24, RenderTextureFormat.Depth);
            // RenderTexture positionTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
            //
            // albedoTexture.Create();
            // depthTexture.Create();
            // positionTexture.Create();
            //
            // RenderBuffer[] colorBuffers = { albedoTexture.colorBuffer, positionTexture.colorBuffer };
            // RenderBuffers colorBuferrs = new RenderBuffers();
            // RenderBuffer colorBuffer = albedoTexture.colorBuffer;
            // RenderBuffer depthBuffer = depthTexture.depthBuffer;
            //
            // RAPI.Buffer.SetRenderTarget(new RenderTargetIdentifier[] { albedoTexture, positionTexture }, depthBuffer);
            //
            // RAPI.Buffer.ClearRenderTarget(true, true, Color.clear);
            //
            // var sortingSettings = new SortingSettings(RAPI.CurCamera)
            // {
            //     criteria = SortingCriteria.CommonOpaque
            // };
            //
            // var drawingSettings = new DrawingSettings(RenderBuffersId, sortingSettings)
            // {
            //     enableDynamicBatching = false,
            //     enableInstancing = true,
            // };
            // var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
            //
            // RAPI.Context.DrawRenderers(RAPI.CullingResults, ref drawingSettings, ref filteringSettings);
            //
            // RAPI.ExecuteBuffer();
            //
            //

            // RAPI.Buffer.SetGlobalTexture(atlas, albedoTexture);
        }
        
    }
}