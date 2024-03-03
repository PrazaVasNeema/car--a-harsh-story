#ifndef CUSTOM_BRDF_INCLUDED
#define CUSTOM_BRDF_INCLUDED

#define PI 3.14

#include "SurfaceData.hlsl"
#include "CommonMath.hlsl"

float F_Schlick(float f0, float f90, float VoH) {
	return f0 + (f90 - f0) * pow5(1.0 - VoH);
}

//------------------------------------------------------------------------------
// Diffuse BRDF implementations
//---

float Fd_Lambert() {
	return 1.0 / PI;
}

float Fd_Burley(float roughness, float NoV, float NoL, float LoH) {
	// Burley 2012, "Physically-Based Shading at Disney"
	float f90 = 0.5 + 2.0 * roughness * LoH * LoH;
	float lightScatter = F_Schlick(1.0, f90, NoL);
	float viewScatter  = F_Schlick(1.0, f90, NoV);
	return lightScatter * viewScatter * (1.0 / PI);
}

//------------------------------------------------------------------------------
// Diffuse BRDF dispatch
//------------------------------------------------------------------------------

float diffuse(float roughness, float NoV, float NoL, float LoH) {
	return Fd_Burley(roughness, NoV, NoL, LoH);
}

#endif