using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
	public class Lighting
	{
		private const string BUFFER_NAME = "Lighting";

		private const int MAX_DIR_LIGHT_COUNT = 1;
		private const int MAX_OTHER_LIGHT_COUNT = 20;
		

		// Dir light

		// private static int dirLightShadowDataId = Shader.PropertyToID("_DirectionalLightShadowData");
		private static Vector4 DirLightDirection = new Vector4();
		private static Vector4 DirLightColor = new Vector4();
		// private static Vector4 DirLightIntensity = new Vector4();
		// private static Vector4 _dirLightShadowData = new Vector4();
		
		// ---
		
		// Point lights
		
		private static Vector4[] OtherLightPositions = new Vector4[MAX_OTHER_LIGHT_COUNT];
		private static Vector4[] OtherLightColors = new Vector4[MAX_OTHER_LIGHT_COUNT];
		// private static Vector4[] OtherLightIntensities = new Vector4[MAX_OTHER_LIGHT_COUNT];
		
		// ---
		
		// private Shadows m_shadows = new Shadows();

		public void Setup(ShadowSettings shadowSettings)
		{
			RAPI.Buffer.BeginSample(BUFFER_NAME);
			// m_shadows.Setup(shadowSettings);
			SetupLights();
			// m_shadows.Render();
			RAPI.Buffer.EndSample(BUFFER_NAME);
			RAPI.Context.ExecuteCommandBuffer(RAPI.Buffer);
			RAPI.Buffer.Clear();
		}

		void SetupLights()
		{
			var visibleLights = RAPI.CullingResults.visibleLights;
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

			if (dirLightCount == 1)
			{
				
				RAPI.SetKeyword(SProps.LightingMain.DirLightOnKeyword, true);
				RAPI.Buffer.SetGlobalVector(SProps.LightingMain.DirLightDirectionId, -DirLightDirection);
				RAPI.Buffer.SetGlobalVector(SProps.LightingMain.DirLightColorId, DirLightColor);
				// RAPI.Buffer.SetGlobalVector(SProps.LightingMain.DirLightIntensityId, DirLightIntensity);
				// RAPI.Buffer.SetGlobalVector(dirLightShadowDataId, _dirLightShadowData);
				
			}
			else
			{
				
				RAPI.SetKeyword(SProps.LightingMain.DirLightOnKeyword, false);
				
			}

			RAPI.Buffer.SetGlobalInt(SProps.LightingMain.OtherLightCountId, otherLightCount);
			if (otherLightCount > 0) {
				
				RAPI.Buffer.SetGlobalVectorArray(SProps.LightingMain.OtherLightPositionsId, OtherLightPositions);
				RAPI.Buffer.SetGlobalVectorArray(SProps.LightingMain.OtherLightColorsId, OtherLightColors);
				// RAPI.Buffer.SetGlobalVectorArray(SProps.LightingMain.DirLightIntensityId, OtherLightIntensities);
				
			}
			
			int otherLightCountDiv5 = (otherLightCount - 1) / 5;

			for (int i = 0; i <= (MAX_OTHER_LIGHT_COUNT - 1) / 5; i++)
			{
				string keyword = SProps.LightingMain.OtherLightnCountKeyword_base + (i+1) * 5;
				if (otherLightCount != 0 && i == otherLightCountDiv5)
				{
					RAPI.SetKeyword(keyword, true);
				}
				else
				{
					RAPI.SetKeyword(keyword, false);
				}
			}
		}

		void SetupDirectionalLight (VisibleLight visibleLight)
		{
			DirLightDirection = -visibleLight.localToWorldMatrix.GetColumn(2);
			DirLightColor = visibleLight.finalColor;
			// _dirLightShadowData = m_shadows.ReserveDirectionalShadows(visibleLight.light);
		}
		
		void SetupPointLight (int index, VisibleLight visibleLight) {
			Vector4 position = visibleLight.localToWorldMatrix.GetColumn(3);
			// 1/range^2
			position.w = 1f / Mathf.Max(visibleLight.range * visibleLight.range, 0.00001f);
			OtherLightPositions[index] = position;
			OtherLightColors[index] = visibleLight.finalColor;
		}
		
	}
	
}