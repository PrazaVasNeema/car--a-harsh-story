Shader "Hidden/Custom RP/Post FX Stack" {
	
	SubShader {
		Cull Off
		ZTest Always
		ZWrite Off
		
		HLSLINCLUDE
		#include "Assets/DopeRP/GPU/HLSL/Common/Common.hlsl"
		#include "Assets/DopeRP/GPU/HLSL/PostFXStackPasses.hlsl"
		ENDHLSL

		Pass {
			Name "Copy"
			
			HLSLPROGRAM
				#pragma target 3.5
				#pragma vertex DefaultPassVertex
				#pragma fragment CopyPassFragment
			ENDHLSL
		}
	}
}