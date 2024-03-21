#ifndef COMMON_MATERIAL_INCLUDED
#define COMMON_MATERIAL_INCLUDED

#define MIN_N_DOT_V 1e-4

float clampNoV(float NoV) {
    // Neubelt and Pettineo 2013, "Crafting a Next-gen Material Pipeline for The Order: 1886"
    return max(NoV, MIN_N_DOT_V);
}

float3 computeDiffuseColor(const float3 baseColor, float metallic) {
    return baseColor.rgb * (1.0 - metallic);
}

float perceptualRoughnessToRoughness(float perceptualRoughness) {
    return perceptualRoughness * perceptualRoughness;
}

float3 computeReflectance(const float4 baseColor, float metallic, float reflectance) {
    return 0.16 * reflectance * reflectance * (1.0 - metallic) + baseColor * metallic;
}

#endif