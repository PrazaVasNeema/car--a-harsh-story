#ifndef SURFACE_DATA_INCLUDED
#define SURFACE_DATA_INCLUDED

struct SurfaceData {
	float3 normal;
	float3 viewDirection;
	float3 color;
	float3 positionWS;
	float alpha;
	float metallic;
	float roughness;
	float f0;
	float depth;
};

#endif