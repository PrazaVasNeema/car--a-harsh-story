using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
	[CreateAssetMenu(menuName = "Rendering/Custom Render Pipeline")]
	public class CustomRenderPipelineAsset : RenderPipelineAsset
	{
		
		public static CustomRenderPipelineAsset instance;
		[SerializeField]
		bool useDynamicBatching = true, useGPUInstancing = true, useSRPBatcher = true;
		[SerializeField] private ShadowSettings m_shadowsSettings = default;
		
		[SerializeField] private SSAOSettings ssaoSettings;
		
		public SSAOSettings SSAOSettings => ssaoSettings;
		
		protected override RenderPipeline CreatePipeline () {
			instance = this;
			return new CustomRenderPipeline(useDynamicBatching, useGPUInstancing, useSRPBatcher, m_shadowsSettings);
			
		}
	}
}