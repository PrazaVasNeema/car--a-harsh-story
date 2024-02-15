using UnityEngine;
using UnityEngine.Rendering;


namespace CustomSRP.Runtime
{
    public class Shadows
    {
        private const int ACTIVE_LIGHT_INDEX = 0;
        private const string BUFFER_NAME = "Shadows";
        // private const int MAX_SHADOWED_DIRECTIONAL_LIGHT_COUNT = 4;
        
        public static int dirShadowAtlasId = Shader.PropertyToID("_DirectionalShadowAtlas");
        
        // struct ShadowedDirectionalLight {
        //     public int visibleLightIndex;
        // }
        
        private CommandBuffer m_buffer = new CommandBuffer {
            name = BUFFER_NAME
        };
        private ScriptableRenderContext m_context;
        private CullingResults m_cullingResults;
        private ShadowSettings m_settings;
        //private ShadowedDirectionalLight[] m_hadowedDirectionalLights = new ShadowedDirectionalLight[MAX_SHADOWED_DIRECTIONAL_LIGHT_COUNT];
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
                RenderDepthBuffer();
            }
            else {
                m_buffer.GetTemporaryRT(dirShadowAtlasId, 1, 1, 32, FilterMode.Bilinear, RenderTextureFormat.Shadowmap);
            }
        }

        private void RenderDepthBuffer()
        {
            int atlasSize = (int)m_settings.directional.atlasSize;
            
            m_buffer.GetTemporaryRT(dirShadowAtlasId, atlasSize, atlasSize, 32, FilterMode.Bilinear, RenderTextureFormat.Shadowmap); // RT = render texture
            //                                           To immediately clear target              The purpose
            m_buffer.SetRenderTarget(dirShadowAtlasId, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            m_buffer.ClearRenderTarget(true, false, Color.clear); // Why clear it here anyway?
            m_buffer.BeginSample(BUFFER_NAME);
            // RenderUtils.ExecuteBuffer(m_buffer, m_context);
            ExecuteBuffer();

            var shadowSettings = new ShadowDrawingSettings(m_cullingResults, ACTIVE_LIGHT_INDEX); // maybe here will be a problem
            
            m_cullingResults.ComputeDirectionalShadowMatricesAndCullingPrimitives(ACTIVE_LIGHT_INDEX, 0, 1,
                Vector3.zero, atlasSize, 0f,
                out Matrix4x4 viewMatrix, out Matrix4x4 projectionMatrix, out ShadowSplitData splitData);
            
            shadowSettings.splitData = splitData;

            m_buffer.SetViewport(new Rect(0, 0, atlasSize, atlasSize));
            m_buffer.SetViewProjectionMatrices(viewMatrix, projectionMatrix);

            Debug.Log($"proj: {projectionMatrix}, view: {viewMatrix}");
            Debug.Log($"proj*view: {projectionMatrix * viewMatrix} ");
            
            Shader.SetGlobalMatrix("_lightProjection", projectionMatrix * viewMatrix);
            //Shader.SetGlobalTexture(_shadowMap);
            m_buffer.SetGlobalTexture("_DirectionalShadowAtlas", dirShadowAtlasId);
            
            // RenderUtils.ExecuteBuffer(m_buffer,m_context);
            ExecuteBuffer();

            m_context.DrawShadows(ref shadowSettings);

            
            m_buffer.EndSample(BUFFER_NAME);
            // RenderUtils.ExecuteBuffer(m_buffer,m_context);
            ExecuteBuffer();


        }

        public void ReserveDirectionalShadows(Light light)
        {
            if (light.shadows != LightShadows.None && light.shadowStrength > 0f 
                                                   && m_cullingResults.GetShadowCasterBounds(0,
                                                       out Bounds b))
            {
                m_shadowedDirectionalLightCount = 1;
            }
        }
        
        public void Cleanup () {
            m_buffer.ReleaseTemporaryRT(dirShadowAtlasId);
            // RenderUtils.ExecuteBuffer(m_buffer, m_context);
            ExecuteBuffer();
        }
        
        
        void ExecuteBuffer () {
            m_context.ExecuteCommandBuffer(m_buffer);
            m_buffer.Clear();
        }

    }
    
    
}
