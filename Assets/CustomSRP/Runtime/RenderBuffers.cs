using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
    public class RenderBuffers
    {
        private static ShaderTagId RenderBuffersId = new ShaderTagId("GeometryPass");
        
        public static int atlas = Shader.PropertyToID("_atlas1");
        
        public void Render()
        {
            // Material customMaterial = new Material(Shader.Find("Custom/GeometryPass"));
            int width = RAPI.CurCamera.pixelWidth;
            int height = RAPI.CurCamera.pixelHeight;
            RenderTexture albedoTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            RenderTexture depthTexture = new RenderTexture(width, height, 24, RenderTextureFormat.Depth);
            RenderTexture positionTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
            
            albedoTexture.Create();
            depthTexture.Create();
            positionTexture.Create();

            RenderBuffer[] colorBuffers = { albedoTexture.colorBuffer, positionTexture.colorBuffer };
            RenderBuffers colorBuferrs = new RenderBuffers();
            RenderBuffer colorBuffer = albedoTexture.colorBuffer;
            RenderBuffer depthBuffer = depthTexture.depthBuffer;
            
            RAPI.Buffer.SetRenderTarget(new RenderTargetIdentifier[] { albedoTexture, positionTexture }, depthBuffer);
            
            RAPI.Buffer.ClearRenderTarget(true, true, Color.clear);
            
            var sortingSettings = new SortingSettings(RAPI.CurCamera)
            {
                criteria = SortingCriteria.CommonOpaque
            };

            var drawingSettings = new DrawingSettings(RenderBuffersId, sortingSettings)
            {
                enableDynamicBatching = false,
                enableInstancing = true,
            };
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
            
            RAPI.Context.DrawRenderers(RAPI.CullingResults, ref drawingSettings, ref filteringSettings);

            RAPI.ExecuteBuffer();
            
            

            // RAPI.Buffer.SetGlobalTexture(atlas, albedoTexture);
        }
        
    }
}