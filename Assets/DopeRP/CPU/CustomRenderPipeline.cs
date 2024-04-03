using UnityEngine;
using UnityEngine.Rendering;

namespace DopeRP.CPU
{
	public class CustomRenderPipeline : RenderPipeline
	{
		DopeRP.CPU.CameraRenderer renderer = new DopeRP.CPU.CameraRenderer();
		
		bool useDynamicBatching, useGPUInstancing;

		private CustomRenderPipelineAsset m_customRenderPipelineAsset;
		private PostFXSettings postFXSettings;

		public CustomRenderPipeline (bool useDynamicBatching, bool useGPUInstancing, bool useSRPBatcher,
			CustomRenderPipelineAsset customRenderPipelineAsset, PostFXSettings postFXSettings) {
			// GraphicsSettings.useScriptableRenderPipelineBatching = true;
			// GraphicsSettings.lightsUseLinearIntensity = true;
			m_customRenderPipelineAsset = customRenderPipelineAsset;
			this.useDynamicBatching = useDynamicBatching;
			this.useGPUInstancing = useGPUInstancing;
			GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
			GraphicsSettings.lightsUseLinearIntensity = true;
			
			this.postFXSettings = postFXSettings;
		}

		protected override void Render(ScriptableRenderContext context, Camera[] cameras)
		{
			RAPI.Context = context;
			for (int i = 0; i < cameras.Length; i++) {
				renderer.Render(cameras[i], useDynamicBatching, useGPUInstancing, m_customRenderPipelineAsset, postFXSettings);
			}
		}
	}
}