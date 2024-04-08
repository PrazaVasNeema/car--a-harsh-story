using DopeRP.CPU;
using UnityEngine;
using UnityEngine.Rendering;
using static PostFXSettings;

public partial class PostFXStack {

    const string bufferName = "Post FX";

    
    
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

    public void Setup (ScriptableRenderContext context, Camera camera, PostFXSettings settings) {
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
       
        // RAPI.Draw(sourceId, SProps.PostFX.fxSourceAtlas, Pass.Copy, settings.Material);
        // RAPI.Buffer.SetGlobalTexture(SProps.PostFX.fxSourceAtlas, sourceId);

        // RAPI.ExecuteBuffer();
        
        foreach (var fxFeature in settings.currentFXFeaturesList)
        {
            if (fxFeature.FXFeatureIsOne)
            {
                fxFeature.fxFeature.SetupUniforms();
                fxFeature.fxFeature.Render(SProps.PostFX.fxSourceAtlas, SProps.PostFX.fxDestinationAtlas, settings);
                RAPI.Draw(SProps.PostFX.fxDestinationAtlas, SProps.PostFX.fxSourceAtlas, Pass.Copy, settings.Material);
            }
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
    
    
}