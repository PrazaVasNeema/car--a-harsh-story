#ifndef GBUFFER_PASS_INCLUDED
#define GBUFFER_PASS_INCLUDED

#include "Assets/DopeRP/GPU/HLSL/Common/Common.hlsl"
#include "Assets/DopeRP/GPU/HLSL/GI.hlsl"

UNITY_INSTANCING_BUFFER_START(LitBasePerMaterial)

    UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
    UNITY_DEFINE_INSTANCED_PROP(float4, _AlbedoMap_ST)

    UNITY_DEFINE_INSTANCED_PROP(float, _NormalScale)

    UNITY_DEFINE_INSTANCED_PROP(float, _Metallic)
    UNITY_DEFINE_INSTANCED_PROP(float, _Roughness)
    UNITY_DEFINE_INSTANCED_PROP(float, _Reflectance)

    UNITY_DEFINE_INSTANCED_PROP(float, _Cutoff)

UNITY_INSTANCING_BUFFER_END(LitBasePerMaterial)

TEXTURE2D(_AlbedoMap);
SAMPLER(sampler_AlbedoMap);

TEXTURE2D(_NormalMap);
SAMPLER(sampler_NormalMap);

CBUFFER_START(GBuffer)

    float _CameraNearPlane;
    float _CameraFarPlane;

CBUFFER_END

struct MeshData {
    float4 position : POSITION;
    float3 normal   : NORMAL;
    float4 tangentOS : TANGENT;

    float2 uv : TEXCOORD0;

    GI_ATTRIBUTE_DATA
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Interpolators {
    float4 position : SV_POSITION;
    float3 positionVS   : TEXCOORD0;
    float3 normalVS   : TEXCOORD1;
    float3 positionWS : TEXCOORD2;
    float4 tangentWS : VAR_TANGENT;
    float3 normalWS : TEXCOORD3;
    float4 positionCS: TEXCOORD4;

    float2 uv         : TEXCOORD5;

    GI_VARYINGS_DATA
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct fragOutput
{
    float4 tangentWorldSpace : SV_Target0;

    float4 albedo : SV_Target1;
    float4 normalWS : SV_Target2;
    float4 clearNormalWS : SV_Target3;
    float4 specular : SV_Target4;
    float4 BRDF : SV_Target5;
};

Interpolators vert(MeshData i)
{
    UNITY_SETUP_INSTANCE_ID(i);
    Interpolators o;
    UNITY_TRANSFER_INSTANCE_ID(i, o);
    TRANSFER_GI_DATA(input, output);
   
    o.position = TransformObjectToHClip(i.position);
    o.positionVS = TransformWorldToView(TransformObjectToWorld(i.position));
    o.normalVS = TransformWorldToViewNormal(TransformObjectToWorldNormal(i.normal));
    o.positionWS = TransformObjectToWorld(i.position);
    o.tangentWS = float4(TransformObjectToWorldDir(i.tangentOS.xyz), i.tangentOS.w);
    o.normalWS = TransformObjectToWorldNormal(i.normal);

    o.positionCS = TransformObjectToHClip(i.position);

    float4 baseMap_ST =  UNITY_ACCESS_INSTANCED_PROP(LitBasePerMaterial, _AlbedoMap_ST);
    o.uv = i.uv * baseMap_ST.xy + baseMap_ST.zw;
    return o;
}

float3 GetNormalTS (float2 baseUV) {
    float4 map = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, baseUV);
    float scale = UNITY_ACCESS_INSTANCED_PROP(LitBasePerMaterial, _NormalScale);
    float3 normal = DecodeNormal(map, scale);
	
    return normal;
}

fragOutput frag(Interpolators i) 
{
    UNITY_SETUP_INSTANCE_ID(i);

    
    fragOutput o;

    #if defined(_STENCIL_MASK)
    o.albedo = 0;
    #endif
    

    float4 baseColor = SAMPLE_TEXTURE2D(_AlbedoMap, sampler_AlbedoMap, i.uv);
    
    #if defined(_CLIPPING)

    clip(baseColor.a - UNITY_ACCESS_INSTANCED_PROP(LitBasePerMaterial, _Cutoff));

    #endif
    
    o.tangentWorldSpace = i.tangentWS;
    
    // o.normalViewSpace = mul(Inverse(GetViewToHClipMatrix()), i.position/i.position.w);

    // o.normalViewSpace = float4(i.position.xy-100,0,1);

    // o.normalViewSpace = i.position;

    // o.normalViewSpace = i.positionCS;

    // o.normalViewSpace = float4(i.normalVS, 1);
    
    // o.normalViewSpace = i.positionCS/i.positionCS.w;

    // o.normalViewSpace = mul(Inverse(GetViewToHClipMatrix()), i.positionCS/i.positionCS.w);


    // o.normalViewSpace = float4(mul(Inverse(GetViewToWorldMatrix()), float4(i.positionVS, 1)).xyz, 1);

    

    // o.positionViewSpace = float4(i.positionVS.xyz, 1);

    // o.positionViewSpace = float4(mul(GetViewToWorldMatrix(), float4(i.positionVS, 1)).xyz, 1);

    // o.positionViewSpace = float4(i.positionWS, 1);

    // o.positionViewSpace = float4(mul(UNITY_MATRIX_I_V, i.normalVS).xyz, 1);

    // o.positionViewSpace = float4(i.normalVS, 1);

    
    // o.positionViewSpace = float4(TransformViewToWorld(i.positionVS), 1);




    // float4 baseColor = SAMPLE_TEXTURE2D(_AlbedoMap, sampler_AlbedoMap, i.uv);
    baseColor *= UNITY_ACCESS_INSTANCED_PROP(LitBasePerMaterial, _BaseColor);
    
    o.albedo = baseColor;

    
    
    float3 normal = NormalTangentToWorld(GetNormalTS(i.uv), normalize(i.normalWS), normalize(i.tangentWS));
    o.normalWS = float4(normal, 1);

    // float3 worldup = float3(0,1,0);
    // float3 orthogonalVector = cross(i.normalWS, worldup);
    // float deltaDot = dot(orthogonalVector, i.tangentWS);
    // float angle = -acos(deltaDot);
    // float3 rotatedVector = orthogonalVector * cos(angle) + cross(i.normalWS, orthogonalVector) * sin(angle) + i.normalWS * dot(i.normalWS, orthogonalVector) * (1 - cos(angle));
    o.clearNormalWS = mul(Inverse(GetViewToHClipMatrix()), i.positionCS/i.positionCS.w);

    o.clearNormalWS = float4(i.normalWS, 1);

    o.normalWS = mul(Inverse(GetViewToHClipMatrix()), i.positionCS);

    o.normalWS = float4(i.positionCS.zzz/i.positionCS.w,1);

float depth = i.positionCS.z/i.positionCS.w;
    
    float sceneZ = CalcLinearZ(depth, _CameraNearPlane, _CameraFarPlane);

    o.normalWS = float4(sceneZ.xxx/ i.positionCS.w, 1);

    o.normalWS = float4((sceneZ * depth/sceneZ).xxx , 1);

    float4 clipSpacePosition = float4((i.positionCS.xy/i.positionCS.w) * sceneZ/depth, sceneZ, 1.0 * sceneZ/depth);

    clipSpacePosition = float4((i.positionCS.xy/i.positionCS.w) , depth, 1.0 );

    float4 view = mul(clipSpacePosition, Inverse(GetViewToHClipMatrix()));

    

    o.normalWS = clipSpacePosition;

    o.normalWS = float4((i.positionCS.xy/i.positionCS.w).xy, 0,1);

    o.normalWS = i.positionCS;

    // o.normalWS = float4(i.positionWS, 1);

    o.normalWS = float4(normal, 1);

    // o.normalWS = mul(Inverse(GetViewToHClipMatrix()), i.positionCS);

    o.normalWS = float4(i.positionVS, 1);
    o.normalWS = i.positionCS;
    o.normalWS = mul(Inverse(GetViewToHClipMatrix()), clipSpacePosition);

    
    o.normalWS = float4(i.positionCS.xy, 0,1);
    // o.normalWS = 1-depth;
    view = mul(Inverse(GetViewToHClipMatrix()), clipSpacePosition);
    o.normalWS =float4(i.positionVS, 1);;
    
    o.normalWS = float4(i.positionWS, 1);

    o.normalWS = float4(i.positionCS.xy/ i.positionCS.w, 0,1);

    float4 view2 = mul(Inverse(GetViewToHClipMatrix()), i.positionCS/i.positionCS.w);

    o.normalWS = view2/view2.w;

    float4 world2 = mul(Inverse(GetWorldToViewMatrix()), view2);

    // o.normalWS = world2;
    
    // o.normalWS = float4(i.positionVS, 1);
    // o.clearNormalWS = float4(rotatedVector,1);

    // o.normalWS = float4(i.positionCS.xy/i.positionCS.w, 0,1);

    o.normalWS = (i.positionCS/i.positionCS.w).zzzz;

    o.normalWS = float4(i.positionVS, 1);

    o.normalWS = float4(normal, 1);

    // o.normalWS = float4(i.positionCS/i.positionCS.w);
    
    float metallic = UNITY_ACCESS_INSTANCED_PROP(LitBasePerMaterial, _Metallic);
    float roughness = perceptualRoughnessToRoughness(UNITY_ACCESS_INSTANCED_PROP(LitBasePerMaterial, _Roughness));
    float reflectance = UNITY_ACCESS_INSTANCED_PROP(LitBasePerMaterial, _Reflectance);

    o.BRDF = float4(metallic, roughness, reflectance, 1);

    float3 viewDir = normalize(_WorldSpaceCameraPos - i.positionWS);
    float3 specular = SampleEnvironment(viewDir, i.normalWS);



    o.specular = float4(specular, 1);
    
    return o;
    
}

#endif