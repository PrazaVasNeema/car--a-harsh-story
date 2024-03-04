using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
	public class Lighting
	{
		private const int MAX_DIR_LIGHT_COUNT = 1;
		private const string BUFFER_NAME = "Lighting";

		// private static int _dirLightCountID = Shader.PropertyToID("_DirectionalLightCount");
		private static int _dirLightColorId = Shader.PropertyToID("_DirectionalLightColor");
		private static int _dirLightDirectionId = Shader.PropertyToID("_DirectionalLightDirection");
		private static Vector4 _dirLightColor = new Vector4();
		private static Vector4 _dirLightDirection = new Vector4();
		
		private Shadows m_shadows = new Shadows();

		public void Setup(ScriptableRenderContext context, CullingResults cullingResults, ShadowSettings shadowSettings)
		{
			RAPI.Buffer.BeginSample(BUFFER_NAME);
			//m_shadows.Setup(context, cullingResults, shadowSettings);
			SetupDirLight(cullingResults);
			//m_shadows.Render();
			RAPI.Buffer.EndSample(BUFFER_NAME);
			RAPI.Context.ExecuteCommandBuffer(RAPI.Buffer);
			RAPI.Buffer.Clear();
		}

		void SetupDirLight(CullingResults cullingResults)
		{
			var visibleLights = cullingResults.visibleLights;

			if (visibleLights[0].lightType == LightType.Directional) {
				SetupDirectionalLight(visibleLights[0]);
				// if (dirLightCount >= MAX_DIR_LIGHT_COUNT) {
				// 	break;
				// }
			}

			RAPI.Buffer.SetGlobalVector(_dirLightColorId, _dirLightColor);
			RAPI.Buffer.SetGlobalVector(_dirLightDirectionId, -_dirLightDirection);
		}

		void SetupDirectionalLight (VisibleLight visibleLight) {
			_dirLightColor = visibleLight.finalColor;
			_dirLightDirection = -visibleLight.localToWorldMatrix.GetColumn(2);
			//m_shadows.ReserveDirectionalShadows(visibleLight.light);
		}
		
	}
	
}