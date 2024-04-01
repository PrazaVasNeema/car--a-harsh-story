using UnityEngine;
using UnityEngine.Rendering;

namespace DopeRP.CPU
{
    public static class SProps
    {
        
        public static class CameraRenderer
        {
            
            public static ShaderTagId UnlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
            public static ShaderTagId LitShaderTagId = new ShaderTagId("Lit");
            
            public static string LitDeferredPassName = "LitDeferred";

            
            public static int ScreenSize = Shader.PropertyToID("_ScreenSize");
            
        }
        
        public static class LightingMain
        {
            public static string DirLightOnKeyword = "_DIR_LIGHT_ON";
            public static string OtherLightnCountKeyword_base = "_OTHER_LIGHT_COUNT_";
            
            public static int DirLightDirectionId = Shader.PropertyToID("_DirLightDirection");
            public static int DirLightColorId = Shader.PropertyToID("_DirLightColor");
            public static int DirLightShadowDataId = Shader.PropertyToID("_DirectionalLightShadowData");
            
            public static int OtherLightCountId = Shader.PropertyToID("_OtherLightCount");
            public static int OtherLightPositionsId = Shader.PropertyToID("_OtherLightPositions");
            public static int OtherLightColorsId = Shader.PropertyToID("_OtherLightColors");
            
            public static int OtherLightDirectionsId = Shader.PropertyToID("_OtherLightDirections");
            public static int OtherLightSpotAnglesId = Shader.PropertyToID("_OtherLightSpotAngles");
            
        }
        
        public static class Shadows
        {
            
            public static int DirShadowAtlasId = Shader.PropertyToID("_DirectionalShadowAtlas");
            public static int DirShadowMatricesId = Shader.PropertyToID("_DirectionalShadowMatrices");
            public static int CascadesCountId = Shader.PropertyToID("_CascadeCount");
            public static int CascadesCullingSpheresId = Shader.PropertyToID("_CascadeCullingSpheres");
            public static int CascadeDataId = Shader.PropertyToID("_CascadeData");
            public static int ShadowAtlasSizeId = Shader.PropertyToID("_ShadowAtlasSize");
            public static int ShadowDistanceFadeId = Shader.PropertyToID("_ShadowDistanceFade");
            
        }
        
        public static class SSAO
        {
            
            // public static ShaderTagId SSAORawPassId = new ShaderTagId("SSAORawPass");
            // public static ShaderTagId SSAOBlurPassId = new ShaderTagId("SSAOBlurPass");
            
            public static string SSAORawPassName = "SSAORawPass";
            public static string SSAOBlurPassName = "SSAOBlurPass";
            
            public static int NoiseTexture = Shader.PropertyToID("_NoiseTexture");
            public static int RandomSize = Shader.PropertyToID("_RandomSize");
            public static int SampleRadius = Shader.PropertyToID("_SampleRadius");
            public static int Bias = Shader.PropertyToID("_Bias");
            public static int Magnitude = Shader.PropertyToID("_Magnitude");
            public static int Contrast = Shader.PropertyToID("_Contrast");
            
            public static int SSAORawAtlas = Shader.PropertyToID("_SSAORawAtlas");
            public static int SSAOBlurAtlas = Shader.PropertyToID("_SSAOBlurAtlas");
            
            public static int ScreenSize = Shader.PropertyToID("_ScreenSize");
            public static int LensProjection = Shader.PropertyToID("_LensProjection");
            public static int NoiseScale = Shader.PropertyToID("_NoiseScale");

        }

        public static class GBuffer
        {
            
            public static ShaderTagId GBufferPassId = new ShaderTagId("GBufferPass");
            
            public static string GBufferPassName = "GBufferPass";
            
            public static int GAux_TangentWorldSpaceAtlas = Shader.PropertyToID("_GAux_TangentWorldSpace");
            
            public static int G_AlbedoAtlas = Shader.PropertyToID("_G_AlbedoAtlas");
            public static int G_NormalWorldSpaceAtlas = Shader.PropertyToID("_G_NormalWorldSpaceAtlas");
            public static int G_SpecularAtlas = Shader.PropertyToID("_G_SpecularAtlas");
            public static int G_BRDFAtlas = Shader.PropertyToID("_G_BRDFAtlas");

            public static int CameraNearPlane = Shader.PropertyToID("_CameraNearPlane");
            public static int CameraFarPlane = Shader.PropertyToID("_CameraFarPlane");

        }
        
        public static class Decals
        {
            
            public static ShaderTagId DecalsPassId = new ShaderTagId("DecalsPass");

            public static int DecalsDamageAlbedoAtlas = Shader.PropertyToID("_DecalsDamageAlbedoAtlas");
            public static int DecalsDamageNormalAtlas = Shader.PropertyToID("_DecalsDamageNormalAtlas");
            
            // Forward rendering
            public static int DecalsArtisticAlbedoAtlas = Shader.PropertyToID("_DecalsArtisticAlbedoAtlas");
            public static int DecalsArtisticNormalAtlas = Shader.PropertyToID("_DDecalsArtisticNormalAtlas");
            
            public static int ScreenSize = Shader.PropertyToID("_ScreenSize");
            
        }
        
    }
}
