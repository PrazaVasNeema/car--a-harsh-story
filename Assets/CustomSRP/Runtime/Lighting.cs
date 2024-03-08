using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
	public class Lighting
	{
		private const int MAX_DIR_LIGHT_COUNT = 1;
		private const int MAX_OTHER_LIGHT_COUNT = 20;
		private const string BUFFER_NAME = "Lighting";

		// Dir lights
		private static int _dirLightCountId = Shader.PropertyToID("_DirLightCount");
		private static int _dirLightColorId = Shader.PropertyToID("_DirectionalLightColor");
		private static int _dirLightDirectionId = Shader.PropertyToID("_DirectionalLightDirection");
		private static int dirLightShadowDataId = Shader.PropertyToID("_DirectionalLightShadowData");
		private static int dirCascades = Shader.PropertyToID("_CascadeCount");
		private static Vector4 _dirLightColor = new Vector4();
		private static Vector4 _dirLightDirection = new Vector4();
		private static Vector4 _dirLightShadowData = new Vector4();
		// ---
		
		// Point lights
		private static int _otherLightCountId = Shader.PropertyToID("_OtherLightCount");
		private static int _otherLightColorsId = Shader.PropertyToID("_OtherLightColors");
		private static int _otherLightPositionsId = Shader.PropertyToID("_OtherLightPositions");
		private static Vector4[] _otherLightColors = new Vector4[MAX_OTHER_LIGHT_COUNT];
		private static Vector4[] _otherLightPositions = new Vector4[MAX_OTHER_LIGHT_COUNT];
		// ---
		
		private Shadows m_shadows = new Shadows();

		public void Setup(ScriptableRenderContext context, CullingResults cullingResults, ShadowSettings shadowSettings)
		{
			RAPI.Buffer.BeginSample(BUFFER_NAME);
			m_shadows.Setup(shadowSettings);
			SetupLights(cullingResults);
			m_shadows.Render();
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
			if (dirLightCount == 1)
			{
				RAPI.SetKeyword("_DIR_LIGHT_ON", true);
				RAPI.Buffer.SetGlobalVector(_dirLightColorId, _dirLightColor);
				RAPI.Buffer.SetGlobalVector(_dirLightDirectionId, -_dirLightDirection);
				RAPI.Buffer.SetGlobalVector(dirLightShadowDataId, _dirLightShadowData);
				RAPI.Buffer.SetGlobalFloat(dirCascades, 4);
				
			}
			else
			{
				RAPI.SetKeyword("_DIR_LIGHT_ON", false);
			}

			RAPI.Buffer.SetGlobalInt(_otherLightCountId, otherLightCount);
			if (otherLightCount > 0) {
				RAPI.Buffer.SetGlobalVectorArray(_otherLightColorsId, _otherLightColors);
				RAPI.Buffer.SetGlobalVectorArray(_otherLightPositionsId, _otherLightPositions);



				// for (int i = 0; i < MAX_OTHER_LIGHT_COUNT; i++)
				// {
				// 	string keyword = "_OTHER_LIGHT_COUNT_" + i + 1;
				// 	if (otherLightCount < MAX_OTHER_LIGHT_COUNT)
				// 	{
				// 		RAPI.SetKeyword(keyword, true);
				// 	}
				// 	else
				// 	{
				// 		RAPI.SetKeyword(keyword, false);
				// 	}
				// }
			}
			int otherLightCountDiv5 = (otherLightCount - 1) / 5;

			for (int i = 0; i <= (MAX_OTHER_LIGHT_COUNT - 1) / 5; i++)
			{
				string keyword = "_OTHER_LIGHT_COUNT_" + (i+1) * 5;
				// Debug.Log(keyword);
				if (otherLightCount != 0 && i == otherLightCountDiv5)
				{
					// Debug.Log(keyword);
					RAPI.SetKeyword(keyword, true);
				}
				else
				{
					// Debug.Log($"and NOT: {keyword}");
					RAPI.SetKeyword(keyword, false);
				}
			}
		}

		void SetupDirectionalLight (VisibleLight visibleLight)
		{
			_dirLightColor = visibleLight.finalColor;
			_dirLightDirection = -visibleLight.localToWorldMatrix.GetColumn(2);
			_dirLightShadowData = m_shadows.ReserveDirectionalShadows(visibleLight.light);
		}
		
		void SetupPointLight (int index, VisibleLight visibleLight) {
			_otherLightColors[index] = visibleLight.finalColor;
			Vector4 position = visibleLight.localToWorldMatrix.GetColumn(3);
			// 1/range^2
			position.w = 1f / Mathf.Max(visibleLight.range * visibleLight.range, 0.00001f);
			_otherLightPositions[index] = position;
		}
		
	}
	
}