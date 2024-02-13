using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
	public class CustomRenderPipeline : RenderPipeline
	{
		CameraRenderer renderer = new CameraRenderer();

		public CustomRenderPipeline () {
			GraphicsSettings.useScriptableRenderPipelineBatching = true;
			GraphicsSettings.lightsUseLinearIntensity = true;
		}

		protected override void Render(
			ScriptableRenderContext context, Camera[] cameras
		)
		{
			for (int i = 0; i < cameras.Length; i++) {
				renderer.Render(context, cameras[i]);
			}
		}
	}
}