Shader "DopeRP/Shaders/Decals"
{
	Properties
	{
		
		[Toggle(_CONTRIBUTE_ALBEDO)] _ContributeAlbedo ("Contribute Albedo", Float) = 0
		_BaseMap("Albedo Texture", 2D) = "(0,0,0,0)" {}
		_BaseColor("Color", Color) = (1.0, 1.0, 1.0, 1.0)

		[Toggle(_CONTRIBUTE_NORMAL)] _ContributeNormals ("Contribute Normals", Float) = 0
		[NoScaleOffset] _NormalMap("Normal Texture", 2D) = "bump" {}
		_NormalScale("Normal Scale", Range(0, 1)) = 1

		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0
		
		_Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		[Toggle(_CLIPPING)] _Clipping ("Alpha Clipping", Float) = 0
		
		[Enum(Off, 0, On, 1)] _ZWrite ("Z Write", Float) = 1
		
	}

	SubShader
	{

		Pass {
			Name "DecalsPass"
			Tags {
				"LightMode" = "DecalsPass"
			}
			
			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]

			HLSLPROGRAM
			#pragma target 3.5
			#pragma multi_compile_instancing
			// #pragma enable_d3d11_debug_symbols
			#pragma shader_feature _CLIPPING
			#pragma shader_feature _CONTRIBUTE_ALBEDO
			#pragma shader_feature _CONTRIBUTE_NORMAL


			#pragma vertex vert
			#pragma fragment frag

			#include "Assets/DopeRP/GPU/HLSL/Common/Common.hlsl"

			TEXTURE2D(_BaseMap);
			SAMPLER(sampler_BaseMap);

			TEXTURE2D(_NormalMap);
			SAMPLER(sampler_NormalMap);
			
			TEXTURE2D(_PositionViewSpace);
			SAMPLER(sampler_PositionViewSpace);

			TEXTURE2D(_NormalViewSpace);
			SAMPLER(sampler_NormalViewSpace);

			TEXTURE2D(_TangentViewSpace);
			SAMPLER(sampler_TangentViewSpace);
			
			CBUFFER_START(Decals)
			
				float4 _ScreenSize;
			
			CBUFFER_END

			UNITY_INSTANCING_BUFFER_START(UnityPerMaterial_DECALS)
			
				UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
				UNITY_DEFINE_INSTANCED_PROP(float, _NormalScale)
				UNITY_DEFINE_INSTANCED_PROP(float, _Cutoff)

			UNITY_INSTANCING_BUFFER_END(UnityPerMaterial_DECALS)

			struct MeshData {
				float4 position : POSITION;
			    float3 normal   : NORMAL;
				float2 uv   : TEXCOORD0;
				float4 tangentOS : TANGENT;
				
			    UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Interpolators {
			    float4 positionSV : SV_POSITION;
			    float3 positionVS   : TEXCOORD0;
			    float3 normalVS   : TEXCOORD1;
				float3 positionWS : TEXCOORD2;
				float2 uv   : TEXCOORD3;
				float4 tangentWS : VAR_TANGENT;
				
			    UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			Interpolators vert(MeshData i)
			{
			    UNITY_SETUP_INSTANCE_ID(i);
			    Interpolators o;
			    UNITY_TRANSFER_INSTANCE_ID(i, o);
			   
			    o.positionSV = TransformObjectToHClip(i.position);
			    o.positionVS = TransformWorldToView(TransformObjectToWorld(i.position));
			    o.normalVS = TransformWorldToViewNormal(TransformObjectToWorldNormal(i.normal));

				o.positionWS = TransformObjectToWorld(i.position);

				o.uv = i.uv;

				o.tangentWS = float4(TransformObjectToWorldDir(i.tangentOS.xyz), i.tangentOS.w);
			    return o;
			}
			

			struct fragOutput
			{
			    float4 decalsAlbedoAtlas : SV_Target0;
			    float4 decalsNormalAtlas : SV_Target1;
			};

			float3 GetNormalTS (float2 baseUV) {
				float4 map = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, baseUV);
				float scale = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _NormalScale);
				float3 normal = DecodeNormal(map, scale);
				return normal;
			}

			float3 NormalTangentToWorld (float3 normalTS, float3 normalWS, float4 tangentWS) {
				float3x3 tangentToWorld = CreateTangentToWorld(normalWS, tangentWS.xyz, tangentWS.w);
				return TransformTangentToWorld(normalTS, tangentToWorld);
			}	

			fragOutput frag(Interpolators i)
			{
				
				UNITY_SETUP_INSTANCE_ID(i);
				fragOutput o;
				
				float4 sampleDepth = SAMPLE_TEXTURE2D(_PositionViewSpace, sampler_PositionViewSpace, i.positionSV.xy * _ScreenSize.zw);
				float4 worldPos = float4(TransformViewToWorld(sampleDepth.xyz),1);
				float4 objectPos = float4(TransformWorldToObject(worldPos), 1);

				
				clip(0.5 - abs(objectPos.xyz));
				
				
				float2 texCoords = objectPos.xy + 0.5;

				#if defined(_CONTRIBUTE_NORMAL)
				
					float3 normal = normalize(SAMPLE_TEXTURE2D(_NormalViewSpace, sampler_NormalViewSpace, i.positionSV.xy * _ScreenSize.zw).xyz);
					normal = float4(mul(UNITY_MATRIX_I_V, normal));
					float4 tangent = normalize(SAMPLE_TEXTURE2D(_TangentViewSpace, sampler_TangentViewSpace, i.positionSV.xy * _ScreenSize.zw));
					
					o.decalsNormalAtlas = float4(NormalTangentToWorld(GetNormalTS(texCoords), normal, tangent),1);

				#else

					o.decalsNormalAtlas = 0;
				
				#endif
			
				
				#if defined(_CONTRIBUTE_ALBEDO)
				
					float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, texCoords);
	
					#if defined(_CLIPPING)
						o.decalsAlbedoAtlas = 0;
						clip(baseColor.a - UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Cutoff));
					#endif
	
					baseColor *= UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial_DECALS, _BaseColor);
					o.decalsAlbedoAtlas = baseColor;

				#endif

				return o;
				
			}
			
			ENDHLSL
		}


	}

}