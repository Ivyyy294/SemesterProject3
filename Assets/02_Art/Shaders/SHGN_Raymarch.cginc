#ifndef SHGN_RAYMARCH_INCLUDED
#define SHGN_RAYMARCH_INCLUDED

#include "SHF_RaymarchingUtils.cginc"

float TestGetDist(float3 ro)
{
    float s1 = sdSphere(ro, float3(0,-.3,0), 0.2);
    float y = sin(_Time.y * 3) * 0.15;
    float s2 = sdSphere(ro, float3(0, y + 0.1, 0), 0.15);
    return sdUnionSmooth(s2, s1, 0.05);
} 

void RaymarchTest_float(float3 RayOrigin, float3 RayDirection, out float3 Position, out float3 Normal, out bool IsHit)
{
    RaymarchResult result;
    RAYMARCH(TestGetDist, result, RayOrigin, RayDirection, DEF_MAX_STEPS, DEF_MAX_DIST, DEF_SURF_DIST);
    Position = result.HitLocation;
    Normal = result.Normal;
    IsHit = result.IsHit;
}

#endif
