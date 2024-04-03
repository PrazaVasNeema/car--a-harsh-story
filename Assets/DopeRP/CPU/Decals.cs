using UnityEngine;
using UnityEngine.Rendering;

namespace DopeRP.CPU
{
    public class Decals
    {
        private const string BUFFER_NAME = "Decals";
        

        public void Render()
        {

            // for (int i = 0; i < 10000; i++)
            // {
            //     Generate();
            // }
            RAPI.Buffer = new CommandBuffer
            {
                name = "decal"
            };
            RAPI.Buffer.name = "decal";
            RAPI.Buffer.BeginSample("decal");
            RAPI.ExecuteBuffer();


            Vector2 cameraWidthHeight = new Vector2(RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight);
            // RAPI.Buffer.GetTemporaryRT(SProps.Decals.DecalsDamageAlbedoAtlas, (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            // RAPI.Buffer.GetTemporaryRT(SProps.Decals.DecalsDamageNormalAtlas, (int)cameraWidthHeight.x, (int)cameraWidthHeight.y, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf);
            RenderTargetIdentifier[] colorTargets = {
                new RenderTargetIdentifier(SProps.GBuffer.G_AlbedoAtlas),
                new RenderTargetIdentifier(SProps.GBuffer.G_NormalWorldSpaceAtlas),
                new RenderTargetIdentifier(SProps.GBuffer.G_BRDFAtlas)
            };
            
            RAPI.Buffer.SetRenderTarget(colorTargets, Shader.PropertyToID("Test"));
            RAPI.Buffer.SetGlobalVector(SProps.Decals.ScreenSize, new Vector4((int)cameraWidthHeight.x, (int)cameraWidthHeight.y,
                (float)1.0/cameraWidthHeight.x, (float)1.0/cameraWidthHeight.y));
            Matrix4x4 invProjectionMatrix = RAPI.CurCamera.projectionMatrix.inverse;
            RAPI.Buffer.SetGlobalMatrix(Shader.PropertyToID("_INVERSE_P"), invProjectionMatrix);
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
            
            // RAPI.ExecuteBuffer();
            
           
            // RAPI.Buffer.SetGlobalTexture("_DecalsAtlas", SProps.Decals.DecalsDamageAlbedoAtlas);
            // RAPI.Buffer.SetGlobalTexture("_DecalsAtlasNormals", SProps.Decals.DecalsDamageNormalAtlas);

            RAPI.Buffer.EndSample("decal");
            RAPI.ExecuteBuffer();
            
        }
        
 
        private void Generate()
        {


            for (int i = 0; i < 64; ++i)
            {
                Vector3 sample = new Vector3(
                    Random.Range(-1.0f, 1.0f), 
                    Random.Range(-1.0f, 1.0f), 
                    Random.Range(0.0f, 1.0f)
                );

                sample = sample.normalized;
                sample *= Random.Range(0.0f, 1.0f);

                float scale = (float)i / 64.0f;
                scale = Mathf.Lerp(0.1f, 1.0f, scale * scale);
                sample *= scale;

            }
        }
       
    }
}
