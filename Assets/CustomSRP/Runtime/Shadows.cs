using UnityEngine;
using UnityEngine.Rendering;


namespace CustomSRP.Runtime
{
    public class Shadows
    {
        private const string BUFFER_NAME = "Shadows";

        private const int ACTIVE_LIGHT_INDEX = 0;
        private const int MAX_CASCADES = 4;
        // private const int MAX_SHADOWED_DIRECTIONAL_LIGHT_COUNT = 4;
        
        public static int dirShadowAtlasId = Shader.PropertyToID("_DirectionalShadowAtlas");
        public static int dirShadowMatricesId = Shader.PropertyToID("_DirectionalShadowMatrices");
        public static int cascadeCountId = Shader.PropertyToID("_CascadeCount");
        public static int cascadeCullingSpheresId = Shader.PropertyToID("_CascadeCullingSpheres");
        public static int cascadeDataId = Shader.PropertyToID("_CascadeData");
        public static int shadowAtlasSizeId = Shader.PropertyToID("_ShadowAtlasSize");
        public static int shadowDistanceFadeId = Shader.PropertyToID("_ShadowDistanceFade");
        private static int dirCascades = Shader.PropertyToID("_CascadeCount");

        private static Vector4[] cascadeCullingSpheres = new Vector4[MAX_CASCADES];
        private static Vector4[] cascadeData = new Vector4[MAX_CASCADES];
        private static Matrix4x4[] dirShadowMatrices = new Matrix4x4[MAX_CASCADES];

        
        struct ShadowedDirectionalLight {
            public float slopeScaleBias;
            public float nearPlaneOffset;
        }

        private ShadowedDirectionalLight m_light;
        
        // struct ShadowedDirectionalLight {
        //     public int visibleLightIndex;
        // }
        

        private ShadowSettings m_settings;
        //private ShadowedDirectionalLight[] m_hadowedDirectionalLights = new ShadowedDirectionalLight[MAX_SHADOWED_DIRECTIONAL_LIGHT_COUNT];


        
        static string[] directionalFilterKeywords = {
            "_DIRECTIONAL_PCF_NONE",
            "_DIRECTIONAL_PCF2x2",
            "_DIRECTIONAL_PCF4x4",
            "_DIRECTIONAL_PCF6x6",
            "_DIRECTIONAL_PCF8x8",
        };
        
        static string[] cascadeBlendKeywords = {
            "_CASCADE_BLEND_SOFT",
            "_CASCADE_BLEND_DITHER"
        };
        
        static string[] cascadesKeywords = {
            "CASCEDE_COUNT_2",
            "CASCEDE_COUNT_4"
        };

        private bool ready;
        public void Setup (ShadowSettings settings)
        {
            m_settings = settings;
            ready = false;
        }
        
        public void Render () {
            if (ready) {
                RenderDepthBuffer();
            }
            else {
                RAPI.Buffer.GetTemporaryRT(dirShadowAtlasId, 1, 1, 16, FilterMode.Bilinear, RenderTextureFormat.Shadowmap);
            }
        }

        private void RenderDepthBuffer()
        {
            int atlasSize = (int)m_settings.directional.atlasSize;
            RAPI.Buffer.GetTemporaryRT(dirShadowAtlasId, atlasSize, atlasSize, 32, FilterMode.Bilinear, RenderTextureFormat.Shadowmap);
            RAPI.Buffer.SetRenderTarget(dirShadowAtlasId, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            RAPI.Buffer.ClearRenderTarget(true, false, Color.clear);
            RAPI.Buffer.BeginSample(BUFFER_NAME);

            int cascadesCount = m_settings.directional.cascades == ShadowSettings.Cascades._2X ? 2 : 4;
            
            int tiles = cascadesCount;
            int split = tiles <= 1 ? 1 : tiles <= 4 ? 2 : 4;
            int tileSize = atlasSize / split;
            
            //
            
            var shadowSettings = new ShadowDrawingSettings(RAPI.CullingResults, 0, BatchCullingProjectionType.Orthographic);
            int cascadeCount = cascadesCount;
            int tileOffset = 0 * cascadeCount;
            Vector3 ratios = m_settings.directional.CascadeRatios;
            
            float cullingFactor = Mathf.Max(0f, 0.8f - m_settings.directional.cascadeFade);
            
            for (int i = 0; i < cascadeCount; i++) {
                RAPI.CullingResults.ComputeDirectionalShadowMatricesAndCullingPrimitives(0, i, cascadeCount, ratios, tileSize, m_light.nearPlaneOffset, out Matrix4x4 viewMatrix, out Matrix4x4 projectionMatrix, out ShadowSplitData splitData);
                splitData.shadowCascadeBlendCullingFactor = cullingFactor;
                shadowSettings.splitData = splitData;
                
                SetCascadeData(i, splitData.cullingSphere, tileSize);
                
                int tileIndex = tileOffset + i;
                dirShadowMatrices[tileIndex] = ConvertToAtlasMatrix(projectionMatrix * viewMatrix, SetTileViewport(tileIndex, split, tileSize), split);
                RAPI.Buffer.SetViewProjectionMatrices(viewMatrix, projectionMatrix);
                RAPI.Buffer.SetGlobalDepthBias(0f, m_light.slopeScaleBias);
                RAPI.ExecuteBuffer();
                RAPI.Context.DrawShadows(ref shadowSettings);
                RAPI.Buffer.SetGlobalDepthBias(0f, 0f);
            }
            
            // RAPI.Buffer.SetGlobalInt(cascadeCountId, cascadesCount);
            RAPI.SetKeywords(cascadesKeywords, cascadeCount / 2 - 1);
            RAPI.Buffer.SetGlobalVectorArray(cascadeCullingSpheresId, cascadeCullingSpheres);
            RAPI.Buffer.SetGlobalVectorArray(cascadeDataId, cascadeData);
            RAPI.Buffer.SetGlobalMatrixArray(dirShadowMatricesId, dirShadowMatrices);
            float f = 1f - m_settings.directional.cascadeFade;
            RAPI.Buffer.SetGlobalVector(shadowDistanceFadeId, new Vector4(1f / m_settings.maxDistance, 1f / m_settings.distanceFade, 1f / (1f - f * f)));
            RAPI.SetKeywords(directionalFilterKeywords, (int)m_settings.directional.filter);
            RAPI.SetKeywords(cascadeBlendKeywords, (int)m_settings.directional.cascadeBlend);
            RAPI.Buffer.SetGlobalVector(shadowAtlasSizeId, new Vector4(atlasSize, 1f / atlasSize));
            RAPI.Buffer.SetGlobalFloat(dirCascades, cascadeCount);
            RAPI.Buffer.EndSample(BUFFER_NAME);
            RAPI.ExecuteBuffer();
            
            //
            //
            // int atlasSize = (int)m_settings.directional.atlasSize;
            //
            // m_buffer.GetTemporaryRT(dirShadowAtlasId, atlasSize, atlasSize, 16, FilterMode.Point, RenderTextureFormat.Shadowmap); // RT = render texture
            // //                                           To immediately clear target              The purpose
            // m_buffer.SetRenderTarget(dirShadowAtlasId, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            // m_buffer.ClearRenderTarget(true, false, Color.clear); // Why clear it here anyway?
            // m_buffer.BeginSample(BUFFER_NAME);
            // // RenderUtils.ExecuteBuffer(m_buffer, m_context);
            // ExecuteBuffer();
            //
            // var shadowSettings = new ShadowDrawingSettings(m_cullingResults, ACTIVE_LIGHT_INDEX, BatchCullingProjectionType.Orthographic); // maybe here will be a problem
            //
            // m_cullingResults.ComputeDirectionalShadowMatricesAndCullingPrimitives(ACTIVE_LIGHT_INDEX, 0, 1,
            //     Vector3.zero, atlasSize, 0f,
            //     out Matrix4x4 viewMatrix, out Matrix4x4 projectionMatrix, out ShadowSplitData splitData);
            //
            // // Vector3 lightPosition = new Vector3(-2.0f, 4.0f, -1.0f);
            // // Vector3 lightTarget = Vector3.zero;
            // // Vector3 up = Vector3.up; // World up direction is usually (0, 1, 0)
            // //
            // // viewMatrix = Matrix4x4.LookAt(lightPosition, lightTarget, up);
            // //
            // // float left = -10.0f;
            // // float right = 10.0f;
            // // float bottom = -10.0f;
            // // float top = 10.0f;
            // // float near_plane = 1.0f; // Near plane distance
            // // float far_plane = 100.0f; // Far plane distance
            // //
            // // projectionMatrix = Matrix4x4.Ortho(left, right, bottom, top, near_plane, far_plane);
            //
            //
            // shadowSettings.splitData = splitData;
            //
            // m_buffer.SetViewport(new Rect(0, 0, atlasSize, atlasSize));
            // m_buffer.SetViewProjectionMatrices(viewMatrix, projectionMatrix);
            //
            // Debug.Log($"proj: {projectionMatrix}, view: {viewMatrix}");
            // Debug.Log($"proj*view: {projectionMatrix * viewMatrix} ");
            //
            //
            //
            // Shader.SetGlobalMatrix("_lightProjection", projectionMatrix * viewMatrix);
            // Shader.SetGlobalVector("_lightDir", m_lightDir);
            // //Shader.SetGlobalTexture(_shadowMap);
            // m_buffer.SetGlobalTexture("_DirectionalShadowAtlas", dirShadowAtlasId);
            //
            // // RenderUtils.ExecuteBuffer(m_buffer,m_context);
            // ExecuteBuffer();
            //
            // m_context.DrawShadows(ref shadowSettings);
            //
            // SetKeywords();
            // m_buffer.EndSample(BUFFER_NAME);
            // // RenderUtils.ExecuteBuffer(m_buffer,m_context);
            // ExecuteBuffer();


        }

        public Vector3 ReserveDirectionalShadows(Light light)
        {
            if (light.shadows != LightShadows.None && light.shadowStrength > 0f && RAPI.CullingResults.GetShadowCasterBounds(0, out Bounds b))
            {
                m_light = new ShadowedDirectionalLight {
                    slopeScaleBias = light.shadowBias,
                    nearPlaneOffset = light.shadowNearPlane
                };
                ready = true;
                return new Vector3(light.shadowStrength, m_settings.directional.cascades == ShadowSettings.Cascades._2X ? 2 : 4,
                    light.shadowNormalBias);
            }
            return Vector3.zero;
        }
        
        
        void SetCascadeData (int index, Vector4 cullingSphere, float tileSize) {
            float texelSize = 2f * cullingSphere.w / tileSize;
            float filterSize = texelSize * ((float)m_settings.directional.filter + 1f);
            cullingSphere.w -= filterSize;
            cullingSphere.w *= cullingSphere.w;
            cascadeCullingSpheres[index] = cullingSphere;
            cascadeData[index] = new Vector4(1f / cullingSphere.w, filterSize * 1.4142136f);
        }

        Vector2 SetTileViewport (int index, int split, float tileSize) {
            Vector2 offset = new Vector2(index % split, index / split);
            RAPI.Buffer.SetViewport(new Rect(offset.x * tileSize, offset.y * tileSize, tileSize, tileSize));
            return offset;
        }
        
        Matrix4x4 ConvertToAtlasMatrix (Matrix4x4 m, Vector2 offset, int split) {
            if (SystemInfo.usesReversedZBuffer) {
                m.m20 = -m.m20;
                m.m21 = -m.m21;
                m.m22 = -m.m22;
                m.m23 = -m.m23;
            }
            float scale = 1f / split;
            m.m00 = (0.5f * (m.m00 + m.m30) + offset.x * m.m30) * scale;
            m.m01 = (0.5f * (m.m01 + m.m31) + offset.x * m.m31) * scale;
            m.m02 = (0.5f * (m.m02 + m.m32) + offset.x * m.m32) * scale;
            m.m03 = (0.5f * (m.m03 + m.m33) + offset.x * m.m33) * scale;
            m.m10 = (0.5f * (m.m10 + m.m30) + offset.y * m.m30) * scale;
            m.m11 = (0.5f * (m.m11 + m.m31) + offset.y * m.m31) * scale;
            m.m12 = (0.5f * (m.m12 + m.m32) + offset.y * m.m32) * scale;
            m.m13 = (0.5f * (m.m13 + m.m33) + offset.y * m.m33) * scale;
            m.m20 = 0.5f * (m.m20 + m.m30);
            m.m21 = 0.5f * (m.m21 + m.m31);
            m.m22 = 0.5f * (m.m22 + m.m32);
            m.m23 = 0.5f * (m.m23 + m.m33);
            return m;
        }


        
    }
    
    
}
