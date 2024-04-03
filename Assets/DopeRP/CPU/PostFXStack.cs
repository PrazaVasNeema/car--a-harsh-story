using UnityEngine;
using UnityEngine.Rendering;

public partial class PostFXStack {

    const string bufferName = "Post FX";
    
    enum Pass {
        Copy,
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
        DoToneMapping(sourceId);
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
    

    
    void DoToneMapping(int sourceId) {
        PostFXSettings.ToneMappingSettings.Mode mode = settings.ToneMapping.mode;
        Pass pass = mode < 0 ? Pass.Copy : Pass.ToneMappingACES + (int)mode;
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