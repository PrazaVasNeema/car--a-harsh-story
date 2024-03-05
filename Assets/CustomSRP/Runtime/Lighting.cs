using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
	public class Lighting
	{
		private const int MAX_DIR_LIGHT_COUNT = 1;
		private const int MAX_OTHER_LIGHT_COUNT = 64;
		private const string BUFFER_NAME = "Lighting";

		// private static int _dirLightCountID = Shader.PropertyToID("_DirectionalLightCount");
		private static int _dirLightCountId = Shader.PropertyToID("_DirLightCount");
		private static int _dirLightColorId = Shader.PropertyToID("_DirectionalLightColor");
		private static int _dirLightDirectionId = Shader.PropertyToID("_DirectionalLightDirection");
		private static Vector4 _dirLightColor = new Vector4();
		private static Vector4 _dirLightDirection = new Vector4();

		private static int _otherLightCountId = Shader.PropertyToID("_OtherLightCount");
		private static int _otherLightColorsId = Shader.PropertyToID("_OtherLightColors");
		private static int _otherLightPositionsId = Shader.PropertyToID("_OtherLightPositions");
		private static Vector4[] _otherLightColors = new Vector4[MAX_OTHER_LIGHT_COUNT];
		private static Vector4[] _otherLightPositions = new Vector4[MAX_OTHER_LIGHT_COUNT];
		
		private Shadows m_shadows = new Shadows();

		public void Setup(ScriptableRenderContext context, CullingResults cullingResults, ShadowSettings shadowSettings)
		{
			RAPI.Buffer.BeginSample(BUFFER_NAME);
			//m_shadows.Setup(context, cullingResults, shadowSettings);
			SetupLights(cullingResults);
			//m_shadows.Render();
			RAPI.Buffer.EndSample(BUFFER_NAME);
			RAPI.Context.ExecuteCommandBuffer(RAPI.Buffer);
			RAPI.Buffer.Clear();
		}

		void SetupLights(CullingResults cullingResults)
		{
			var visibleLights = cullingResults.visibleLights;
			int dirLightCount = 0;
			int otherLightCount = 0;
			
			foreach (var visibleLight in visibleLights)
			{
				switch (visibleLight.lightType) {
					case LightType.Directional:
						if (dirLightCount < MAX_DIR_LIGHT_COUNT) {
							SetupDirectionalLight(visibleLight);
							dirLightCount++;
						}
						break;
					case LightType.Point:
						if (otherLightCount < MAX_OTHER_LIGHT_COUNT) {
							SetupPointLight(otherLightCount++, visibleLight);
						}
						break;
				}
			}

			RAPI.Buffer.SetGlobalInt(_dirLightCountId, dirLightCount);
			RAPI.Buffer.SetGlobalVector(_dirLightColorId, _dirLightColor);
			RAPI.Buffer.SetGlobalVector(_dirLightDirectionId, -_dirLightDirection);
			
			RAPI.Buffer.SetGlobalInt(_otherLightCountId, otherLightCount);
			if (otherLightCount > 0) {
				Debug.Log($"---------------\n{_otherLightPositions}\n-------------------------");
				RAPI.Buffer.SetGlobalVectorArray(_otherLightColorsId, _otherLightColors);
				RAPI.Buffer.SetGlobalVectorArray(_otherLightPositionsId, _otherLightPositions);
			}
		}

		void SetupDirectionalLight (VisibleLight visibleLight) {
			_dirLightColor = visibleLight.finalColor;
			_dirLightDirection = -visibleLight.localToWorldMatrix.GetColumn(2);
			//m_shadows.ReserveDirectionalShadows(visibleLight.light);
		}
		
		void SetupPointLight (int index, VisibleLight visibleLight) {
			_otherLightColors[index] = visibleLight.finalColor;
			Vector4 position = visibleLight.localToWorldMatrix.GetColumn(3);
			position.w = 1f / Mathf.Max(visibleLight.range * visibleLight.range, 0.00001f);
			_otherLightPositions[index] = position;
		}
		
	}
	
}