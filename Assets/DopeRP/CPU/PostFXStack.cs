using DopeRP.CPU;
using UnityEngine;
using UnityEngine.Rendering;
using static PostFXSettings;

public partial class PostFXStack {

    const string bufferName = "Post FX";

    private int colorAdjustmentsId = Shader.PropertyToID("_ColorAdjustments");
    int colorFilterId = Shader.PropertyToID("_ColorFilter");
    private int whiteBalanceId = Shader.PropertyToID("_WhiteBalance");
    private int splitToningShadowsId = Shader.PropertyToID("_SplitToningShadows");
    private int splitToningHighlightsId = Shader.PropertyToID("_SplitToningHighlights");
    private int channelMixerRedId = Shader.PropertyToID("_ChannelMixerRed");
    private int channelMixerGreenId = Shader.PropertyToID("_ChannelMixerGreen");
    private int channelMixerBlueId = Shader.PropertyToID("_ChannelMixerBlue");
    private int smhShadowsId = Shader.PropertyToID("_SMHShadows");
     private   int smhMidtonesId = Shader.PropertyToID("_SMHMidtones");
     private   int smhHighlightsId = Shader.PropertyToID("_SMHHighlights");
     private   int        smhRangeId = Shader.PropertyToID("_SMHRange");
     
     private int colorGradingLUTId = Shader.PropertyToID("_ColorGradingLUT");
     private int colorGradingLUTParametersId = Shader.PropertyToID("_ColorGradingLUTParameters");
     private int colorGradingLUTInLogId = Shader.PropertyToID("_ColorGradingLUTInLogC");
     
     int colorLUTResolution;
    
    public enum Pass {
        Copy,
        ColorGradingNone,
        ColorGradingACES,
        ColorGradingNeutral,
        ColorGradingReinhard,
        Final,
        FXAA,
        Vignette
    }

    private int fxSourceId = Shader.PropertyToID("_PostFXSource");
    private int fxSource2Id = Shader.PropertyToID("_PostFXSource2");
    
    public bool IsActive => settings != null;

    CommandBuffer buffer = new CommandBuffer {
        name = bufferName
    };

    ScriptableRenderContext context;
	
    Camera camera;

    PostFXSettings settings;

    public void Setup (ScriptableRenderContext context, Camera camera, PostFXSettings settings, int colorLUTResolution) {
        this.colorLUTResolution = colorLUTResolution;
        this.context = context;
        this.camera = camera;
        this.settings = settings;
        this.settings = camera.cameraType <= CameraType.SceneView ? settings : null;
        ApplySceneViewState();
        RAPI.Buffer.GetTemporaryRT(SProps.PostFX.fxSourceAtlas, RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight, 32, FilterMode.Bilinear, RenderTextureFormat.Default);
        RAPI.Buffer.GetTemporaryRT(SProps.PostFX.fxDestinationAtlas, RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight, 32, FilterMode.Bilinear, RenderTextureFormat.Default);
        // RAPI.Draw();
        
    }
    
    public void Render (int sourceId) {
        // Draw(sourceId, BuiltinRenderTextureType.CameraTarget, Pass.Copy);
       
        RAPI.Draw(sourceId, SProps.PostFX.fxSourceAtlas, Pass.Copy, settings.Material);
        // RAPI.Buffer.SetGlobalTexture(SProps.PostFX.fxSourceAtlas, sourceId);

        // RAPI.ExecuteBuffer();
        
        foreach (var fxFeature in settings.currentFXFeaturesList)
        {
            fxFeature.SetupUniforms();
            fxFeature.Render(SProps.PostFX.fxSourceAtlas, SProps.PostFX.fxDestinationAtlas, settings);
            RAPI.Draw(SProps.PostFX.fxDestinationAtlas, SProps.PostFX.fxSourceAtlas, Pass.Copy, settings.Material);
        
        }
        // DoColorGradingAndToneMapping(sourceId);
        // context.ExecuteCommandBuffer(buffer);
        // buffer.Clear();
        RAPI.Draw(SProps.PostFX.fxSourceAtlas, BuiltinRenderTextureType.CameraTarget, Pass.Copy,
            settings.Material);
        
        // RAPI.Buffer.ReleaseTemporaryRT(SProps.PostFX.fxSourceAtlas);
        // RAPI.Buffer.ReleaseTemporaryRT(SProps.PostFX.fxDestinationAtlas);
        RAPI.ExecuteBuffer();
        
    }
    
    void ConfigureColorAdjustments () {
        ColorAdjustmentsSettings colorAdjustments = settings.ColorAdjustments;
        
        buffer.SetGlobalVector(colorAdjustmentsId, new Vector4(Mathf.Pow(2f, colorAdjustments.postExposure), colorAdjustments.contrast * 0.01f + 1f, colorAdjustments.hueShift * (1f / 360f), colorAdjustments.saturation * 0.01f + 1f));
        buffer.SetGlobalColor(colorFilterId, colorAdjustments.colorFilter.linear);
    }
    
    void ConfigureWhiteBalance () {
        WhiteBalanceSettings whiteBalance = settings.WhiteBalance;
        buffer.SetGlobalVector(whiteBalanceId, ColorUtils.ColorBalanceToLMSCoeffs(whiteBalance.temperature, whiteBalance.tint));
    }
    
    void ConfigureSplitToning () {
        SplitToningSettings splitToning = settings.SplitToning;
        Color splitColor = splitToning.shadows;
        splitColor.a = splitToning.balance * 0.01f;
        buffer.SetGlobalColor(splitToningShadowsId, splitColor);
        buffer.SetGlobalColor(splitToningHighlightsId, splitToning.highlights);
    }
    
    void ConfigureChannelMixer () {
        ChannelMixerSettings channelMixer = settings.ChannelMixer;
        buffer.SetGlobalVector(channelMixerRedId, channelMixer.red);
        buffer.SetGlobalVector(channelMixerGreenId, channelMixer.green);
        buffer.SetGlobalVector(channelMixerBlueId, channelMixer.blue);
    }
    
    void ConfigureShadowsMidtonesHighlights () {
        ShadowsMidtonesHighlightsSettings smh = settings.ShadowsMidtonesHighlights;
        buffer.SetGlobalColor(smhShadowsId, smh.shadows.linear);
        buffer.SetGlobalColor(smhMidtonesId, smh.midtones.linear);
        buffer.SetGlobalColor(smhHighlightsId, smh.highlights.linear);
        buffer.SetGlobalVector(smhRangeId, new Vector4(smh.shadowsStart, smh.shadowsEnd, smh.highlightsStart, smh.highLightsEnd));
    }
    
    void DoColorGradingAndToneMapping(int sourceId, bool useHDR = false) {
        ConfigureColorAdjustments();
        ConfigureWhiteBalance();
        ConfigureSplitToning();
        ConfigureChannelMixer();
        ConfigureShadowsMidtonesHighlights();

        int lutHeight = colorLUTResolution;
        int lutWidth = lutHeight * lutHeight;
        buffer.GetTemporaryRT(colorGradingLUTId, lutWidth, lutHeight, 0, FilterMode.Bilinear, RenderTextureFormat.DefaultHDR);
        buffer.SetGlobalVector(colorGradingLUTParametersId, new Vector4(lutHeight, 0.5f / lutWidth, 0.5f / lutHeight, lutHeight / (lutHeight - 1f)));

        ToneMappingSettings.Mode mode = settings.ToneMapping.mode;
        Pass pass = Pass.ColorGradingNone + (int)mode;
        buffer.SetGlobalFloat(colorGradingLUTInLogId, useHDR && pass != Pass.ColorGradingNone ? 1f : 0f);
        Draw(sourceId, colorGradingLUTId, pass);

        buffer.SetGlobalVector(colorGradingLUTParametersId, new Vector4(1f / lutWidth, 1f / lutHeight, lutHeight - 1f));
        
        RenderTextureFormat format = useHDR ?
            RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
        buffer.GetTemporaryRT(Shader.PropertyToID("_ColorGrading"), RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight, 0, FilterMode.Bilinear, format);
        Draw(sourceId, Shader.PropertyToID("_ColorGrading"), Pass.Final);
        
        buffer.GetTemporaryRT(Shader.PropertyToID("_FXAA"), RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight, 0, FilterMode.Bilinear, format);
        Draw(Shader.PropertyToID("_ColorGrading"), Shader.PropertyToID("_FXAA"), Pass.FXAA);
        
        buffer.GetTemporaryRT(Shader.PropertyToID("_Vingette"), RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight, 0, FilterMode.Bilinear, format);
        Draw(Shader.PropertyToID("_FXAA"), Shader.PropertyToID("_Vingette"), Pass.Vignette);
        
        //
        // if (settings.m_FXAA_ON)
            // Draw(sourceId, BuiltinRenderTextureType.CameraTarget, Pass.FXAA);
        //
        // buffer.SetGlobalFloat(Shader.PropertyToID("power"), 5);
        //
        // if (settings.m_vignette_on)
        //     Draw(sourceId, sourceId, Pass.Vignette);
        // buffer.ReleaseTemporaryRT(colorGradingLUTId);
        //
        var vignetteSettings = new Vector2(settings.m_vignette_offset, settings.m_vignette_power);
        RAPI.Buffer.SetGlobalVector(SProps.PostFX.VignetteSettings, vignetteSettings);
        Draw(Shader.PropertyToID("_Vingette"), BuiltinRenderTextureType.CameraTarget, Pass.Copy);
        
        RAPI.Buffer.ReleaseTemporaryRT(Shader.PropertyToID("_ColorGrading"));
        RAPI.Buffer.ReleaseTemporaryRT(Shader.PropertyToID("_FXAA"));
        RAPI.Buffer.ReleaseTemporaryRT(Shader.PropertyToID("_Vingette"));

    }
    
    void Draw (RenderTargetIdentifier from, RenderTargetIdentifier to, Pass pass) {
        buffer.SetGlobalTexture(fxSourceId, from);
        buffer.SetRenderTarget(to, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
        buffer.DrawProcedural(Matrix4x4.identity, settings.Material, (int)pass, MeshTopology.Triangles, 3);
    }
}