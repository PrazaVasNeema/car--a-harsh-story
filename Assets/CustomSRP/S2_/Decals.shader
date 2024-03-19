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
		
				[NoScaleOffset] _NormalMap("Normals", 2D) = "bump" {}
		_NormalScale("Normal Scale", Range(0, 1)) = 1

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

			TEXTURE2D(_NormalMap);
			
			TEXTURE2D(_PositionViewSpace);
			SAMPLER(sampler_PositionViewSpace);

			TEXTURE2D(_NormalViewSpace);
			SAMPLER(sampler_NormalViewSpace);

			TEXTURE2D(_TangentViewSpace);
			SAMPLER(sampler_TangentViewSpace);
			
			CBUFFER_START(GBuffer)
			
			    float _CameraNearPlane;
			    float _CameraFarPlane;

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
			    float4 decalsAtlas : SV_Target0;
			    float4 decalsNormalAtlas : SV_Target1;
			};

			float3 GetNormalTS (float2 baseUV) {
				float4 map = SAMPLE_TEXTURE2D(_NormalMap, sampler_BaseMap, baseUV);
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

				float3 normal = normalize(SAMPLE_TEXTURE2D(_NormalViewSpace, sampler_NormalViewSpace, i.uv).xyz);
				float3 tangent = normalize(ddy(worldPos));
				tangent = TransformWorldToViewDir(i.tangentWS);
				// normal = TransformViewToWorldNormal(normal);
				// tangent = TransformViewToWorldNormal(tangent);
				float3 binormal = cross(normal,tangent);
				float3x3 tbn = float3x3(tangent, binormal, normal);

				float3 normalLTS =  GetNormalTS(textureCoords);

				binormal = normalize(ddx(worldPos));
				tangent = normalize(ddy(worldPos));
				
				normal = normalize(cross(tangent, binormal));

				tbn = float3x3(tangent, binormal, normal);

				normal = normalize(SAMPLE_TEXTURE2D(_NormalViewSpace, sampler_NormalViewSpace, i.positionSV / _ScreenSize).xyz);
				// normal = TransformViewToWorldNormal(normal);
				normal = float4(mul(UNITY_MATRIX_I_V, normal));
				
				float4 tangent2 = normalize(SAMPLE_TEXTURE2D(_TangentViewSpace, sampler_TangentViewSpace, i.positionSV / _ScreenSize));

				o.decalsNormalAtlas = float4(NormalTangentToWorld(GetNormalTS(textureCoords), normal, tangent2),1);

				// o.decalsNormalAtlas = float4(normal,1);

				// o.decalsNormalAtlas = float4(GetNormalTS(texCoord), 1);
				// o.decalsNormalAtlas = SAMPLE_TEXTURE2D(_NormalViewSpace, sampler_NormalViewSpace, i.uv);
				// o.decalsNormalAtlas = float4(mul(tbn, normalLTS), 1);
				// o.decalsNormalAtlas = SAMPLE_TEXTURE2D(_NormalMap, sampler_BaseMap, texCoord);;
				// float4 map = SAMPLE_TEXTURE2D(_NormalMap, sampler_BaseMap, texCoord);
				// o.decalsNormalAtlas = float4(map.xyz, 1);

				// o.decalsNormalAtlas = float4(mul(normal, UNITY_MATRIX_I_V));
				
				#if defined(_CLIPPING)
					clip(baseColor.a - UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Cutoff));
				#endif

				baseColor *= UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial_DECALS, _BaseColor);
				o.decalsAtlas = baseColor;



				return o;

				//Get values across and along the surface
				float3 ddxWp = ddx(worldPos);
				float3 ddyWp = ddy(worldPos);

				//Determine the normal
				normal = normalize(cross(ddyWp, ddxWp));

				//Normalizing things is cool
				binormal = normalize(ddxWp);
				tangent = normalize(ddyWp);

				float3x3 tangentToView;
				tangentToView[0] = TransformWorldToViewNormal(tangent);
				tangentToView[1] = TransformWorldToViewNormal(tangent);
				tangentToView[2] = TransformWorldToViewNormal(tangent);

				// normal = TransformWorldToViewNormal(CreateTangentToWorld(normal, tangent, ));
				
				return o;
			}
			
			
			ENDHLSL
		}


	}
}