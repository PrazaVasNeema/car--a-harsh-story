using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
    public class Decals
    {
        private const string BUFFER_NAME = "Decals";


        private static ShaderTagId DecalsPassId = new ShaderTagId("DecalsPass");
        
        public static int DecalsAtlas = Shader.PropertyToID("_DecalsAtlas");
        
        public static int DecalsAtlasNormals = Shader.PropertyToID("_DecalsAtlasNormals");

        
        
        




        public void Render()
        {
            
            RAPI.Buffer.BeginSample(BUFFER_NAME);
            RAPI.ExecuteBuffer();
            
            RAPI.Buffer.GetTemporaryRT(DecalsAtlas, RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            RAPI.Buffer.GetTemporaryRT(DecalsAtlasNormals, RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat);
            RenderTargetIdentifier[] colorTargets = {
                new RenderTargetIdentifier(DecalsAtlas),
                new RenderTargetIdentifier(DecalsAtlasNormals)
            };
            
            // RAPI.Buffer.SetRenderTarget(DecalsAtlas, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            RAPI.Buffer.SetRenderTarget(colorTargets, BuiltinRenderTextureType.CameraTarget);
            RAPI.Buffer.ClearRenderTarget(true, true, Color.clear);
            RAPI.Buffer.SetGlobalVector("_ScreenSize", new Vector4(RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight,
                1/RAPI.CurCamera.pixelWidth, 1/RAPI.CurCamera.pixelHeight));

            
            RAPI.ExecuteBuffer();

            var sortingSettings = new SortingSettings(RAPI.CurCamera)
            {
                criteria = SortingCriteria.CommonOpaque
            };

            var drawingSettings = new DrawingSettings(DecalsPassId, sortingSettings)
            {
                enableDynamicBatching = false,
                enableInstancing = true,
            };
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

            RAPI.Context.DrawRenderers(RAPI.CullingResults, ref drawingSettings, ref filteringSettings);
            
            
            RAPI.ExecuteBuffer();
           
            RAPI.Buffer.SetGlobalTexture("_DecalsAtlas", DecalsAtlas);
            RAPI.Buffer.SetGlobalTexture("_DecalsAtlasNormals", DecalsAtlasNormals);

            RAPI.Buffer.EndSample(BUFFER_NAME);
            RAPI.ExecuteBuffer();
            
        }
        
 

       
    }
}
