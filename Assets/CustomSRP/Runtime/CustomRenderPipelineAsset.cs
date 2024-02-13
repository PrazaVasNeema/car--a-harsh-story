using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
	[CreateAssetMenu(menuName = "Rendering/Custom Render Pipeline")]
	public class CustomRenderPipelineAsset : RenderPipelineAsset
	{
		[SerializeField] private ShadowSettings m_shadowsSettings = default;
		
		protected override RenderPipeline CreatePipeline () {
			return new CustomRenderPipeline(m_shadowsSettings);
		}
	}
}