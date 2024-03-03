Shader "CustomSRP/Unlit"
{
	Properties
	{
		_BaseMap("Texture", 2D) = "white" {}
		_BaseColor("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0
		[Enum(Off, 0, On, 1)] _ZWrite ("Z Write", Float) = 1
	}

	SubShader
	{
		Pass
		{
			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]

			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex vert
			#pragma fragment Fragment
			#include "UnlitPass.hlsl"
			ENDHLSL
		}
	}
}