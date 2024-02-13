using UnityEngine;
using UnityEngine.Rendering;


namespace CustomSRP.Runtime
{
    public class Shadows
    {
        private const string BUFFER_NAME = "Shadows";
        private const int MAX_SHADOWED_DIRECTIONAL_LIGHT_COUNT = 4;
        
        private static int dirShadowAtlasId = Shader.PropertyToID("_DirectionalShadowAtlas");
        
        struct ShadowedDirectionalLight {
            public int visibleLightIndex;
        }
        
        CommandBuffer buffer = new CommandBuffer {
            name = BUFFER_NAME
        };
        
        private ScriptableRenderContext m_context;
        private CullingResults m_cullingResults;
        private ShadowSettings m_settings;
        private ShadowedDirectionalLight[] m_hadowedDirectionalLights = new ShadowedDirectionalLight[MAX_SHADOWED_DIRECTIONAL_LIGHT_COUNT];
        private int m_shadowedDirectionalLightCount;

        
    
        public void Setup (ScriptableRenderContext context, CullingResults cullingResults, ShadowSettings settings)
        {
            m_context = context;
            m_cullingResults = cullingResults;
            m_settings = settings;
            m_shadowedDirectionalLightCount = 0;
        }
        public void Render () {
            if (m_shadowedDirectionalLightCount > 0) {
                RenderDirectionalShadows();
            }
            else {
                buffer.GetTemporaryRT(dirShadowAtlasId, 1, 1, 32, FilterMode.Bilinear, RenderTextureFormat.Shadowmap);
            }
        }
    }
    
    
}
