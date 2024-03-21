using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
    public static class SProps
    {
        
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
            
            public static int PositionViewSpaceAtlas = Shader.PropertyToID("_PositionViewSpace");
            public static int NormalViewSpaceAtlas = Shader.PropertyToID("_NormalViewSpace");
            public static int TangentViewSpaceAtlas = Shader.PropertyToID("_TangentViewSpace");

            public static int CameraNearPlane = Shader.PropertyToID("_CameraNearPlane");
            public static int CameraFarPlane = Shader.PropertyToID("_CameraFarPlane");

        }
        
        public static class Decals
        {
            
            public static ShaderTagId DecalsPassId = new ShaderTagId("DecalsPass");

            public static int DecalsAlbedoAtlas = Shader.PropertyToID("_DecalsAlbedoAtlas");
            public static int DecalsNormalAtlas = Shader.PropertyToID("_DecalsNormalAtlas");
            
            public static int ScreenSize = Shader.PropertyToID("_ScreenSize");
            
        }
        
    }
}
