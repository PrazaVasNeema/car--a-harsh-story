using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
	public partial class CameraRenderer {

		ScriptableRenderContext context;

		Camera camera;
		CullingResults cullingResults;

		const string bufferName = "RenderCamera";
		CommandBuffer buffer = new CommandBuffer {
			name = bufferName
		};

		static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
		static ShaderTagId litShaderTagId = new ShaderTagId("CustomLit");

		Lighting lighting = new Lighting();


		public void Render(ScriptableRenderContext context, Camera camera, ShadowSettings shadowSettings)
		{
			this.context = context;
			this.camera = camera;
			PrepareUIForSceneWindow();
			if (!Cull(shadowSettings.maxDistance)) {
				return;
			}
			lighting.Setup(context, cullingResults, shadowSettings);

			Setup();
			DrawVisibleGeometry();
			DrawUnsupportedShaders();
			DrawGizmos();
			//RenderUtils.CleanupTempRT(buffer, Shadows.dirShadowAtlasId);
			lighting.Cleanup();

			Submit();
		}

		void Setup () {
			context.SetupCameraProperties(camera);
			CameraClearFlags flags = camera.clearFlags;
			buffer.ClearRenderTarget(
				flags <= CameraClearFlags.Depth,
				flags == CameraClearFlags.Color,
				flags == CameraClearFlags.Color ?
					camera.backgroundColor.linear : Color.clear);
			buffer.BeginSample(bufferName);
		
			RenderUtils.ExecuteBuffer(buffer, context);
			
		}

		bool Cull ()
		{
			if (camera.TryGetCullingParameters(out ScriptableCullingParameters p)) {
				cullingResults = context.Cull(ref p);
				return true;
			}
			return false;
		}

		void DrawVisibleGeometry ()
		{
			var sortingSettings = new SortingSettings(camera)
			{
				criteria = SortingCriteria.CommonOpaque
			};
			var drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings);
			drawingSettings.SetShaderPassName(1, litShaderTagId);
			var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

			context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

			context.DrawSkybox(camera);

			//Draw transparent geometry
			sortingSettings.criteria = SortingCriteria.CommonTransparent;
			drawingSettings.sortingSettings = sortingSettings;
			filteringSettings.renderQueueRange = RenderQueueRange.transparent;
			context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
		}

		void Submit () {
			buffer.EndSample(bufferName);
			RenderUtils.ExecuteBuffer(buffer, context);
			context.Submit();
		}

		bool Cull(float maxShadowDistance)
		{

			if (camera.TryGetCullingParameters(out ScriptableCullingParameters p))
			{
				p.shadowDistance = Mathf.Min(maxShadowDistance, camera.farClipPlane);
				cullingResults = context.Cull(ref p);
				return true;
			}
			return false;
		}
		
	}
}