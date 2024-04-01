using UnityEngine;
using UnityEngine.Rendering;

namespace DopeRP.CPU
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
			
			DrawVisibleGeometry(useDynamicBatching, useGPUInstancing, customRenderPipelineAsset.LitDeferredMaterial);
			DrawUnsupportedShaders();
			DrawGizmos();
			
			RAPI.CleanupTempRT(SProps.Shadows.DirShadowAtlasId);
			RAPI.CleanupTempRT(SProps.GBuffer.GAux_TangentWorldSpaceAtlas);
			RAPI.CleanupTempRT(SProps.SSAO.SSAORawAtlas);
			RAPI.CleanupTempRT(SProps.SSAO.SSAOBlurAtlas);
			RAPI.CleanupTempRT(SProps.Decals.DecalsDamageAlbedoAtlas);
			RAPI.CleanupTempRT(SProps.Decals.DecalsDamageNormalAtlas);
			// RAPI.Context.DrawSkybox(RAPI.CurCamera);
			RAPI.CleanupTempRT(Shader.PropertyToID("Test"));
			
			RAPI.CleanupTempRT((SProps.GBuffer.G_AlbedoAtlas));
			RAPI.CleanupTempRT((SProps.GBuffer.G_NormalWorldSpaceAtlas));
			RAPI.CleanupTempRT((SProps.GBuffer.G_SpecularAtlas));
			RAPI.CleanupTempRT((SProps.GBuffer.G_BRDFAtlas));
			Submit();
		}

		void Setup () {
			RAPI.Context.SetupCameraProperties(RAPI.CurCamera);
			var flags = RAPI.CurCamera.clearFlags;
			RAPI.Buffer.ClearRenderTarget(flags <= CameraClearFlags.Depth, flags == CameraClearFlags.Color,
				flags == CameraClearFlags.Color ? RAPI.CurCamera.backgroundColor.linear : Color.clear);
			
			RAPI.ExecuteBuffer();
		}

		void DrawVisibleGeometry (bool useDynamicBatching, bool useGPUInstancing, Material litDeferredMaterial)
		{
			RAPI.Buffer.SetGlobalVector(SProps.CameraRenderer.ScreenSize, new Vector4(RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight,
				(float)1/RAPI.CurCamera.pixelWidth , (float)1/RAPI.CurCamera.pixelHeight));

			RAPI.Buffer.SetGlobalVector(Shader.PropertyToID("_nearFarPlanes"), new Vector4(RAPI.CurCamera.nearClipPlane, RAPI.CurCamera.farClipPlane, 0, 0 ));
			RAPI.Buffer.SetGlobalMatrix(Shader.PropertyToID("adfgdgf_WorldToCameraMatrix"),  RAPI.CurCamera.worldToCameraMatrix);
			RAPI.Buffer.SetGlobalMatrix(Shader.PropertyToID("adfgdgf_CameraToWorldMatrix"),  RAPI.CurCamera.cameraToWorldMatrix);
            
			Matrix4x4 invProjectionMatrix = RAPI.CurCamera.projectionMatrix.inverse;
			RAPI.Buffer.SetGlobalMatrix(Shader.PropertyToID("_INVERSE_P"), invProjectionMatrix);

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
			
			var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
			
			// var sortingSettings = new SortingSettings(RAPI.CurCamera)
			// {
			// 	criteria = SortingCriteria.CommonOpaque
			// };

			// var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
			//
			RAPI.Context.DrawRenderers(RAPI.CullingResults, ref drawingSettings, ref filteringSettings);
			
			RAPI.Buffer.Blit(null, BuiltinRenderTextureType.CurrentActive, litDeferredMaterial, litDeferredMaterial.FindPass(SProps.CameraRenderer.LitDeferredPassName));

			
			RAPI.Context.DrawSkybox(RAPI.CurCamera);
			RAPI.ExecuteBuffer();

			//Draw transparent geometry
			drawingSettings.SetShaderPassName(1, SProps.CameraRenderer.LitShaderTagId);

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