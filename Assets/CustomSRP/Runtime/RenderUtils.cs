using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public static class RenderUtils
{
    public static void ExecuteBuffer (CommandBuffer buffer, ScriptableRenderContext context) {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
}
