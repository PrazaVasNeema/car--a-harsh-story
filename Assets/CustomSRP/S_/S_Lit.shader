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
			#pragma enable_d3d11_debug_symbols
			#pragma multi_compile _ _DIRECTIONAL_PCF_NONE _DIRECTIONAL_PCF2x2 _DIRECTIONAL_PCF4x4 _DIRECTIONAL_PCF8x8
			#pragma vertex vert
			#pragma fragment frag
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
			#pragma enable_d3d11_debug_symbols
			#pragma fragment ShadowCasterPassFragment
			#include "Assets/CustomSRP/Shaders/ShadowCasterPass.hlsl"
			ENDHLSL
		}
	}
}