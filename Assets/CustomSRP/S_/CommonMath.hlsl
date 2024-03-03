#ifndef COMMON_MATH_INCLUDED
#define COMMON_MATH_INCLUDED

/**
 * Computes x^5 using only multiply operations.
 *
 * @public-api
 */
float pow5(float x) {
	float x2 = x * x;
	return x2 * x2 * x;
}

#endif