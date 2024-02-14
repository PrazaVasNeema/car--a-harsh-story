using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
	public class Lighting
	{
		private const int MAX_DIR_LIGHT_COUNT = 4;

		private static int DIR_LIGHT_COUNT_ID = Shader.PropertyToID("_DirectionalLightCount");
		private static int dirLightColorsId = Shader.PropertyToID("_DirectionalLightColors");
		private static int dirLightDirectionsId = Shader.PropertyToID("_DirectionalLightDirections");

		private static Vector4[] dirLightColors = new Vector4[MAX_DIR_LIGHT_COUNT];
		private static Vector4[] dirLightDirections = new Vector4[MAX_DIR_LIGHT_COUNT];

		const string bufferName = "Lighting";
		
		private Shadows m_shadows = new Shadows();

		CommandBuffer buffer = new CommandBuffer {
			name = bufferName
		};

		public void Setup(ScriptableRenderContext context, CullingResults cullingResults, ShadowSettings shadowSettings)
		{
			buffer.BeginSample(bufferName);
			m_shadows.Setup(context, cullingResults, shadowSettings);
			SetupLights(cullingResults);
			m_shadows.Render();
			buffer.EndSample(bufferName);
			context.ExecuteCommandBuffer(buffer);
			buffer.Clear();
		}

		void SetupLights(CullingResults cullingResults)
		{
			NativeArray<VisibleLight> visibleLights = cullingResults.visibleLights;

			int dirLightCount = 0;
			for (int i = 0; i < visibleLights.Length; i++) {
				VisibleLight visibleLight = visibleLights[i];
				if (visibleLight.lightType == LightType.Directional) {
					SetupDirectionalLight(dirLightCount++, ref visibleLight);
					if (dirLightCount >= MAX_DIR_LIGHT_COUNT) {
						break;
					}
				}
			}

			buffer.SetGlobalInt(DIR_LIGHT_COUNT_ID, visibleLights.Length);
			buffer.SetGlobalVectorArray(dirLightColorsId, dirLightColors);
			buffer.SetGlobalVectorArray(dirLightDirectionsId, dirLightDirections);
		}

		void SetupDirectionalLight (int index, ref VisibleLight visibleLight) {
			dirLightColors[index] = visibleLight.finalColor;
			dirLightDirections[index] = -visibleLight.localToWorldMatrix.GetColumn(2);
			m_shadows.ReserveDirectionalShadows(visibleLight.light);
		}
		
		public void Cleanup () {
			m_shadows.Cleanup();
		}
	}
}