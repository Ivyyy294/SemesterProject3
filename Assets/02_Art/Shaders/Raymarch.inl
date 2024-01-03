
// Should Define:
//  MAX_DIST
//  MAX_STEPS
//  SURF_DIST
//  USE_CALC_NORMAL
//  float SDFSceneFunction
//  RaymarchInput raymarchInput
//  RaymarchResult raymarchResult

float3 ro = raymarchInput.RayOrigin;
float3 rd = raymarchInput.RayDirection;

raymarchResult.IsHit = true;

float d = 0.0;
float dS;
for (int index = 0; index < 100; index++)
{
    float3 p = ro + rd * d;
    dS = SDFSceneFunction(p);
    d += dS;
    if (d>128)
    {
        raymarchResult.IsHit = false;
    }
    if (dS<0.01)
    {
        break;
    }
}
raymarchResult.Distance = d;
raymarchResult.HitLocation = raymarchInput.RayOrigin + d * raymarchInput.RayDirection;
raymarchResult.Normal = float3(0,0,0);

// Calculate normals

d = SDFSceneFunction(raymarchResult.HitLocation);
float2 e = float2(0.01, 0.0);
float3 n = d - float3(
    SDFSceneFunction(raymarchResult.HitLocation-e.xyy),
    SDFSceneFunction(raymarchResult.HitLocation-e.yxy),
    SDFSceneFunction(raymarchResult.HitLocation-e.yyx));
raymarchResult.Normal = normalize(n);


