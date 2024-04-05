using UnityEngine;
using UnityEngine.Rendering;

namespace DopeRP.CPU
{
    public class Decals
    {
        private const string BUFFER_NAME = "Decals";
        

        public void Render()
        {

            RAPI.BeginSample(BUFFER_NAME);
            // RAPI.Buffer = new CommandBuffer
            // {
            //     name = "decal"
            // };
            // RAPI.Buffer.name = "decal";
            // RAPI.Buffer.BeginSample("decal");
            // RAPI.ExecuteBuffer();
            
            
            RAPI.ExecuteBuffer();

            RenderTargetIdentifier[] colorTargets = {
                new RenderTargetIdentifier(SProps.GBuffer.G_AlbedoAtlas),
                new RenderTargetIdentifier(SProps.GBuffer.G_NormalWorldSpaceAtlas),
                new RenderTargetIdentifier(SProps.GBuffer.G_BRDFAtlas)
            };

            RAPI.Buffer.SetRenderTarget(colorTargets, BuiltinRenderTextureType.None);
            // RAPI.Buffer.ClearRenderTarget(true, true, Color.clear);
            RAPI.ExecuteBuffer();
            
            var sortingSettings = new SortingSettings(RAPI.CurCamera)
            {
                criteria = SortingCriteria.CommonTransparent
            };
            var drawingSettings = new DrawingSettings(SProps.Decals.DecalsPassId, sortingSettings)
            {
                enableDynamicBatching = false,
                enableInstancing = true,
            };
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

            RAPI.Context.DrawRenderers(RAPI.CullingResults, ref drawingSettings, ref filteringSettings);
            
            
            RAPI.EndSample(BUFFER_NAME);
            // RAPI.Buffer.EndSample("decal");
            // RAPI.ExecuteBuffer();
            
            
            
        }
       
    }
}
