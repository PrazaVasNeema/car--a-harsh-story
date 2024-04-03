using UnityEngine;
using UnityEngine.Rendering;

namespace DopeRP.CPU
{
    public class SSAO
    {
        private const string BUFFER_NAME = "SSAO";
        
        // public static int SSAOAtlas = Shader.PropertyToID("_SSAOAtlas");
        // public static int SSAOAtlasBlurred = Shader.PropertyToID("_SSAOAtlasBlurred");

        private static Vector4[] samples = new Vector4[64];
        
        public void Render(SSAOSettings ssaoSettings)
        {
            
            RAPI.Buffer.name = "ssao";
            RAPI.Buffer.BeginSample("ssao");
            RAPI.ExecuteBuffer();
            
            
            
            RAPI.Buffer.GetTemporaryRT(SProps.SSAO.SSAORawAtlas, RAPI.CurCamera.pixelWidth/2, RAPI.CurCamera.pixelHeight/2, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat);
            RAPI.Buffer.SetRenderTarget(SProps.SSAO.SSAORawAtlas, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            RAPI.Buffer.ClearRenderTarget(true, true, Color.clear);
            
            RAPI.Buffer.SetGlobalVector(SProps.SSAO.ScreenSize, new Vector4(RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight, 0f, 0f));
            Matrix4x4 projectionMatrix = RAPI.CurCamera.projectionMatrix;
            RAPI.Buffer.SetGlobalMatrix(SProps.SSAO.LensProjection, projectionMatrix);
            RAPI.Buffer.SetGlobalVector(SProps.SSAO.NoiseScale, new Vector4(RAPI.CurCamera.pixelWidth/ssaoSettings.randomSize, RAPI.CurCamera.pixelHeight/ssaoSettings.randomSize, 0f, 0f));
            RAPI.SetKeywords(ssaoSettings.samplesCount);

            var camPos = RAPI.CurCamera.transform.position;
            RAPI.Buffer.SetGlobalVector(Shader.PropertyToID("_WorldSpaceCameraPos"), new Vector4(camPos.x, camPos.y, camPos.z, 0 ));
            RAPI.Buffer.SetGlobalVector(Shader.PropertyToID("_nearFarPlanes"), new Vector4(RAPI.CurCamera.nearClipPlane, RAPI.CurCamera.farClipPlane, 0, 0 ));
            RAPI.Buffer.SetGlobalMatrix(Shader.PropertyToID("adfgdgf_WorldToCameraMatrix"),  RAPI.CurCamera.worldToCameraMatrix);
            RAPI.Buffer.SetGlobalMatrix(Shader.PropertyToID("adfgdgf_CameraToWorldMatrix"),  RAPI.CurCamera.cameraToWorldMatrix);
            
            Matrix4x4 invProjectionMatrix = RAPI.CurCamera.projectionMatrix.inverse;
            RAPI.Buffer.SetGlobalMatrix(Shader.PropertyToID("_INVERSE_P"), invProjectionMatrix);
            
            // Generate();
            
            RAPI.Buffer.SetGlobalVectorArray(Shader.PropertyToID("SAMPLES"), samples);

            


            
            
            RAPI.ExecuteBuffer(); 
            Mesh fullscreenQuad = CreateFullscreenQuad();
            // RAPI.Buffer.SetViewport(new Rect(0, 0, Screen.width, Screen.height));
            
            // RAPI.Buffer.DrawMesh(RAPI.fullscreenTriangle, Matrix4x4.identity, ssaoSettings.SSAOMaterial, 0 , 0);
            
            RAPI.Buffer.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            RAPI.Buffer.DrawMesh(RAPI.fullscreenMesh, Matrix4x4.identity, ssaoSettings.SSAOMaterial, 0, 0);
            RAPI.Buffer.SetViewProjectionMatrices(RAPI.CurCamera.worldToCameraMatrix, RAPI.CurCamera.projectionMatrix);
            // RAPI.Buffer.DrawProcedural(Matrix4x4.identity, ssaoSettings.SSAOMaterial, 0, MeshTopology.Triangles, 3, 1,
                // null);
            
            // RAPI.Buffer.Blit(null, BuiltinRenderTextureType.CurrentActive, ssaoSettings.SSAOMaterial, ssaoSettings.SSAOMaterial.FindPass(SProps.SSAO.SSAORawPassName));

            RAPI.ExecuteBuffer();
            
            RAPI.Buffer.SetGlobalTexture("_SSAORawAtlas", SProps.SSAO.SSAORawAtlas);

            RAPI.Buffer.GetTemporaryRT(SProps.SSAO.SSAOBlurAtlas, RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat);
            RAPI.Buffer.SetRenderTarget(SProps.SSAO.SSAOBlurAtlas, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            RAPI.Buffer.ClearRenderTarget(true, true, Color.clear);
            
            RAPI.ExecuteBuffer();
            
            
            RAPI.Buffer.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            RAPI.Buffer.DrawMesh(RAPI.fullscreenMesh, Matrix4x4.identity, ssaoSettings.SSAOMaterial, 0, ssaoSettings.SSAOMaterial.FindPass(SProps.SSAO.SSAOBlurPassName));
            RAPI.Buffer.SetViewProjectionMatrices(RAPI.CurCamera.worldToCameraMatrix, RAPI.CurCamera.projectionMatrix);
            
            // RAPI.Buffer.Blit(null, BuiltinRenderTextureType.CurrentActive, ssaoSettings.SSAOMaterial, ssaoSettings.SSAOMaterial.FindPass(SProps.SSAO.SSAOBlurPassName));

            RAPI.ExecuteBuffer();

            RAPI.Buffer.SetGlobalTexture("_SSAOBlurAtlas", SProps.SSAO.SSAOBlurAtlas);
            
            RAPI.Buffer.EndSample("ssao");
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

                samples[i] = sample;
            }
        }
        Mesh CreateFullscreenQuad()
        {
            Mesh quad = new Mesh();
            quad.vertices = new Vector3[] {
                new Vector3(-1, -1, 0),
                new Vector3( 1, -1, 0),
                new Vector3( 1,  1, 0),
                new Vector3(-1,  1, 0)
            };
            quad.uv = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
            };
            quad.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
            return quad;
        }

    }
    
    
    
}
