#ifndef COMMON_MATH_INCLUDED
#define COMMON_MATH_INCLUDED

#define PI 3.14

/**
 * Computes x^5 using only multiply operations.
 *
 * @public-api
 */
float pow5(float x) {
	float x2 = x * x;
	return x2 * x2 * x;
}

// Logical operations

float4 when_eq(float4 x, float4 y) {
	return 1.0 - abs(sign(x - y));
}

float4 when_neq(float4 x, float4 y) {
	return abs(sign(x - y));
}

float4 when_gt(float4 x, float4 y) {
	return max(sign(x - y), 0.0);
}

float4 when_lt(float4 x, float4 y) {
	return max(sign(y - x), 0.0);
}

float4 when_ge(float4 x, float4 y) {
	return 1.0 - when_lt(x, y);
}

float4 when_le(float4 x, float4 y) {
	return 1.0 - when_gt(x, y);
}

// For usage with outputs from the comparisons above

float4 and(float4 a, float4 b) {
	return a * b;
}

float4 or(float4 a, float4 b) {
	return min(a + b, 1.0);
}

float4 xor(float4 a, float4 b) {
	return (a + b) % 2.0;
}

float4 not(float4 a) {
	return 1.0 - a;
}


#endif