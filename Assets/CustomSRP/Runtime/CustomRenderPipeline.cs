using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
	public class CustomRenderPipeline : RenderPipeline
	{
		CameraRenderer renderer = new CameraRenderer();
		private ShadowSettings m_shadowSettings;

		public CustomRenderPipeline (ShadowSettings shadowSettings) {
			GraphicsSettings.useScriptableRenderPipelineBatching = true;
			GraphicsSettings.lightsUseLinearIntensity = true;
			m_shadowSettings = shadowSettings;
		}

		protected override void Render(
			ScriptableRenderContext context, Camera[] cameras
		)
		{
			for (int i = 0; i < cameras.Length; i++) {
				renderer.Render(context, cameras[i], m_shadowSettings);
			}
		}
	}
}