using UnityEngine;
using UnityEngine.Rendering;

namespace DopeRP.CPU
{
	public partial class CameraRenderer {
		
		private const string BUFFER_NAME = "RenderCamera";

		private readonly StencilPrepass m_stencilPrepass = new StencilPrepass();
		
		private readonly Lighting m_lighting = new Lighting();
		private readonly RenderBuffers m_renderBuffers = new RenderBuffers();
		private readonly GBuffers m_gBuffers = new GBuffers();
		private readonly Decals m_decals = new Decals();

		private readonly SSAO m_ssao = new SSAO();
		
		PostFXStack postFXStack = new PostFXStack();
		
		static int frameBufferId = Shader.PropertyToID("_CameraFrameBuffer");


		public void Render(Camera camera, bool useDynamicBatching, bool useGPUInstancing,
			CustomRenderPipelineAsset customRenderPipelineAsset, PostFXSettings postFXSettings,
			int colorLUTResolution)
		{
			RAPI.CurCamera = camera;
			PrepareUIForSceneWindow();
			if (!RAPI.Cull(customRenderPipelineAsset.shadowSettings.maxDistance)) {
				return;
			}
			
			RAPI.Context.SetupCameraProperties(RAPI.CurCamera);
			RAPI.SetupCommonUniforms();
			
			RAPI.CurCamera.depthTextureMode = DepthTextureMode.None;

			// if ( customRenderPipelineAsset.SSAO || customRenderPipelineAsset.decalsOn)
			m_stencilPrepass.Render();
				m_gBuffers.Render();
				RAPI.DrawEmpty(customRenderPipelineAsset.EmptyMaterial);
				if (customRenderPipelineAsset.decalsOn)
				{
					RAPI.SetKeyword("DECALS_ON", true);
					m_decals.Render();
				}
				else
				{
					RAPI.SetKeyword("DECALS_ON", false);
				}
			if (customRenderPipelineAsset.SSAO)
			{
				m_ssao.Render(customRenderPipelineAsset.SSAOSettings);
				RAPI.SetKeyword("SSAO_ON", true);
			}
			else
			{
				RAPI.SetKeyword("SSAO_ON", false);
			}




			m_lighting.Setup(customRenderPipelineAsset.shadowSettings);
			postFXStack.Setup(RAPI.Context, camera, postFXSettings, colorLUTResolution);
			Setup();

			if (RAPI.CurCamera.cameraType == CameraType.Reflection)
			{
				DrawVisibleGeometryRefProbes(useDynamicBatching, useGPUInstancing);
			}
			else
			{
				DrawVisibleGeometry(useDynamicBatching, useGPUInstancing,
					customRenderPipelineAsset.LitDeferredMaterial);
				DrawUnsupportedShaders();
				// DrawGizmos();


				RAPI.CleanupTempRT(SProps.GBuffer.GAux_TangentWorldSpaceAtlas);
				RAPI.CleanupTempRT(SProps.SSAO.SSAORawAtlas);
				RAPI.CleanupTempRT(SProps.SSAO.SSAOBlurAtlas);
				// RAPI.Context.DrawSkybox(RAPI.CurCamera);
				RAPI.CleanupTempRT(Shader.PropertyToID("Test"));

				RAPI.CleanupTempRT((SProps.GBuffer.G_AlbedoAtlas));
				RAPI.CleanupTempRT((SProps.GBuffer.G_NormalWorldSpaceAtlas));
				RAPI.CleanupTempRT((SProps.GBuffer.GAux_ClearNormalWorldSpaceAtlas));
				RAPI.CleanupTempRT((SProps.GBuffer.G_SpecularAtlas));
				RAPI.CleanupTempRT((SProps.GBuffer.G_BRDFAtlas));

				RAPI.CleanupTempRT(Shader.PropertyToID("Test2"));


				RAPI.CleanupTempRT((Shader.PropertyToID("1")));
				DrawGizmosBeforeFX();
				if (postFXStack.IsActive)
				{
					postFXStack.Render(frameBufferId);
				}

				DrawGizmosAfterFX();
				if (postFXStack.IsActive)
				{
					RAPI.Buffer.ReleaseTemporaryRT(frameBufferId);
				}
			}
			RAPI.CleanupTempRT(SProps.Shadows.DirShadowAtlasId);

			Submit();
		}

		void Setup () {
			RAPI.Context.SetupCameraProperties(RAPI.CurCamera);
			var flags = RAPI.CurCamera.clearFlags;
			
			if (postFXStack.IsActive) {
				if (flags > CameraClearFlags.Color) {
					flags = CameraClearFlags.Color;
				}
				RAPI.Buffer.GetTemporaryRT(
					frameBufferId, RAPI.CurCamera.pixelWidth, RAPI.CurCamera.pixelHeight,
					32, FilterMode.Bilinear, RenderTextureFormat.Default
				);
				RAPI.Buffer.SetRenderTarget(
					frameBufferId,
					RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store
				);
			}
			
			RAPI.Buffer.ClearRenderTarget(flags <= CameraClearFlags.Depth, flags == CameraClearFlags.Color,
				flags == CameraClearFlags.Color ? RAPI.CurCamera.backgroundColor.linear : Color.clear);
			
			RAPI.ExecuteBuffer();
		}

		void DrawVisibleGeometry (bool useDynamicBatching, bool useGPUInstancing, Material litDeferredMaterial)
		{
			var a = GL.GetGPUProjectionMatrix(RAPI.CurCamera.cameraToWorldMatrix, false);
			RAPI.Buffer.SetGlobalVector(Shader.PropertyToID("_nearFarPlanes"), new Vector4(RAPI.CurCamera.nearClipPlane, RAPI.CurCamera.farClipPlane, 0, 0 ));

			var m = RAPI.CurCamera.worldToCameraMatrix;
			if (SystemInfo.usesReversedZBuffer) {
				m.m20 = -m.m20;
				m.m21 = -m.m21;
				m.m22 = -m.m22;
				m.m23 = -m.m23;
			}
			// RAPI.Buffer.SetGlobalMatrix(Shader.PropertyToID("adfgdgf_WorldToCameraMatrix"),  RAPI.CurCamera.worldToCameraMatrix);

			// RAPI.Buffer.SetGlobalMatrix(Shader.PropertyToID("adfgdgf_CameraToWorldMatrix"),  RAPI.CurCamera.cameraToWorldMatrix);
            
			Matrix4x4 invProjectionMatrix = RAPI.CurCamera.projectionMatrix.inverse;
			RAPI.Buffer.SetGlobalMatrix(Shader.PropertyToID("_INVERSE_P"), invProjectionMatrix);
			
			RAPI.ExecuteBuffer();

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
			
			// RAPI.Buffer.Blit(null, BuiltinRenderTextureType.CurrentActive, litDeferredMaterial, litDeferredMaterial.FindPass(SProps.CameraRenderer.LitDeferredPassName));

			RAPI.Buffer.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
			RAPI.Buffer.DrawMesh(RAPI.fullscreenMesh, Matrix4x4.identity, litDeferredMaterial, 0, litDeferredMaterial.FindPass(SProps.CameraRenderer.LitDeferredPassName));
			RAPI.Buffer.SetViewProjectionMatrices(RAPI.CurCamera.worldToCameraMatrix, RAPI.CurCamera.projectionMatrix);
			// RAPI.ExecuteBuffer();

			RAPI.Context.DrawSkybox(RAPI.CurCamera);
			RAPI.ExecuteBuffer();
			
			RAPI.Buffer.SetRenderTarget(Shader.PropertyToID("_CameraFrameBuffer"), new RenderTargetIdentifier(Shader.PropertyToID("Test")));
			RAPI.ExecuteBuffer();

			//Draw transparent geometry
			drawingSettings.SetShaderPassName(1, SProps.CameraRenderer.LitShaderTagId);

			sortingSettings.criteria = SortingCriteria.CommonTransparent;
			drawingSettings.sortingSettings = sortingSettings;
			filteringSettings.renderQueueRange = RenderQueueRange.transparent;
			RAPI.Context.DrawRenderers(RAPI.CullingResults, ref drawingSettings, ref filteringSettings);
			
		}
		
				
		void DrawVisibleGeometryRefProbes (
			bool useDynamicBatching, bool useGPUInstancing
		) {

			var sortingSettings = new SortingSettings(RAPI.CurCamera) {
				criteria = SortingCriteria.CommonOpaque
			};
			var drawingSettings = new DrawingSettings(
				SProps.CameraRenderer.UnlitShaderTagId, sortingSettings
			) {
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

			RAPI.Context.DrawRenderers(
				RAPI.CullingResults, ref drawingSettings, ref filteringSettings
			);

			RAPI.Context.DrawSkybox(RAPI.CurCamera);

			sortingSettings.criteria = SortingCriteria.CommonTransparent;
			drawingSettings.sortingSettings = sortingSettings;
			filteringSettings.renderQueueRange = RenderQueueRange.transparent;

			RAPI.Context.DrawRenderers(
				RAPI.CullingResults, ref drawingSettings, ref filteringSettings
			);
		}

		void Submit () {
			
			RAPI.ExecuteBuffer();
			RAPI.Context.Submit();
		}
		
	}
	
}