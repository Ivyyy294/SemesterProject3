#ifndef SHF_RAYMARCHING_UTILS
#define SHF_RAYMARCHING_UTILS

// Defaults

#define DEF_MAX_STEPS 128
#define DEF_MAX_DIST 64
#define DEF_SURF_DIST 0.001

// Shapes

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

// Raymarching

struct RaymarchResult
{
    float Distance;
    float3 Normal;
    float3 HitLocation;
    bool IsHit;
};

/**
 * \brief 
 * \param Function Scene Function for the SDF: takes one float3 argument
 * \param Result RaymarchResult: macro will fill this struct
 * \param RayOrigin float3: origin of the ray. Usually camera position
 * \param RayDirection float3: direction of the ray. Usually difference between camera position and geometry position
 * \param MAX_STEPS int: maximum amounts of steps. Use DEF_MAX_STEPS for default 128
 * \param MAX_DIST float: maximum distance before ray is considered to have missed. Use DEF_MAX_DIST for default 64
 * \param SURF_DIST float: small number representing the tolerance between ray hit and surface. Use DEF_SURF_DIST for default 0.001
 */
#define RAYMARCH(Function, Result, RayOrigin, RayDirection, MAX_STEPS, MAX_DIST, SURF_DIST) \
{ \
    float dist = 0.0; \
    float surfaceDistance; \
    for (int step = 0; step < MAX_STEPS; step++) \
    { \
        float3 p = RayOrigin + RayDirection * dist; \
        surfaceDistance = Function(p); \
        dist += surfaceDistance; \
        if (dist > MAX_DIST || surfaceDistance < SURF_DIST) break; \
    } \
    Result.Distance = dist; \
    Result.Normal = float3(0,0,0); \
    Result.HitLocation = RayOrigin + RayDirection * dist; \
    Result.IsHit = dist < MAX_DIST; \
    \
    if (Result.IsHit) \
    { \
        float3 p = Result.HitLocation; \
        float2 e = float2(0.01, 0.0); \
        float3 normal = Function(p) - float3( \
            Function(p-e.xyy), \
            Function(p-e.yxy), \
            Function(p-e.yyx)); \
        Result.Normal = normalize(normal); \
    } \
}

/**
 * \brief 
 * \param Function Scene Function for the SDF: takes one float3 argument
 * \param Result RaymarchResult: macro will fill this struct
 * \param RayOrigin float3: origin of the ray. Usually camera position
 * \param RayDirection float3: direction of the ray. Usually difference between camera position and geometry position
 */
#define RAYMARCH_EASY(Function, Result, RayOrigin, RayDirection) RAYMARCH(Function, Result, RayOrigin, RayDirection, DEF_MAX_STEPS, DEF_MAX_DIST, DEF_SURF_DIST)

#endif
