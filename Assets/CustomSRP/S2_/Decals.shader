Shader "CustomSRP/Decals"
{
	Properties
	{
		_BaseMap("DecalTexture", 2D) = "white" {}
		_BaseColor("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Metallic ("Metallic", Range(0, 1)) = 0
		_Roughness ("Roughness", Range(0, 1)) = 0.5
		_Reflectance ("Reflectance", Range(0, 1)) = 0.5

		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0
		
		[Enum(Off, 0, On, 1)] _WorkingVar ("check????", Float) = 1
		
		[Toggle(_PREMULTIPLY_ALPHA)] _PremulAlpha ("Premultiply Alpha", Float) = 0
				[Toggle(_RECEIVE_SHADOWS)] _ReceiveShadows ("Receive Shadows", Float) = 1
		
_DepthLevel ("Depth Level", Range(1, 3)) = 1
		
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

			HLSLPROGRAM
			#pragma target 3.5
			#pragma multi_compile_instancing
			#pragma enable_d3d11_debug_symbols
			#pragma shader_feature _CLIPPING

			#pragma vertex vert
			#pragma fragment frag
			// #include "Assets/CustomSRP/S2_/GBUFFERPass.hlsl"

			#include "../ShaderLibrary/Common.hlsl"

			TEXTURE2D(_BaseMap);
			SAMPLER(sampler_BaseMap);
			
			TEXTURE2D(_PositionViewSpace);
			SAMPLER(sampler_PositionViewSpace);
			
			CBUFFER_START(GBuffer)
			
			    float _CameraNearPlane;
			    float _CameraFarPlane;

				float4 _ScreenSize;
			
			CBUFFER_END

			UNITY_INSTANCING_BUFFER_START(UnityPerMaterial_DECALS)
				UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
	UNITY_DEFINE_INSTANCED_PROP(float, _Cutoff)


				

			UNITY_INSTANCING_BUFFER_END(UnityPerMaterial_DECALS)

			struct MeshData {
				float4 position : POSITION;
			    float3 normal   : NORMAL;
				float2 uv   : TEXCOORD0;
			    UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Interpolators {
			    float4 positionSV : SV_POSITION;
			    float3 positionVS   : TEXCOORD0;
			    float3 normalVS   : TEXCOORD1;
				float3 positionWS : TEXCOORD2;
				float2 uv   : TEXCOORD3;
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
			    return o;
			}

			float4 frag(Interpolators i) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(i);

				// return float4(baseColor.xyz,1);
				float2 screenPos = i.positionSV.xy / i.positionSV.w;

				float2 texCoord = float2((1+screenPos.x)/2 + (0.5 * _ScreenSize.z),
					(1+screenPos.y)/2 + (0.5 * _ScreenSize.w));

				float3 a = i.positionSV / _ScreenSize;
				// return float4(a.xy, 0, 1);
				float4 sampleDepth = SAMPLE_TEXTURE2D(_PositionViewSpace, sampler_PositionViewSpace, i.positionSV / _ScreenSize);
				// return sampleDepth;
				// float3 viewRay = i.positionVS.xyz * (_CameraFarPlane /  i.positionVS.z);

				// float3 viewPosition = viewRay * sampleDepth.z;

				// return float4(sampleDepth.x, i.positionVS.x, 0, 1);
				
				// return float4(sampleDepth.xyz, 1);

				// return float4(i.positionVS, 1);

				float4 worldPos = mul(UNITY_MATRIX_I_V, float4(sampleDepth));
				worldPos = mul(i.positionVS, UNITY_MATRIX_I_V);
				worldPos = mul(sampleDepth, UNITY_MATRIX_I_V);

				worldPos = float4(TransformViewToWorld(i.positionVS),1);
				worldPos = float4(TransformViewToWorld(sampleDepth.xyz),1);

				// return float4(worldPos.xyz, 1);

				// return float4(i.positionWS, 1);
				
				float4 objectPos = float4(TransformWorldToObject(worldPos), 1);
				
				clip(0.5 - abs(objectPos.xyz));
				float2 textureCoords = objectPos.xy + 0.5;

				float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, textureCoords);

				#if defined(_CLIPPING)
					clip(baseColor.a - UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Cutoff));
				#endif

				baseColor += UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial_DECALS, _BaseColor);
				return baseColor;
			}
			
			
			ENDHLSL
		}


	}
}