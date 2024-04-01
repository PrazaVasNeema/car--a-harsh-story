Shader "DopeRP/Shaders/Decals"
{
	Properties
	{
		[Toggle(_IS_DAMAGE_TYPE)] _IsDamageDecalType ("Is Damage Decal Type (not Artistic)", Float) = 0
		
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
			#pragma shader_feature _IS_DAMAGE_TYPE


			#pragma vertex vert
			#pragma fragment frag

			#include "Assets/DopeRP/GPU/HLSL/Common/Common.hlsl"

			TEXTURE2D(_BaseMap);
			SAMPLER(sampler_BaseMap);

			TEXTURE2D(_NormalMap);
			SAMPLER(sampler_NormalMap);
			

			TEXTURE2D(_G_NormalWorldSpaceAtlas);
			SAMPLER(sampler_G_NormalWorldSpaceAtlas);

			TEXTURE2D(_GAux_TangentWorldSpaceAtlas);
			SAMPLER(sampler_GAux_TangentWorldSpaceAtlas);

			TEXTURE2D(Test);
			SAMPLER(samplerTest);
			
			
			CBUFFER_START(Decals)
			
				float4 _ScreenSize;
			
				float2 _nearFarPlanes;
				float4x4 _INVERSE_P;
			
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
			    float4 decalsArtisticAlbedoAtlas : SV_Target0;
			    float4 decalsArtisticNormalAtlas : SV_Target1;
				
				float4 decalsDamageAlbedoAtlas : SV_Target2;
			    float4 decalsDamageNormalAtlas : SV_Target3;
			};

			float3 GetNormalTS (float2 baseUV) {
				float4 map = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, baseUV);
				float scale = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _NormalScale);
				float3 normal = DecodeNormal(map, scale);
				return normal;
			}


			fragOutput frag(Interpolators i)
			{
				
				UNITY_SETUP_INSTANCE_ID(i);
				fragOutput o;

				float2 uv = i.positionSV.xy * _ScreenSize.zw;
				
				float depth = SAMPLE_TEXTURE2D(Test, samplerTest, uv).r;
				depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, depth);
				float sceneZ =CalcLinearZ(depth, _nearFarPlanes.x, _nearFarPlanes.y);
				float4 clipSpacePosition = float4((uv * 2.0 - 1.0) * sceneZ/depth, sceneZ, 1.0 * sceneZ/depth);
				float4 viewSpacePosition = mul(_INVERSE_P, clipSpacePosition);
			
				viewSpacePosition /= viewSpacePosition.w;
				
				float4 worldPos = float4(TransformViewToWorld(viewSpacePosition.xyz),1);
				float4 objectPos = float4(TransformWorldToObject(worldPos), 1);

				
				clip(0.5 - abs(objectPos.xyz));
				
				
				float2 texCoords = objectPos.xy + 0.5;

				#if defined(_CONTRIBUTE_NORMAL)
				
					float3 normalWS = normalize(SAMPLE_TEXTURE2D(_G_NormalWorldSpaceAtlas, sampler_G_NormalWorldSpaceAtlas, uv).xyz);
					float4 tangentWS = normalize(SAMPLE_TEXTURE2D(_GAux_TangentWorldSpaceAtlas, sampler_GAux_TangentWorldSpaceAtlas, uv));

				
					// float isFlat = or(when_eq((int)normalOutput.x*10, 5), when_eq((int)normalOutput.x*10, 128)) * when_eq(normalOutput.y, 128) * when_eq(normalOutput.z, 255);

					float3 targetValue = float3(0.5, 0.5, 1);
					float epsilon = .05; // Tolerance for the comparison
					
					bool isEqual = all(abs(GetNormalTS(texCoords) - DecodeNormal(targetValue.xyzz, 1)) < epsilon);

					
				
					#if !defined(_IS_DAMAGE_TYPE)

									o.decalsArtisticNormalAtlas = float4(NormalTangentToWorld(GetNormalTS(texCoords), normalWS, tangentWS),1) * (1-isEqual);

				#else

				o.decalsArtisticNormalAtlas = float4(NormalTangentToWorld(GetNormalTS(texCoords), normalWS, tangentWS),1) * (1-isEqual);
				o.decalsDamageNormalAtlas = 0;


				#endif

				o.decalsArtisticNormalAtlas = 0;
				o.decalsDamageNormalAtlas = float4(NormalTangentToWorld(GetNormalTS(texCoords), normalWS, tangentWS),1) * (1-isEqual);

				// o.decalsNormalAtlas = 0;

				#else

					o.decalsArtisticNormalAtlas = 0;
					o.decalsDamageNormalAtlas = 0;

				#endif
				
			
				
				#if defined(_CONTRIBUTE_ALBEDO)
				
					float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, texCoords);
	
					#if defined(_CLIPPING)
				if (baseColor.a - UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Cutoff) < 0)
					{
					o.decalsArtisticAlbedoAtlas = 0;
					o.decalsDamageAlbedoAtlas = 0;
					return o;
					}
						// // #if !defined(_CONTRIBUTE_NORMAL)
						// 	clip(baseColor.a - UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Cutoff));
						// // #endif
					#endif
	
					baseColor *= UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial_DECALS, _BaseColor);
				#if !defined(_IS_DAMAGE_TYPE)
				
					o.decalsArtisticAlbedoAtlas = baseColor;
					o.decalsDamageAlbedoAtlas = 0;
				
				#else

					o.decalsArtisticAlbedoAtlas = 0;
					o.decalsDamageAlbedoAtlas = baseColor;

				#endif

				#endif
				o.decalsDamageNormalAtlas = 1;
				return o;
				
			}
			
			ENDHLSL
		}


	}

}