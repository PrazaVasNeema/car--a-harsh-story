Shader "CustomSRP/S_Lit"
{
	Properties
	{
		_BaseMap("Texture", 2D) = "white" {}
		_BaseColor("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Metallic ("Metallic", Range(0, 1)) = 0
		_Roughness ("Roughness", Range(0, 1)) = 0.5
		_Reflectance ("Reflectance", Range(0, 1)) = 0.5

		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0
		[Enum(Off, 0, On, 1)] _ZWrite ("Z Write", Float) = 1
		
		[Enum(Off, 0, On, 1)] _WorkingVar ("check????", Float) = 1
		
		[Toggle(_PREMULTIPLY_ALPHA)] _PremulAlpha ("Premultiply Alpha", Float) = 0
				[Toggle(_RECEIVE_SHADOWS)] _ReceiveShadows ("Receive Shadows", Float) = 1
		
_DepthLevel ("Depth Level", Range(1, 3)) = 1
	}

	SubShader
	{
		Pass
		{
			Tags {
				"LightMode" = "CustomLit"
			}

			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]
			
			

			HLSLPROGRAM

			
			#define CHECK _WorkingVar
			#pragma target 3.5
			// #pragma enable_d3d11_debug_symbols
			#pragma multi_compile_instancing
			#pragma multi_compile _ _DIRECTIONAL_PCF_NONE _DIRECTIONAL_PCF2x2 _DIRECTIONAL_PCF4x4 _DIRECTIONAL_PCF6x6 _DIRECTIONAL_PCF8x8
			#pragma multi_compile _ _DIR_LIGHT_ON
			#pragma multi_compile _ _OTHER_LIGHT_COUNT_20 _OTHER_LIGHT_COUNT_15 _OTHER_LIGHT_COUNT_10 _OTHER_LIGHT_COUNT_5
			#pragma multi_compile _ CASCEDE_COUNT_2 CASCEDE_COUNT_4
			#pragma shader_feature _PREMULTIPLY_ALPHA
			#pragma shader_feature _RECEIVE_SHADOWS
			#pragma vertex vert
			#pragma fragment frag
			#include "../ShaderLibrary/Common.hlsl"
			#define MAX_OTHER_LIGHT_COUNT 64
TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);

			UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
				UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
				UNITY_DEFINE_INSTANCED_PROP(float4, _BaseMap_ST)
				UNITY_DEFINE_INSTANCED_PROP(float, _Metallic)
				UNITY_DEFINE_INSTANCED_PROP(float, _Roughness)
				UNITY_DEFINE_INSTANCED_PROP(float, _Reflectance)

				

			UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

CBUFFER_START(_CustomLight)
float3 _DirectionalLightDirection;
			
int _OtherLightCount;
float4 _OtherLightColors[MAX_OTHER_LIGHT_COUNT];
float4 _OtherLightPositions[MAX_OTHER_LIGHT_COUNT];

	int _DirectionalLightCount;
	float4 _DirectionalLightColors;
	float4 _DirectionalLightDirections;
	float4 _DirectionalLightShadowData;
CBUFFER_END
			
			#include "S_LitPass.hlsl"
			ENDHLSL
		}

		Pass {
			Tags {
				"LightMode" = "ShadowCaster"
			}

			ColorMask 0

			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex ShadowCasterPassVertex
			#pragma shader_feature _ _SHADOWS_CLIP _SHADOWS_DITHER
			#pragma multi_compile_instancing
			#pragma enable_d3d11_debug_symbols
			#pragma fragment ShadowCasterPassFragment
			#include "Assets/CustomSRP/Shaders/ShadowCasterPass.hlsl"
			ENDHLSL
		}



		Pass {
			Tags {
				"LightMode" = "DepthBuffer"
			}


			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex vert
			#pragma multi_compile_instancing
			#pragma enable_d3d11_debug_symbols
			#pragma fragment frag
			#include "UnityCG.cginc"

			#include "SSAO_DEPTH_NORMALS.hlsl"
			ENDHLSL
		}

		Pass {

			Tags {
				"LightMode" = "2"
			}

			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex vert
			#pragma multi_compile_instancing
			#pragma enable_d3d11_debug_symbols
			#pragma fragment frag
			#include "../ShaderLibrary/Common.hlsl"

			#include "S_SSAOPass.hlsl"
			ENDHLSL
		}


	}
}