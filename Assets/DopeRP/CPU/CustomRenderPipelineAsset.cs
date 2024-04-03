using UnityEngine;
using UnityEngine.Rendering;

namespace DopeRP.CPU
{
	[CreateAssetMenu(menuName = "Rendering/Custom Render Pipeline")]
	public class CustomRenderPipelineAsset : RenderPipelineAsset
	{
		[Header("(Just so unity don't create a new material each render call)")]
		public Material EmptyMaterial;
		public static CustomRenderPipelineAsset instance;
		[SerializeField]
		bool useDynamicBatching = true, useGPUInstancing = true, useSRPBatcher = true;
		[Header("(Just so unity don't create a new material each render call)")]
		public Material LitDeferredMaterial;
		[SerializeField] 
		private bool m_shadows;
		public bool shadows => m_shadows;
		[SerializeField] private ShadowSettings m_shadowsSettings = default;
		public ShadowSettings shadowSettings => m_shadowsSettings;
		[SerializeField]
		PostFXSettings postFXSettings = default;

		[Header("-------------------------")]
		[SerializeField] 
		private bool m_SSAO;
		public bool SSAO => m_SSAO;
		[SerializeField] 
		private bool m_decalsOn;
		public bool decalsOn => m_decalsOn;
		[SerializeField] 
		private SSAOSettings ssaoSettings;
		public SSAOSettings SSAOSettings => ssaoSettings;
		
		public enum ColorLUTResolution { _16 = 16, _32 = 32, _64 = 64 }

		[SerializeField]
		ColorLUTResolution colorLUTResolution = ColorLUTResolution._32;
		
		protected override RenderPipeline CreatePipeline () {
			instance = this;
			return new CustomRenderPipeline(useDynamicBatching, useGPUInstancing, useSRPBatcher, this,
				postFXSettings, (int)colorLUTResolution);
			
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