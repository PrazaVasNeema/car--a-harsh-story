using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
	public class CustomRenderPipeline : RenderPipeline
	{
		CameraRenderer renderer = new CameraRenderer();
		
		bool useDynamicBatching, useGPUInstancing;

		private ShadowSettings m_shadowSettings;

		public CustomRenderPipeline (bool useDynamicBatching, bool useGPUInstancing, bool useSRPBatcher,
			ShadowSettings shadowSettings) {
			// GraphicsSettings.useScriptableRenderPipelineBatching = true;
			// GraphicsSettings.lightsUseLinearIntensity = true;
			m_shadowSettings = shadowSettings;
			this.useDynamicBatching = useDynamicBatching;
			this.useGPUInstancing = useGPUInstancing;
			GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
			GraphicsSettings.lightsUseLinearIntensity = true;
		}

		protected override void Render(ScriptableRenderContext context, Camera[] cameras)
		{
			RAPI.Context = context;
			for (int i = 0; i < cameras.Length; i++) {
				renderer.Render(cameras[i], useDynamicBatching, useGPUInstancing, m_shadowSettings);
			}
		}
	}
}