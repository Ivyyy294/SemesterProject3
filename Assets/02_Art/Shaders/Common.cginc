
#ifndef SHF_COMMON
#define SHF_COMMON

#define snap(a, b) floor(a / b) * b

#define snapRound(a, b) floor((a / b) + .5) * b

#define snapCeil(a, b) floor((a / b) + 1.) * b

#define remap(v, fromMin, fromMax, toMin, toMax) toMin + (v - fromMin) * (toMax - toMin) / (fromMax - fromMin)

#define remapClamped(v, fromMin, fromMax, toMin, toMax) clamp(remap(v, fromMin, fromMax, toMin, toMax), min(toMin, toMax), max(toMin, toMax))

#define greater(a, b) a > b? 1:0

#define lesser(a, b) a < b? 1:0

#define rmod(a, b) frac(a / b) * b

float2 lengthSquared(float2 a)
{
    return (a.x * a.x) + (a.y * a.y);
}

float3 lengthSquared(float3 a)
{
    return (a.x * a.x) + (a.y * a.y) + (a.z * a.z);
}

float4 lengthSquared(float4 a)
{
    return (a.x * a.x) + (a.y * a.y) + (a.z * a.z) + (a.w * a.w);
}

int bayerIndex(uint2 texelIndex, int matrixWidth)
{
    return texelIndex.x % matrixWidth + matrixWidth * (texelIndex.y % matrixWidth);
}

float bayer4x4(int index)
{
    const int bayerMatrix[16] = {
        0,8,2,10,
        12,4,14,6,
        3,11,1,9,
        15,7,13,5};
    return bayerMatrix[index] / 16.;
}

float bayer8x8(int index)
{
    const int bayerMatrix[] = {
        0,32,8,40,2,34,10,42,
        48,16,56,24,50,18,58,26,
        12,44,4,36,14,46,6,38,
        60,28,53,20,62,30,54,22,
        3,35,11,43,1,33,9,41,
        51,19,59,27,49,17,57,25,
        15,47,7,39,13,45,5,37,
        63,31,55,23,61,29,53,21
    };
    return bayerMatrix[index] / 64.;
}

float3 limitColors(float3 color, int stops)
{
    float frac = 1. / (stops + 1);
    return snapRound(color, float3(frac, frac, frac));
}

float4 alphaBlend(float4 base, float4 blend)
{
    if(blend.a <= 0)
    {
        return base;
    }
    else if(blend.a >= 1)
    {
        return blend;
    }
    
    float alpha = blend.a + base.a * (1.0 - blend.a);
    float4 result = (blend * blend.a + base * base.a * (1.0 - blend.a)) / alpha;
    result.a = alpha;

    return result;
}

float4 clipToScreen(float4 clip)
{
    float4 screen = float4(clip.xyz / clip.w, clip.w);
    screen.xy = screen.xy * float2(0.5, -0.5) + 0.5;
    return screen;
}

float4 screenToClip(float2 screen, float z, float w)
{
    screen.xy = (screen.xy - 0.5) * float2(2, -2);
    return float4(float3(screen, z) * w, w);
}

float4 screenToClip(float3 screen, float w)
{
    return screenToClip(screen.xy, screen.z, w);
}

float4 screenToClip(float4 screen)
{
    return screenToClip(screen.xy, screen.z, screen.w);
}

#endif

