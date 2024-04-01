using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace DopeRP.CPU
{
    public static class RAPI
    {
        private const string BUFFER_NAME = "DefaultBufferName";
        static Mesh s_FullscreenTriangle;
        public static CommandBuffer Buffer { get; private set; } = new CommandBuffer {
            name = BUFFER_NAME
        };
        public static ScriptableRenderContext Context { get; set; }
        public static Camera CurCamera { get; set; }
        public static CullingResults CullingResults { get; private set; }

        public static RenderTexture a;
        
        static Mesh s_FullscreenMesh = null;
    
        public static void ExecuteBuffer () {
            Context.ExecuteCommandBuffer(Buffer);
            Buffer.Clear();
        }
        

        public static void CleanupTempRT(int atlasID)
        {
            Buffer.ReleaseTemporaryRT(atlasID);
            ExecuteBuffer();
        }
    
        public static bool Cull(float maxShadowDistance)
        {
            if (CurCamera.TryGetCullingParameters(out ScriptableCullingParameters p))
            {
                p.shadowDistance = Mathf.Min(maxShadowDistance, CurCamera.farClipPlane);
                CullingResults = Context.Cull(ref p);
                return true;
            }
            return false;
        }
        
        public static void SetKeyword (string keyword, bool shouldBeSet) {
            if (shouldBeSet)
            {
                Buffer.EnableShaderKeyword(keyword);
            }
            else
            {
                Buffer.DisableShaderKeyword(keyword);
            }
        }
        
        public static void SetKeywords (string[] keywords, int enabledIndex) {
            for (int i = 0; i < keywords.Length; i++) {
                if (i == enabledIndex) {
                    Buffer.EnableShaderKeyword(keywords[i]);
                }
                else {
                    Buffer.DisableShaderKeyword(keywords[i]);
                }
            }
        }
        
        public static void SetKeywords <TEnum> (TEnum enabledIndex) where TEnum : Enum
        {
            var values = Enum.GetValues(typeof(TEnum));

            for (int i = 0; i < values.Length; i++)
            {
                var value = values.GetValue(i);
                string keyword = value.ToString();
                
                if(value.Equals(enabledIndex))
                {
                    Buffer.EnableShaderKeyword(keyword);
                }
                else
                    Buffer.DisableShaderKeyword(keyword);
            }
        }
        
        public static Mesh fullscreenTriangle
        {
            get
            {
                if (s_FullscreenTriangle != null)
                    return s_FullscreenTriangle;

                s_FullscreenTriangle = new Mesh { name = "Fullscreen Triangle" };

                // Because we have to support older platforms (GLES2/3, DX9 etc) we can't do all of
                // this directly in the vertex shader using vertex ids :(
                s_FullscreenTriangle.SetVertices(new List<Vector3>
                {
                    new Vector3(-1f, -1f, 0f),
                    new Vector3(-1f,  3f, 0f),
                    new Vector3( 3f, -1f, 0f)
                });

                s_FullscreenTriangle.SetIndices(new [] { 0, 1, 2 }, MeshTopology.Triangles, 0, false);
                s_FullscreenTriangle.UploadMeshData(false);

                return s_FullscreenTriangle;
            }
        }
        
        public static Mesh fullscreenMesh
        {
            get
            {
                if (s_FullscreenMesh != null)
                    return s_FullscreenMesh;

                float topV = 1.0f;
                float bottomV = 0.0f;

                s_FullscreenMesh = new Mesh { name = "Fullscreen Quad" };
                s_FullscreenMesh.SetVertices(new List<Vector3>
                {
                    new Vector3(-1.0f, -1.0f, 0.0f),
                    new Vector3(-1.0f,  1.0f, 0.0f),
                    new Vector3(1.0f, -1.0f, 0.0f),
                    new Vector3(1.0f,  1.0f, 0.0f)
                });

                s_FullscreenMesh.SetUVs(0, new List<Vector2>
                {
                    new Vector2(0.0f, bottomV),
                    new Vector2(0.0f, topV),
                    new Vector2(1.0f, bottomV),
                    new Vector2(1.0f, topV)
                });

                s_FullscreenMesh.SetIndices(new[] { 0, 1, 2, 2, 1, 3 }, MeshTopology.Triangles, 0, false);
                s_FullscreenMesh.UploadMeshData(true);
                return s_FullscreenMesh;
            }
        }
    }
}
