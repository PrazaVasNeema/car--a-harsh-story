using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
	[CreateAssetMenu(menuName = "Rendering/Custom Render Pipeline")]
	public class CustomRenderPipelineAsset : RenderPipelineAsset
	{
		protected override RenderPipeline CreatePipeline () {
			return new CustomRenderPipeline();
		}
	}
}