#ifndef SSR_RAW_PASS_INCLUDED
#define SSR_RAW_PASS_INCLUDED

#include "Assets/DopeRP/GPU/HLSL/Common/Common.hlsl"

#define ITER_COUNT 2

TEXTURE2D(_G_NormalWorldSpaceAtlas);
SAMPLER(sampler_G_NormalWorldSpaceAtlas);

TEXTURE2D(_GAux_WorldSpaceAtlas);
SAMPLER(sampler_GAux_WorldSpaceAtlas);

TEXTURE2D(_DepthBuffer);
SAMPLER(sampler_DepthBuffer);

TEXTURE2D(_G_SpecularAtlas);
SAMPLER(sampler_G_SpecularAtlas);

TEXTURE2D(_G_BRDFAtlas);
SAMPLER(sampler_G_BRDFAtlas);


CBUFFER_START(SSRRaw)

    float4 _ScreenSize;
float2 _NearFarPlanes;
float4x4 _Matrix_V;
    float4x4 _Matrix_P;
float4x4 _Matrix_I_P;


CBUFFER_END


struct MeshData
{
    float4 position : POSITION;
    float2 uv : TEXCOORD0;
};

struct Interpolators
{
    float4 positionSV : SV_POSITION;
    float2 uv : TEXCOORD0;
};


Interpolators vert (MeshData i)
{
    Interpolators o;
    o.uv = i.uv;
    o.positionSV = TransformObjectToHClip(i.position);
    return o;
}

float Mix(float x, float y, float a)
{
    return x * (1-a) + y * a;
}

float4 frag (Interpolators i) : SV_Target
{
  
    float4 fragColor = 0;

    float maxDistance = 15;
    float resolution  = 0.00003;
    int   steps       = 10;
    float thickness   = 0.5;

    float2 texSize  = _ScreenSize.xy;
    float2 texCoord = i.positionSV.xy / texSize;

    // return float4(texCoord,0,1);

    float4 positionFrom     = SAMPLE_TEXTURE2D(_GAux_WorldSpaceAtlas, sampler_GAux_WorldSpaceAtlas, i.uv);
    positionFrom = mul(_Matrix_V, positionFrom);
    // return  positionFrom;
    // return float4(i.uv, 0, 1);
    
    float3 unitPositionFrom = normalize(positionFrom.xyz);
    float4 normalWS = SAMPLE_TEXTURE2D(_G_NormalWorldSpaceAtlas, sampler_G_NormalWorldSpaceAtlas, i.uv);
    float4 normalVS = float4(mul((real3x3)_Matrix_V, normalWS).xyz, 1);
    // return normalVS;
    float3 normal           = normalize(normalVS.xyz);
    // return float4(normal,1);
    float3 pivot            = normalize(reflect(unitPositionFrom, normal));

    float4 startView = float4(positionFrom.xyz + (pivot *           0), 1);
    float4 endView   = float4(positionFrom.xyz + (pivot * maxDistance), 1);

    float4 startFrag      = startView;
    // Project to screen space.
    startFrag      = mul(_Matrix_P, startFrag);
    // Perform the perspective divide.
    startFrag.xyz /= startFrag.w;
    // Convert the screen-space XY coordinates to UV coordinates.
    startFrag.xy   = startFrag.xy * 0.5 + 0.5;
    // Convert the UV coordinates to fragment/pixel coordnates.
    startFrag.xy  *= texSize;

    float4 endFrag      = endView;
    endFrag      = mul(_Matrix_P, endFrag);
    endFrag.xyz /= endFrag.w;
    endFrag.xy   = endFrag.xy * 0.5 + 0.5;
    endFrag.xy  *= texSize;

    float4 uv = 0;
    float2 frag  = startFrag.xy;
    uv.xy = frag / texSize;

    float deltaX    = endFrag.x - startFrag.x;
    float deltaY    = endFrag.y - startFrag.y;

    float useX      = abs(deltaX) >= abs(deltaY) ? 1 : 0;
    float delta     = Mix(abs(deltaY), abs(deltaX), useX) * clamp(resolution, 0.0, 1.0);

    float2  increment = float2(deltaX, deltaY) / max(delta, 0.001);

    float search0 = 0;
    float search1 = 0;

    int hit0 = 0;
    int hit1 = 0;

    float viewDistance = startView.y;
    float depth        = thickness;

    float4 positionTo = positionFrom;
    return (int)delta;
    // return abs(deltaY);
    for (int j = 0; j < delta; j++)
    {
        frag      += increment;
        uv.xy      = frag / texSize;
        positionTo = mul(_Matrix_V, SAMPLE_TEXTURE2D(_GAux_WorldSpaceAtlas, sampler_GAux_WorldSpaceAtlas, uv.xy));

        search1 =
      Mix
        ( (frag.y - startFrag.y) / deltaY
        , (frag.x - startFrag.x) / deltaX
        , useX
        );

        viewDistance = (startView.y * endView.y) / Mix(endView.y, startView.y, search1);

        // // Incorrect.
        // viewDistance = mix(startView.y, endView.y, search1);

        // Correct.
        viewDistance = (startView.y * endView.y) / Mix(endView.y, startView.y, search1);

        depth        = viewDistance - positionTo.y;

        if (depth > 0 && depth < thickness) {
            hit0 = 1;
            break;
        } else {
            search0 = search1;
        }

    }
    search1 = search0 + ((search1 - search0) / 2);

    return depth;
    
 //    float maxDistance = 8;
 //    float resolution  = 0.03;
 //    int   steps       = 5;
 //    float thickness   = 0.5;
 //  
 //    // float2 texSize  = textureSize(positionTexture, 0).xy;
 //    float2 texCoord = i.uv;
 //  
 //    float4 uv = 0;
 //
 // float4 positionWS = SAMPLE_TEXTURE2D(_GAux_WorldSpaceAtlas, sampler_GAux_WorldSpaceAtlas, texCoord);
 //
 //  float4 fragPositionVS = mul(_Matrix_V, positionWS);
 //
 //  
 //    float4 positionFrom = fragPositionVS;
 //    float4 mask         = SAMPLE_TEXTURE2D(_G_BRDFAtlas, sampler_G_BRDFAtlas, texCoord);
 //  
 //    if (  positionFrom.w <= 0.0
 //       // || enabled.x      != 1.0
 //       // || mask.r         <= 0.0
 //       ) { fragColor = uv; return fragColor; }
 //  
 //    float3 unitPositionFrom = normalize(positionFrom.xyz);
 //    float4 normalWS = SAMPLE_TEXTURE2D(_G_NormalWorldSpaceAtlas, sampler_G_NormalWorldSpaceAtlas, i.uv);
 //    float3 normal           = normalize(mul((real3x3)_Matrix_V, normalWS).xyz);
 //    float3 pivot            = normalize(reflect(unitPositionFrom, normal));
 //  
 //    float4 positionTo = positionFrom;
 //  
 //    float4 startView = float4(positionFrom.xyz + (pivot *         0.0), 1.0);
 //    float4 endView   = float4(positionFrom.xyz + (pivot * maxDistance), 1.0);
 //  
 //    float4 startFrag      = startView;
 //         startFrag      = mul(_Matrix_P, startFrag);
 //         startFrag.xyz /= startFrag.w;
 //         startFrag.xy   = startFrag.xy * 0.5 + 0.5;
 //         startFrag.xy  *= _ScreenSize.xy;
 //  
 //    float4 endFrag      = endView;
 //         endFrag      = mul(_Matrix_P, endFrag);
 //         endFrag.xyz /= endFrag.w;
 //         endFrag.xy   = endFrag.xy * 0.5 + 0.5;
 //         endFrag.xy  *= _ScreenSize.xy;
 //  
 //    float2 frag  = startFrag.xy;
 //         uv.xy = frag / _ScreenSize.xy;
 //  
 //    float deltaX    = endFrag.x - startFrag.x;
 //    float deltaY    = endFrag.y - startFrag.y;
 //    float useX      = abs(deltaX) >= abs(deltaY) ? 1.0 : 0.0;
 //    float delta     = lerp(abs(deltaY), abs(deltaX), useX) * clamp(resolution, 0.0, 1.0);
 //    float2  increment = float2(deltaX, deltaY) / max(delta, 0.001);
 //  
 //    float search0 = 0;
 //    float search1 = 0;
 //  
 //    int hit0 = 0;
 //    int hit1 = 0;
 //  
 //    float viewDistance = startView.y;
 //    float depth        = thickness;
 //  
 //    // float iter = 0; 
 //
 //    float iterationsMax = min((int)delta, 100);
 //    for (int j = 0; j < (int)delta; ++j) {
 //      frag      += increment;
 //      uv.xy      = frag / _ScreenSize.xy;
 //      positionTo = mul(_Matrix_V, SAMPLE_TEXTURE2D(_GAux_WorldSpaceAtlas, sampler_GAux_WorldSpaceAtlas, uv.xy));
 //  
 //      search1 =
 //        lerp
 //          ( (frag.y - startFrag.y) / deltaY
 //          , (frag.x - startFrag.x) / deltaX
 //          , useX
 //          );
 //  
 //      search1 = clamp(search1, 0.0, 1.0);
 //  
 //      viewDistance = (startView.y * endView.y) / lerp(endView.y, startView.y, search1);
 //      depth        = viewDistance - positionTo.y;
 //  
 //      if (depth > 0 && depth < thickness) {
 //        hit0 = 1;
 //        break;
 //      } else {
 //        search0 = search1;
 //      }
 //    }
 //  
 //    search1 = search0 + ((search1 - search0) / 2.0);
 //  
 //    steps *= hit0;
 //  
 //    for (int j = 0; j < steps; ++j) {
 //      frag       = lerp(startFrag.xy, endFrag.xy, search1);
 //      uv.xy      = frag / _ScreenSize.xy;
 //      positionTo = mul(_Matrix_V, SAMPLE_TEXTURE2D(_GAux_WorldSpaceAtlas, sampler_GAux_WorldSpaceAtlas, uv.xy));
 //  
 //      viewDistance = (startView.y * endView.y) / lerp(endView.y, startView.y, search1);
 //      depth        = viewDistance - positionTo.y;
 //  
 //      if (depth > 0 && depth < thickness) {
 //        hit1 = 1;
 //        search1 = search0 + ((search1 - search0) / 2);
 //      } else {
 //        float temp = search1;
 //        search1 = search1 + ((search1 - search0) / 2);
 //        search0 = temp;
 //      }
 //    }
 //  
 //    float visibility =
 //        hit1
 //      * positionTo.w
 //      * ( 1
 //        - max
 //           ( dot(-unitPositionFrom, pivot)
 //           , 0
 //           )
 //        )
 //      * ( 1
 //        - clamp
 //            ( depth / thickness
 //            , 0
 //            , 1
 //            )
 //        )
 //      * ( 1
 //        - clamp
 //            (   length(positionTo - positionFrom)
 //              / maxDistance
 //            , 0
 //            , 1
 //            )
 //        )
 //      * (uv.x < 0 || uv.x > 1 ? 0 : 1)
 //      * (uv.y < 0 || uv.y > 1 ? 0 : 1);
 //  
 //    visibility = clamp(visibility, 0, 1);
 //  
 //    uv.ba = visibility;
 //  
 //    fragColor = uv;
 //
 //    return fragColor;
    
}

#endif