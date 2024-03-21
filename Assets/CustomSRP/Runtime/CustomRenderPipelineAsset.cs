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
		public ShadowSettings shadowSettings => m_shadowsSettings;

		[Header("-------------------------")]
		[SerializeField] 
		public bool SSAO;
		[SerializeField] 
		private SSAOSettings ssaoSettings;
		public SSAOSettings SSAOSettings => ssaoSettings;
		
		protected override RenderPipeline CreatePipeline () {
			instance = this;
			return new CustomRenderPipeline(useDynamicBatching, useGPUInstancing, useSRPBatcher, this);
			
		}
		
		
		public void onPropertyChangeSSAOSettings()
		{
			Material ssaoMaterial = ssaoSettings.SSAOMaterial;
			ssaoMaterial.SetTexture(SProps.SSAO.NoiseTexture, ssaoSettings.noiseTexture);
			ssaoMaterial.SetFloat(SProps.SSAO.RandomSize, ssaoSettings.randomSize);
			ssaoMaterial.SetFloat(SProps.SSAO.SampleRadius, ssaoSettings.sampleRadius);
			ssaoMaterial.SetFloat(SProps.SSAO.Bias, ssaoSettings.bias);
			ssaoMaterial.SetFloat(SProps.SSAO.Magnitude, ssaoSettings.magnitude);
			ssaoMaterial.SetFloat(SProps.SSAO.Contrast, ssaoSettings.contrast);
		}
	}
}