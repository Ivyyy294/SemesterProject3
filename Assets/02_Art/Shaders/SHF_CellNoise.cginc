#ifndef SHF_CELL_NOISE
#define SHF_CELL_NOISE

// Copied from BlenderMalt repository and adapted for HLSL
// Repository: https://github.com/bnpr/Malt

#include "SHF_Hash.cginc"
#include "SHF_Common.cginc"

struct CellNoiseResult
{
    float4 Color;
    float4 Position;
    float Distance;
};

CellNoiseResult CellNoise4(float4 coord)
{
    CellNoiseResult result;
    #define DIMENSIONS 4
    #define T float4
    #define TILE 0
    #define hash Hash44
    
    #include "SHF_CellNoise.inl"
    result.Color = r_cell_color;
    result.Position = r_cell_position;
    result.Distance = r_cell_distance;

    #undef DIMENSIONS
    #undef T
    #undef TILE
    #undef hash
    
    return result;
}

CellNoiseResult CellNoise4Tiled(float4 coord, int4 tile_size)
{
    CellNoiseResult result;
    #define DIMENSIONS 4
    #define T float4
    #define TILE 1
    #define hash Hash44
    
    #include "SHF_CellNoise.inl"
    result.Color = r_cell_color;
    result.Position = r_cell_position;
    result.Distance = r_cell_distance;

    #undef DIMENSIONS
    #undef T
    #undef TILE
    #undef hash

    return result;
}

CellNoiseResult CellNoise3(float3 coord)
{
    CellNoiseResult result;
    #define DIMENSIONS 3
    #define T float3
    #define TILE 0
    #define hash Hash43
    
    #include "SHF_CellNoise.inl"
    result.Color = r_cell_color;
    result.Position.xyz = r_cell_position;
    result.Distance = r_cell_distance;

    #undef DIMENSIONS
    #undef T
    #undef TILE
    #undef hash

    return result;
}

CellNoiseResult CellNoise3Tiled(float3 coord, int3 tile_size)
{
    CellNoiseResult result;
    #define DIMENSIONS 3
    #define T float3
    #define TILE 1
    #define hash Hash43
    
    #include "SHF_CellNoise.inl"
    result.Color = r_cell_color;
    result.Position.xyz = r_cell_position;
    result.Distance = r_cell_distance;
    
    #undef DIMENSIONS
    #undef T
    #undef TILE
    #undef hash

    return result;
}

CellNoiseResult CellNoise2(float2 coord)
{
    CellNoiseResult result;
    #define DIMENSIONS 2
    #define T float2
    #define TILE 0
    #define hash Hash42
    
    #include "SHF_CellNoise.inl"
    result.Color = r_cell_color;
    result.Position.xy = r_cell_position;
    result.Distance = r_cell_distance;

    #undef DIMENSIONS
    #undef T
    #undef TILE
    #undef hash

    return result;
}

CellNoiseResult CellNoise2Tiled(float2 coord, int2 tile_size)
{
    CellNoiseResult result;
    #define DIMENSIONS 2
    #define T float2
    #define TILE 1
    #define hash Hash42
    
    #include "SHF_CellNoise.inl"
    result.Color = r_cell_color;
    result.Position.xy = r_cell_position;
    result.Distance = r_cell_distance;

    #undef DIMENSIONS
    #undef T
    #undef TILE
    #undef hash

    return result;
}

CellNoiseResult CellNoise1(float coord)
{
    CellNoiseResult result;
    #define DIMENSIONS 1
    #define T float
    #define TILE 0
    #define hash Hash41
    
    #include "SHF_CellNoise.inl"
    result.Color = r_cell_color;
    result.Position.x = r_cell_position;
    result.Distance = r_cell_distance;

    #undef DIMENSIONS
    #undef T
    #undef TILE
    #undef hash

    return result;
}

CellNoiseResult CellNoise1Tiled(float coord, int tile_size)
{
    CellNoiseResult result;
    #define DIMENSIONS 1
    #define T float
    #define TILE 1
    #define hash Hash41
    
    #include "SHF_CellNoise.inl"
    result.Color = r_cell_color;
    result.Position.x = r_cell_position;
    result.Distance = r_cell_distance;

    #undef DIMENSIONS
    #undef T
    #undef TILE
    #undef hash

    return result;
}

#endif
