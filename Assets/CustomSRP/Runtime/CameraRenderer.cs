using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
	public partial class CameraRenderer {
		
		private const string BUFFER_NAME = "RenderCamera";

		private static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
		private static ShaderTagId litShaderTagId = new ShaderTagId("CustomLit");

		private readonly Lighting m_lighting = new Lighting();
		private readonly RenderBuffers m_renderBuffers = new RenderBuffers();
		private readonly GBuffers m_gBuffers = new GBuffers();
		private readonly Decals m_decals = new Decals();

		private readonly SSAO m_ssao = new SSAO();


		public void Render(Camera camera, bool useDynamicBatching, bool useGPUInstancing,
			CustomRenderPipelineAsset customRenderPipelineAsset)
		{
			RAPI.CurCamera = camera;
			PrepareUIForSceneWindow();
			if (!RAPI.Cull(customRenderPipelineAsset.shadowSettings.maxDistance)) {
				return;
			}
			
			// m_ssao.Render();
			RAPI.Context.SetupCameraProperties(RAPI.CurCamera);

			m_gBuffers.Render();
			m_ssao.Render(customRenderPipelineAsset.SSAOSettings);
			m_decals.Render();
			//
			
			m_lighting.Setup(RAPI.Context, RAPI.CullingResults, customRenderPipelineAsset.shadowSettings);
			Setup();
			
			DrawVisibleGeometry(useDynamicBatching, useGPUInstancing);
			DrawUnsupportedShaders();
			DrawGizmos();
			RAPI.CleanupTempRT(Shadows.dirShadowAtlasId);
			// RAPI.CleanupTempRT(SSAO.SSAODepthNormalsAtlas);
			// RAPI.CleanupTempRT(SSAO.SSAOAtlas);
			
			RAPI.CleanupTempRT(SProps.GBuffer.PositionViewSpaceAtlas);
			RAPI.CleanupTempRT(SProps.GBuffer.NormalViewSpaceAtlas);
			RAPI.CleanupTempRT(SProps.GBuffer.TangentViewSpaceAtlas);
			RAPI.CleanupTempRT(SProps.SSAO.SSAORawAtlas);
			RAPI.CleanupTempRT(SProps.SSAO.SSAOBlurAtlas);
			RAPI.CleanupTempRT(Decals.DecalsAtlas);
			RAPI.CleanupTempRT(Decals.DecalsAtlasNormals);


			//lighting.Cleanup();

			Submit();
		}

		void Setup () {
			RAPI.Context.SetupCameraProperties(RAPI.CurCamera);
			var flags = RAPI.CurCamera.clearFlags;
			RAPI.Buffer.ClearRenderTarget(flags <= CameraClearFlags.Depth, flags == CameraClearFlags.Color,
				flags == CameraClearFlags.Color ? RAPI.CurCamera.backgroundColor.linear : Color.clear);
			RAPI.Buffer.BeginSample(BUFFER_NAME);
			RAPI.ExecuteBuffer();
		}

		void DrawVisibleGeometry (bool useDynamicBatching, bool useGPUInstancing)
		{
			var sortingSettings = new SortingSettings(RAPI.CurCamera)
			{
				criteria = SortingCriteria.CommonOpaque
			};
			var drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings)
			{
				enableDynamicBatching = useDynamicBatching,
				enableInstancing = useGPUInstancing,
				perObjectData =
				PerObjectData.ReflectionProbes |
				PerObjectData.Lightmaps | PerObjectData.ShadowMask |
				PerObjectData.LightProbe | PerObjectData.OcclusionProbe |
				PerObjectData.LightProbeProxyVolume |
				PerObjectData.OcclusionProbeProxyVolume
			};
			drawingSettings.SetShaderPassName(1, litShaderTagId);
			var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

			RAPI.Context.DrawRenderers(RAPI.CullingResults, ref drawingSettings, ref filteringSettings);

			RAPI.Context.DrawSkybox(RAPI.CurCamera);

			//Draw transparent geometry
			sortingSettings.criteria = SortingCriteria.CommonTransparent;
			drawingSettings.sortingSettings = sortingSettings;
			filteringSettings.renderQueueRange = RenderQueueRange.transparent;
			RAPI.Context.DrawRenderers(RAPI.CullingResults, ref drawingSettings, ref filteringSettings);
			
		}

		void Submit () {
			RAPI.Buffer.EndSample(BUFFER_NAME);
			RAPI.ExecuteBuffer();
			RAPI.Context.Submit();
		}
		
	}
	
}