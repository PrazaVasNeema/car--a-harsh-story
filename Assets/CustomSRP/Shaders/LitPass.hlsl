#ifndef CUSTOM_LIT_PASS_INCLUDED
#define CUSTOM_LIT_PASS_INCLUDED

#include "../ShaderLibrary/Common.hlsl"
#include "../ShaderLibrary/Surface.hlsl"
#include "../ShaderLibrary/Light.hlsl"
#include "../ShaderLibrary/BRDF.hlsl"
#include "../ShaderLibrary/Lighting.hlsl"

TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);

CBUFFER_START(UnityPerMaterial)
	float4 _BaseColor;
	float4 _BaseMap_ST; //texture scale and transform params
	float _Metalness;
	float _Roughness;
CBUFFER_END

//uniform float4x4 _lightSpaceMatrix;
uniform float4x4 _lightProjection;
uniform float3 _lightDir;
// uniform Texture2D _DirectionalShadowAtlas;

// TEXTURE2D(_DirectionalShadowAtlas);
// TEXTURE2D_SHADOW(_DirectionalShadowAtlas);
// SAMPLER(sampler_DirectionalShadowAtlas);


// TEXTURE2D_SHADOW(_DirectionalShadowAtlas);
// #define SHADOW_SAMPLER sampler_linear_clamp_compare
// SAMPLER_CMP(SHADOW_SAMPLER);
sampler2D _DirectionalShadowAtlas;

float2 _DirectionalShadowAtlas_TexelSize;

// sampler2D _DirectionalShadowAtlasSampler;


struct VertexAttributes {
	float3 positionOS : POSITION;
	float3 normalOS   : NORMAL;
	float2 uv         : TEXCOORD0;
};

struct Varyings {
	float4 positionCS : SV_POSITION;
	float3 positionWS : VAR_POSITION;
	float3 normalWS   : VAR_NORMAL;
	float2 uv         : TEXCOORD0;
	float4 fragPosLight : TEXCOORD1;
};

Varyings Vertex(VertexAttributes vertexInput)
{
	Varyings vertexOut;
	vertexOut.positionWS = TransformObjectToWorld(vertexInput.positionOS.xyz);
	vertexOut.positionCS = TransformWorldToHClip(vertexOut.positionWS);
	vertexOut.normalWS = TransformObjectToWorldNormal(vertexInput.normalOS);
	vertexOut.uv = vertexInput.uv * _BaseMap_ST.xy + _BaseMap_ST.zw;
	
	vertexOut.fragPosLight = mul(_lightProjection,float4(vertexOut.positionWS, 1.0f));
	return vertexOut;
}

// float ShadowCalculation(float4 fragPosLightSpace)
// {
// 	float3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
//
// 	projCoords = projCoords * 0.5 + 0.5;
// 	// get closest depth value from light's perspective (using [0,1] range fragPosLight as coords)
// 	float closestDepth = tex2D(_DirectionalShadowAtlas, projCoords.xy).r; 
// 	// get depth of current fragment from light's perspective
// 	float currentDepth = projCoords.z;
// 	// check whether current frag pos is in shadow
// 	float shadow = currentDepth > closestDepth  ? 1.0 : 0.0;
//
// 	return shadow;
// }

// float SampleDirectionalShadowAtlas (float3 positionSTS) {
// 	return SAMPLE_TEXTURE2D_SHADOW(_DirectionalShadowAtlas, SHADOW_SAMPLER, positionSTS);
// }

float checkShadow(float mapDepth, float curDepth)
{
	return curDepth > mapDepth ? 1 : 0;
}

float4 Fragment(Varyings fragmentInput) : SV_TARGET
{
	float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, fragmentInput.uv);
	baseColor *= _BaseColor;

	Surface surface;
	surface.normal = normalize(fragmentInput.normalWS);
	surface.viewDirection = normalize(_WorldSpaceCameraPos - fragmentInput.positionWS);
	surface.color = baseColor.rgb;
	surface.alpha = baseColor.a;
	surface.metalness = _Metalness;
	surface.roughness = _Roughness;

	BRDF brdf = GetBRDF(surface);
	float3 color = GetLighting(surface, brdf);

	float shadow1 = 0.0f;
	float3 lightCoords = fragmentInput.fragPosLight.xyz ;

	//shadow1 = SampleDirectionalShadowAtlas(lightCoords);
	lightCoords = lightCoords * 0.5 + 0.5;

	// lightCoords = saturate(lightCoords);

		// return float4(tex2D(_DirectionalShadowAtlas, lightCoords.xy).r,0,0,1);
	
		//lightCoords = (lightCoords + 1.0f) / 2.0f;
		// lightCoords = lightCoords * 0.5f + 0.5f;

		float sum = 0;

		float2 texelSize = float2(_DirectionalShadowAtlas_TexelSize.x, _DirectionalShadowAtlas_TexelSize.y);

		// float3 lightDir = normalize(_lightPos - fragmentInput.positionWS);
	
		float currentDepth = lightCoords.z - max(0.01 * (1.0 - dot(fragmentInput.normalWS, -_lightDir)), 0.001);
		sum += checkShadow(tex2D(_DirectionalShadowAtlas, lightCoords.xy + float2(1,1) * texelSize).r, currentDepth);
		sum += checkShadow(tex2D(_DirectionalShadowAtlas, lightCoords.xy + float2(-1,-1) * texelSize ).r, currentDepth);
		sum += checkShadow(tex2D(_DirectionalShadowAtlas, lightCoords.xy + float2(1,-1) * texelSize).r, currentDepth);
		sum += checkShadow(tex2D(_DirectionalShadowAtlas, lightCoords.xy + float2(-1,1) * texelSize).r, currentDepth);
		// shadow1 = checkShadow(tex2D(_DirectionalShadowAtlas, lightCoords.xy).r, currentDepth);

	shadow1 = sum * 0.25;
		// return float4(dot(fragmentInput.normalWS, -_lightDir),0,0,1);
	// return float4(fragmentInput.normalWS,1);

	// return float4(_lightDir,1);
// if (lightCoords.y > 1)
// 	return float4(1,0,0,1);
		// float closestDepth = tex2D(_DirectionalShadowAtlas, lightCoords.xy).r;
		// float currentDepth = lightCoords.z - 0.001;
		//
		//
		//
		//  if (currentDepth > closestDepth)
		//  {
		//  	shadow1 = 1.0f;
		//  }

	// if (lightCoords.z >= 1 || lightCoords.x >1 || lightCoords.x <0 || lightCoords.y >1 || lightCoords.y <0)
	if (lightCoords.z >= 1 || lightCoords.x >1 || lightCoords.x <0 || lightCoords.y >1 || lightCoords.y <0)
		shadow1 = 0;
	// lightCoords = fragmentInput.fragPosLight.xyz ;
	// lightCoords = (lightCoords + 1.0f) / 2.0f;
	//
	// return float4(lightCoords.x, lightCoords.y,0,1);
	
	//float shadow = ShadowCalculation(fragmentInput.fragPosLightSpace);
	
	return float4(color * (1.0f - shadow1), surface.alpha);
}

#endif