#ifndef RAYMARCHING_UTILS
#define RAYMARCHING_UTILS

float sdSphere(float3 p, float3 sp, float r)
{
    return length(sp - p) - r;
}

// Boolean Operations

float sdUnion(float a, float b)
{
    return min(a, b);
}

float sdUnionSmooth(float a, float b, float k)
{
    float h = clamp(0.5 + 0.5*(b-a)/k, 0, 1);
    return lerp(b, a, h) - k*h*(1-h);
}

float sdSubtract(float a, float b)
{
    return max(-a, b);
}

float sdSubtractSmooth(float a, float b, float k)
{
    float h = clamp(0.5 - 0.5*(b+a)/k, 0, 1);
    return lerp(b, -a, h) + k*h*(1-h);
}

float sdIntersect(float a, float b)
{
    return max(a, b);
}

float sdIntersectSmooth(float a, float b, float k)
{
    float h = clamp(0.5 - 0.5*(b-a)/k, 0, 1);
    return lerp(b, a, h) + k*h*(1-h);
}


#endif