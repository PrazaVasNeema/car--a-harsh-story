using UnityEngine;
using UnityEngine.Rendering;

public class PostFXStack {

    const string bufferName = "Post FX";
    
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
    }
    
    public void Render (int sourceId) {
        buffer.Blit(sourceId, BuiltinRenderTextureType.CameraTarget);
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
}