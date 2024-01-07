
#ifndef SHF_PERLIN_NOISE
#define SHF_PERLIN_NOISE

// Copied from BlenderMalt repository and adapted for HLSL
// Repository: https://github.com/bnpr/Malt

#include "SHF_Hash.cginc"

float4 PerlinNoise4Tiled(float4 coord, int4 tile_size)
{
    
    #define DIMENSIONS 4
    #define T float4
    #define TILE 1
    #define hash Hash44

    #include "SHF_PerlinNoise.inl"
    
    #undef DIMENSIONS
    #undef T
    #undef TILE
    #undef hash

    return color;
}

float4 PerlinNoise4(float4 coord)
{
    #define DIMENSIONS 4
    #define T float4
    #define TILE 0
    #define hash Hash44
    
    #include "SHF_PerlinNoise.inl"

    #undef DIMENSIONS
    #undef T
    #undef TILE
    #undef hash
    
    return color;
}

float4 PerlinNoise3Tiled(float3 coord, int3 tile_size)
{
    #define DIMENSIONS 3
    #define T float3
    #define TILE 1
    #define hash Hash43
    
    #include "SHF_PerlinNoise.inl"
    #undef DIMENSIONS
    #undef T
    #undef TILE
    #undef hash
    
    return color;
}

float4 PerlinNoise3(float3 coord)
{
    #define DIMENSIONS 3
    #define T float3
    #define TILE 0
    #define hash Hash43
    
    #include "SHF_PerlinNoise.inl"

    #undef DIMENSIONS
    #undef T
    #undef TILE
    #undef hash
    
    return color;
}

float4 PerlinNoise2Tiled(float2 coord, int2 tile_size)
{
    #define DIMENSIONS 2
    #define T float2
    #define TILE 1
    #define hash Hash42

    #include "SHF_PerlinNoise.inl"

    #undef DIMENSIONS
    #undef T
    #undef TILE
    #undef hash
    
    return color;
}

float4 PerlinNoise2(float2 coord)
{
    #define DIMENSIONS 2
    #define T float2
    #define TILE 0
    #define hash Hash42
    
    #include "SHF_PerlinNoise.inl"

    #undef DIMENSIONS
    #undef T
    #undef TILE
    #undef hash

    return color;
}

float4 PerlinNoise1Tiled(float coord, int tile_size)
{
    #define DIMENSIONS 1
    #define T float
    #define TILE 1
    #define hash Hash41

    #include "SHF_PerlinNoise.inl"

    #undef DIMENSIONS
    #undef T
    #undef TILE
    #undef hash

    return color;
}

float4 PerlinNoise1(float coord)
{
    #define DIMENSIONS 1
    #define T float
    #define TILE 0
    #define hash Hash41
    
    #include "SHF_PerlinNoise.inl"

    #undef DIMENSIONS
    #undef T
    #undef TILE
    #undef hash

    return color;
}

#endif
