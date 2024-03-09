using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
	public partial class CameraRenderer {
		
		private const string BUFFER_NAME = "RenderCamera";

		private static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
		private static ShaderTagId litShaderTagId = new ShaderTagId("CustomLit");

		private readonly Lighting m_lighting = new Lighting();


		public void Render(Camera camera, bool useDynamicBatching, bool useGPUInstancing, ShadowSettings shadowSettings)
		{
			RAPI.CurCamera = camera;
			PrepareUIForSceneWindow();
			if (!RAPI.Cull(shadowSettings.maxDistance)) {
				return;
			}
			
			m_lighting.Setup(RAPI.Context, RAPI.CullingResults, shadowSettings);
			Setup();
			
			DrawVisibleGeometry(useDynamicBatching, useGPUInstancing);
			DrawUnsupportedShaders();
			DrawGizmos();
			RAPI.CleanupTempRT(Shadows.dirShadowAtlasId);
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
				enableInstancing = useGPUInstancing
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