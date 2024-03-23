using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
	public partial class CameraRenderer {
		
		private const string BUFFER_NAME = "RenderCamera";

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
			
			RAPI.Context.SetupCameraProperties(RAPI.CurCamera);

			if ( customRenderPipelineAsset.SSAO || customRenderPipelineAsset.decalsOn)
				m_gBuffers.Render();
			if (customRenderPipelineAsset.SSAO)
			{
				m_ssao.Render(customRenderPipelineAsset.SSAOSettings);
				RAPI.SetKeyword("SSAO_ON", true);
			}
			else
			{
				RAPI.SetKeyword("SSAO_ON", false);
			}

			if (customRenderPipelineAsset.decalsOn)
			{
				RAPI.SetKeyword("DECALS_ON", true);
				m_decals.Render();
			}
			else
			{
				RAPI.SetKeyword("DECALS_ON", false);
			}


			m_lighting.Setup(customRenderPipelineAsset.shadowSettings);
			Setup();
			
			DrawVisibleGeometry(useDynamicBatching, useGPUInstancing);
			DrawUnsupportedShaders();
			DrawGizmos();
			
			RAPI.CleanupTempRT(SProps.Shadows.DirShadowAtlasId);
			RAPI.CleanupTempRT(SProps.GBuffer.PositionViewSpaceAtlas);
			RAPI.CleanupTempRT(SProps.GBuffer.NormalViewSpaceAtlas);
			RAPI.CleanupTempRT(SProps.GBuffer.TangentViewSpaceAtlas);
			RAPI.CleanupTempRT(SProps.SSAO.SSAORawAtlas);
			RAPI.CleanupTempRT(SProps.SSAO.SSAOBlurAtlas);
			RAPI.CleanupTempRT(SProps.Decals.DecalsAlbedoAtlas);
			RAPI.CleanupTempRT(SProps.Decals.DecalsNormalAtlas);
			RAPI.Context.DrawSkybox(RAPI.CurCamera);

			Submit();
		}

		void Setup () {
			RAPI.Context.SetupCameraProperties(RAPI.CurCamera);
			var flags = RAPI.CurCamera.clearFlags;
			RAPI.Buffer.ClearRenderTarget(flags <= CameraClearFlags.Depth, flags == CameraClearFlags.Color,
				flags == CameraClearFlags.Color ? RAPI.CurCamera.backgroundColor.linear : Color.clear);
			
			RAPI.ExecuteBuffer();
		}

		void DrawVisibleGeometry (bool useDynamicBatching, bool useGPUInstancing)
		{
			RAPI.Buffer.SetGlobalVector(SProps.CameraRenderer.ScreenSize, new Vector4(RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight,
				(float)1/RAPI.CurCamera.pixelWidth , (float)1/RAPI.CurCamera.pixelHeight));

			
			var sortingSettings = new SortingSettings(RAPI.CurCamera)
			{
				criteria = SortingCriteria.CommonOpaque
			};
			var drawingSettings = new DrawingSettings(SProps.CameraRenderer.UnlitShaderTagId, sortingSettings)
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
			drawingSettings.SetShaderPassName(1, SProps.CameraRenderer.LitShaderTagId);
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
			
			RAPI.ExecuteBuffer();
			RAPI.Context.Submit();
		}
		
	}
	
}