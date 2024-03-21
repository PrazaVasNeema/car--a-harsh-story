using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
    public static class RAPI
    {
        private const string BUFFER_NAME = "DefaultBufferName";
    
        public static CommandBuffer Buffer { get; private set; } = new CommandBuffer {
            name = BUFFER_NAME
        };
        public static ScriptableRenderContext Context { get; set; }
        public static Camera CurCamera { get; set; }
        public static CullingResults CullingResults { get; private set; }

        public static RenderTexture a;
    
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
    }
}
