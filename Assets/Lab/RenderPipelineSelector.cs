using UnityEngine;
using UnityEngine.Rendering;

public class RenderPipelineSelector : MonoBehaviour
{
    public RenderPipelineAsset highQualityPipeline;
    public RenderPipelineAsset mediumQualityPipeline;
    public RenderPipelineAsset lowQualityPipeline;

    void Start()
    {
        SelectRenderPipeline();
    }

    void SelectRenderPipeline()
    {
        int systemMemory = SystemInfo.systemMemorySize;
        int graphicsMemory = SystemInfo.graphicsMemorySize;
        int processorCount = SystemInfo.processorCount;
        int graphicsShaderLevel = SystemInfo.graphicsShaderLevel;

        Debug.Log($"System Memory: {systemMemory} MB");
        Debug.Log($"Graphics Memory: {graphicsMemory} MB");
        Debug.Log($"Processor Count: {processorCount}");
        Debug.Log($"Graphics Shader Level: {graphicsShaderLevel}");

        const int highQualityMemoryThreshold = 16000;
        const int mediumQualityMemoryThreshold = 8000;
        const int highQualityGraphicsThreshold = 8000; 
        const int mediumQualityGraphicsThreshold = 4000;
        const int highQualityProcessorThreshold = 8;
        const int mediumQualityProcessorThreshold = 4;
        const int highQualityShaderLevelThreshold = 50;
        const int mediumQualityShaderLevelThreshold = 35;

        if (systemMemory >= highQualityMemoryThreshold &&
            graphicsMemory >= highQualityGraphicsThreshold &&
            processorCount >= highQualityProcessorThreshold &&
            graphicsShaderLevel >= highQualityShaderLevelThreshold)
        {
            Debug.Log("Selecting High Quality Render Pipeline");
            GraphicsSettings.renderPipelineAsset = highQualityPipeline;
        }
        else if (systemMemory >= mediumQualityMemoryThreshold &&
                 graphicsMemory >= mediumQualityGraphicsThreshold &&
                 processorCount >= mediumQualityProcessorThreshold &&
                 graphicsShaderLevel >= mediumQualityShaderLevelThreshold)
        {
            Debug.Log("Selecting Medium Quality Render Pipeline");
            GraphicsSettings.renderPipelineAsset = mediumQualityPipeline;
        }
        else
        {
            Debug.Log("Selecting Low Quality Render Pipeline");
            GraphicsSettings.renderPipelineAsset = lowQualityPipeline;
        }
    }
}
