#ifndef CUSTOM_SURFACE_DATA_INCLUDED
#define CUSTOM_SURFACE_DATA_INCLUDED

struct SurfaceData {
	float3 normal;
	float3 viewDirection;
	float3 color;
	float alpha;
	float metallic;
	float roughness;
	float f0;
};

#endif