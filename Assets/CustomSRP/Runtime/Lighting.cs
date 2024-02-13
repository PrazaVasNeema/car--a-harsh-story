using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomSRP.Runtime
{
	public class Lighting
	{
		const int maxDirLightCount = 4;

		private static int dirLightCountId = Shader.PropertyToID("_DirectionalLightCount");
		private static int dirLightColorsId = Shader.PropertyToID("_DirectionalLightColors");
		private static int dirLightDirectionsId = Shader.PropertyToID("_DirectionalLightDirections");

		private static Vector4[] dirLightColors = new Vector4[maxDirLightCount];
		private static Vector4[] dirLightDirections = new Vector4[maxDirLightCount];

		const string bufferName = "Lighting";

		CommandBuffer buffer = new CommandBuffer {
			name = bufferName
		};

		public void Setup(ScriptableRenderContext context, CullingResults cullingResults)
		{
			buffer.BeginSample(bufferName);
			SetupLights(cullingResults);
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
					if (dirLightCount >= maxDirLightCount) {
						break;
					}
				}
			}

			buffer.SetGlobalInt(dirLightCountId, visibleLights.Length);
			buffer.SetGlobalVectorArray(dirLightColorsId, dirLightColors);
			buffer.SetGlobalVectorArray(dirLightDirectionsId, dirLightDirections);
		}

		void SetupDirectionalLight (int index, ref VisibleLight visibleLight) {
			dirLightColors[index] = visibleLight.finalColor;
			dirLightDirections[index] = -visibleLight.localToWorldMatrix.GetColumn(2);
		}
	}
}