#ifndef RANDOM
#define RANDOM

// Hashes

float Hash11(float p)
{
    p = frac(p * .1031);
    p *= p + 33.33;
    p *= p + p;
    return frac(p);
}

float Hash12(float2 p)
{
    float3 p3  = frac(float3(p.xyx) * .1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return frac((p3.x + p3.y) * p3.z);
}

float Hash13(float3 p3)
{
    p3  = frac(p3 * .1031);
    p3 += dot(p3, p3.zyx + 31.32);
    return frac((p3.x + p3.y) * p3.z);
}

float Hash14(float4 vec)
{
    vec = frac(vec  * float4(.1031, .1030, .0973, .1099));
    vec += dot(vec, vec.wzxy+33.33);
    return frac((vec.x + vec.y) * (vec.z + vec.w));
}

float2 Hash21(float p)
{
    float3 p3 = frac(p.xxx * float3(.1031, .1030, .0973));
    p3 += dot(p3, p3.yzx + 33.33);
    return frac((p3.xx+p3.yz)*p3.zy);
}

float2 Hash22(float2 p)
{
    float3 p3 = frac(float3(p.xyx) * float3(.1031, .1030, .0973));
    p3 += dot(p3, p3.yzx+33.33);
    return frac((p3.xx+p3.yz)*p3.zy);

}

float2 Hash23(float3 p3)
{
    p3 = frac(p3 * float3(.1031, .1030, .0973));
    p3 += dot(p3, p3.yzx+33.33);
    return frac((p3.xx+p3.yz)*p3.zy);
}

float3 Hash31(float p)
{
    float3 p3 = frac(p.xxx * float3(.1031, .1030, .0973));
    p3 += dot(p3, p3.yzx+33.33);
    return frac((p3.xxy+p3.yzz)*p3.zyx); 
}

float3 Hash32(float2 p)
{
    float3 p3 = frac(float3(p.xyx) * float3(.1031, .1030, .0973));
    p3 += dot(p3, p3.yxz+33.33);
    return frac((p3.xxy+p3.yzz)*p3.zyx);
}

float3 Hash33(float3 vec)
{
    return float3(
        Hash14(float4(vec, 0.344)),
        Hash14(float4(vec, 0.403)),
        Hash14(float4(vec, 0.065))
    );
}

float PerlinNoise(float2 uv, int scale,  float randomScaler = 23457.124, float randomW = 345.3234)
{
    uv *= scale;
    float2 gridID = floor(uv);
    float2 gridUV = frac(uv);

    float2 c00 = gridID;
    float2 c10 = (gridID + float2(1, 0)) % scale;
    float2 c01 = (gridID + float2(0, 1)) % scale;
    float2 c11 = (gridID + float2(1, 1)) % scale;
    
    float r00 = Hash13(float3(c00 * randomScaler, randomW)) - 0.5;
    float r10 = Hash13(float3(c10 * randomScaler, randomW)) - 0.5 ;
    float r01 = Hash13(float3(c01 * randomScaler, randomW)) - 0.5;
    float r11 = Hash13(float3(c11 * randomScaler, randomW)) - 0.5;
    
    float2 d00 = gridUV;
    float2 d10 = gridUV - float2(1,0);
    float2 d01 = gridUV - float2(0,1);
    float2 d11 = gridUV - float2(1,1);

    float dot00 = dot(r00, d00);
    float dot10 = dot(r10, d10);
    float dot01 = dot(r01, d01);
    float dot11 = dot(r11, d11);

    gridUV = smoothstep(0, 1, gridUV);

    float b = lerp(dot00, dot10, gridUV.x);
    float t = lerp(dot01, dot11, gridUV.x);
    float perlin = lerp(b, t, gridUV.y);

    return perlin + 0.5;
}



#endif