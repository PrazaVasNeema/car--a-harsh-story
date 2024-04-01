using UnityEngine;
using UnityEngine.Rendering;

namespace DopeRP.CPU
{
    public class Decals
    {
        private const string BUFFER_NAME = "Decals";
        

        public void Render()
        {
            
            RAPI.Buffer.ClearRenderTarget(true, true, Color.clear);

            RAPI.ExecuteBuffer();
            

            Vector2 cameraWidthHeight = new Vector2(RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight);
            RAPI.Buffer.GetTemporaryRT(SProps.Decals.DecalsDamageAlbedoAtlas, (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            RAPI.Buffer.GetTemporaryRT(SProps.Decals.DecalsDamageNormalAtlas, (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf);
            RenderTargetIdentifier[] colorTargets = {
                new RenderTargetIdentifier(SProps.GBuffer.G_AlbedoAtlas),
                new RenderTargetIdentifier(SProps.GBuffer.G_NormalWorldSpaceAtlas),
                new RenderTargetIdentifier(SProps.Decals.DecalsDamageAlbedoAtlas),
                new RenderTargetIdentifier(SProps.Decals.DecalsDamageNormalAtlas)
            };
            
            RAPI.Buffer.SetRenderTarget(colorTargets, BuiltinRenderTextureType.CameraTarget);
            RAPI.Buffer.SetGlobalVector(SProps.Decals.ScreenSize, new Vector4((int)cameraWidthHeight.x, (int)cameraWidthHeight.y,
                (float)1.0/cameraWidthHeight.x, (float)1.0/cameraWidthHeight.y));
            
            RAPI.ExecuteBuffer();
            

            var sortingSettings = new SortingSettings(RAPI.CurCamera)
            {
                criteria = SortingCriteria.CommonOpaque
            };

            var drawingSettings = new DrawingSettings(SProps.Decals.DecalsPassId, sortingSettings)
            {
                enableDynamicBatching = false,
                enableInstancing = true,
            };
            var filteringSettings = new FilteringSettings(RenderQueueRange.transparent);

            RAPI.Context.DrawRenderers(RAPI.CullingResults, ref drawingSettings, ref filteringSettings);
            
            RAPI.ExecuteBuffer();
            
           
            RAPI.Buffer.SetGlobalTexture("_DecalsAtlas", SProps.Decals.DecalsDamageAlbedoAtlas);
            RAPI.Buffer.SetGlobalTexture("_DecalsAtlasNormals", SProps.Decals.DecalsDamageNormalAtlas);

            
            RAPI.ExecuteBuffer();
            
        }
        
 

       
    }
}
