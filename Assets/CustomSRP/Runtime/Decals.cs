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
        
        
        




        public void Render()
        {
            
            RAPI.Buffer.BeginSample(BUFFER_NAME);
            RAPI.ExecuteBuffer();
            
            RAPI.Buffer.GetTemporaryRT(DecalsAtlas, RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            RAPI.Buffer.SetRenderTarget(DecalsAtlas, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
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

            RAPI.Buffer.EndSample(BUFFER_NAME);
            RAPI.ExecuteBuffer();
            
        }
        
 

       
    }
}
