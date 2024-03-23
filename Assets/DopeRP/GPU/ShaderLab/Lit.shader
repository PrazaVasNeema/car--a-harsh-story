Shader "DopeRP/Shaders/Lit"
{
	Properties
	{
		_AlbedoMap("Albedo Map", 2D) = "white" {}
		_BaseColor("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		
		[NoScaleOffset] _NormalMap("Normal Map", 2D) = "bump" {}
		_NormalScale("Normal Scale", Range(0, 1)) = 1
		
		_Metallic ("Metallic", Range(0, 1)) = 0
		_Roughness ("Roughness", Range(0, 1)) = 0.5
		_Reflectance ("Reflectance", Range(0, 1)) = 0.5
		
		[NoScaleOffset] _EmissionMap("Emission", 2D) = "white" {}
		[HDR] _EmissionColor("Emission", Color) = (0.0, 0.0, 0.0, 0.0)

		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0
		[Enum(Off, 0, On, 1)] _ZWrite ("Z Write", Float) = 1
		
		[Toggle(_PREMULTIPLY_ALPHA)] _PremulAlpha ("Premultiply Alpha", Float) = 0
//		[Toggle(_RECEIVE_SHADOWS)] _ReceiveShadows ("Receive Shadows", Float) = 1
		
//		_DepthLevel ("Depth Level", Range(1, 3)) = 1
		

	}

	SubShader
	{
		Pass
		{
			Tags {
				"LightMode" = "Lit"
			}

			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]
			
			

			HLSLPROGRAM

			
			#pragma target 3.5
			// #pragma enable_d3d11_debug_symbols
			#pragma multi_compile_instancing
			
			#pragma multi_compile _ _DIR_LIGHT_ON
			#pragma multi_compile _ _OTHER_LIGHT_COUNT_20 _OTHER_LIGHT_COUNT_15 _OTHER_LIGHT_COUNT_10 _OTHER_LIGHT_COUNT_5
			
			#pragma multi_compile _ SHADOWS_ON
			#pragma multi_compile _ _DIRECTIONAL_PCF_NONE _DIRECTIONAL_PCF2x2 _DIRECTIONAL_PCF4x4 _DIRECTIONAL_PCF6x6 _DIRECTIONAL_PCF8x8
			#pragma multi_compile _ CASCADE_COUNT_2 CASCADE_COUNT_4
			
			#pragma multi_compile _ SSAO_ON
			
			#pragma shader_feature _PREMULTIPLY_ALPHA
			// #pragma shader_feature _RECEIVE_SHADOWS
			#pragma vertex vert
			#pragma fragment frag
			#include "Assets/DopeRP/GPU/HLSL/LitPass.hlsl"
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
			Name "GBufferPass"
			Tags {
				"LightMode" = "GBufferPass"
			}

			HLSLPROGRAM
			#pragma target 3.5
			#pragma multi_compile_instancing
			#pragma enable_d3d11_debug_symbols
			#pragma vertex vert
			#pragma fragment frag
			#include "Assets/DopeRP/GPU/HLSL/GBufferPass.hlsl"
			ENDHLSL
		}
		
	}

}