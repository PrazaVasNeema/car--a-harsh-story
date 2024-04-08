using UnityEngine;
using UnityEngine.Rendering;

namespace DopeRP.CPU
{
	public class DopeRP : RenderPipeline
	{
		global::DopeRP.CPU.CameraRenderer renderer = new global::DopeRP.CPU.CameraRenderer();
		
		bool useGPUInstancing;

		private DopeRPAsset m_assetSettings;

		public DopeRP (bool useGPUInstancing,
			DopeRPAsset assetSettings) {
			// GraphicsSettings.useScriptableRenderPipelineBatching = true;
			// GraphicsSettings.lightsUseLinearIntensity = true;
			m_assetSettings = assetSettings;
			// this.useDynamicBatching = useDynamicBatching;
			this.useGPUInstancing = useGPUInstancing;
			// GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
			GraphicsSettings.lightsUseLinearIntensity = true;
		}

		protected override void Render(ScriptableRenderContext context, Camera[] cameras)
		{
			RAPI.Context = context;
			for (int i = 0; i < cameras.Length; i++) {
				renderer.Render(cameras[i], useGPUInstancing, m_assetSettings);
			}
		}
	}
}