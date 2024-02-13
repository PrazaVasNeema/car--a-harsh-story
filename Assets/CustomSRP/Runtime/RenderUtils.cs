using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public static class RenderUtils
{
    public static CommandBuffer THE_buffer = new CommandBuffer()
    {
        name = "BUFFER_NAME_DEFAULT"
    };

    public static ScriptableRenderContext THE_context;
    public static void ExecuteBuffer (CommandBuffer buffer, ScriptableRenderContext context) {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
        THE_context = context;
    }

    public static void CleanupTempRT(CommandBuffer buffer, int atlasID)
    {
        buffer.ReleaseTemporaryRT(atlasID);
        ExecuteBuffer(buffer, THE_context);
    }
}
