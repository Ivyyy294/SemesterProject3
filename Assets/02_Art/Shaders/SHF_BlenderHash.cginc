#ifndef SHF_BLENDER_HASH_GLSL
#define SHF_BLENDER_HASH_GLSL

// Copied from BlenderMalt repository and adapted for HLSL
// Repository: https://github.com/bnpr/Malt

uint4 _pcg4dInt(uint4 v)
{
    //http://www.jcgt.org/published/0009/03/02/
    v = v * 1664525u + 1013904223u;
    v.x += v.y*v.w;
    v.y += v.z*v.x;
    v.z += v.x*v.y;
    v.w += v.y*v.z;
    v ^= v >> 16u;
    v.x += v.y*v.w;
    v.y += v.z*v.x;
    v.z += v.x*v.y;
    v.w += v.y*v.z;
    return v;
}

float4 _pcg4d(float4 v)
{
    uint4 u = _pcg4dInt(asfloat(v));
    return float4(u) / float(0xffffffffU);
}

float4 hash(float v){ return _pcg4d(float4(v,0,0,0)); }
float4 hash(float2  v){ return _pcg4d(float4(v,0,0)); }
float4 hash(float3  v){ return _pcg4d(float4(v,0)); }
float4 hash(float4  v){ return _pcg4d(v); }

#endif
