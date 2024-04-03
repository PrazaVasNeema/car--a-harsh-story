using UnityEngine;
using UnityEngine.Rendering;
using static PostFXSettings;

public partial class PostFXStack {

    const string bufferName = "Post FX";

    private int colorAdjustmentsId = Shader.PropertyToID("_ColorAdjustments");
    int colorFilterId = Shader.PropertyToID("_ColorFilter");
    
    enum Pass {
        Copy,
        ColorGradingNone,
        ToneMappingACES,
        ToneMappingNeutral,
        ToneMappingReinhard
    }
    
    int 		fxSourceId = Shader.PropertyToID("_PostFXSource"),
        fxSource2Id = Shader.PropertyToID("_PostFXSource2");
    
    public bool IsActive => settings != null;

    CommandBuffer buffer = new CommandBuffer {
        name = bufferName
    };

    ScriptableRenderContext context;
	
    Camera camera;

    PostFXSettings settings;

    public void Setup (
        ScriptableRenderContext context, Camera camera, PostFXSettings settings
    ) {
        this.context = context;
        this.camera = camera;
        this.settings = settings;
        this.settings =
            camera.cameraType <= CameraType.SceneView ? settings : null;
        ApplySceneViewState();
    }
    
    public void Render (int sourceId) {
        // Draw(sourceId, BuiltinRenderTextureType.CameraTarget, Pass.Copy);
        DoColorGradingAndToneMapping(sourceId);
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
    
    void ConfigureColorAdjustments () {
        ColorAdjustmentsSettings colorAdjustments = settings.ColorAdjustments;
        
        buffer.SetGlobalVector(colorAdjustmentsId, new Vector4(
            Mathf.Pow(2f, colorAdjustments.postExposure),
            colorAdjustments.contrast * 0.01f + 1f,
            colorAdjustments.hueShift * (1f / 360f),
            colorAdjustments.saturation * 0.01f + 1f
        ));
        buffer.SetGlobalColor(colorFilterId, colorAdjustments.colorFilter.linear);
    }
    
    void DoColorGradingAndToneMapping(int sourceId) {
        ConfigureColorAdjustments();
        
        ToneMappingSettings.Mode mode = settings.ToneMapping.mode;
        Pass pass = Pass.ColorGradingNone + (int)mode;
        Draw(sourceId, BuiltinRenderTextureType.CameraTarget, pass);
    }
    
    void Draw (
        RenderTargetIdentifier from, RenderTargetIdentifier to, Pass pass
    ) {
        buffer.SetGlobalTexture(fxSourceId, from);
        buffer.SetRenderTarget(
            to, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store
        );
        buffer.DrawProcedural(
            Matrix4x4.identity, settings.Material, (int)pass,
            MeshTopology.Triangles, 3
        );
    }
}