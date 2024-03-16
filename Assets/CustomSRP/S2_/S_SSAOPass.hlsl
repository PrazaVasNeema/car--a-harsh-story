#ifndef GEOMETRY_PASS_INCLUDED
#define GEOMETRY_PASS_INCLUDED




struct MeshData {
    float3 positionOS : POSITION;
    float2 uv         : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Interpolators {
    float4 positionCS : SV_POSITION;
    float2 uv         : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct FragmentOutput {
    float4 albedo : SV_Target0;
    float4 position : SV_Target1;
};


Interpolators vert(MeshData i)
{
    UNITY_SETUP_INSTANCE_ID(i);
    Interpolators o;
    UNITY_TRANSFER_INSTANCE_ID(i, o);
    o.positionCS = TransformWorldToHClip(TransformObjectToWorld(i.positionOS.xyz));
    o.uv = i.uv;
	
	
    return o;
}

FragmentOutput frag(Interpolators i) 
{
    UNITY_SETUP_INSTANCE_ID(i);

    FragmentOutput output;
    // return float4(1,1,0,1);
    // return float4(i.uv.xxx,1);
    output.albedo = float4(0.95, 0.95, 0.95, 1);
    output.position = float4(1, 0, 0, 1);
    // return float4(CalculateSSAO(i.uv).xyz,1);

    return output;
    
}

#endif