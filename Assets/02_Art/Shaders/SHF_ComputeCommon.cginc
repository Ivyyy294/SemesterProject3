#ifndef SHF_COMPUTE_COMMON
#define SHF_COMPUTE_COMMON

int UnravelID(uint3 id, int textureSize)
{
    return id.x + textureSize * id.y + id.z * textureSize * textureSize;
}

float2 UV(uint3 id, int textureSize)
{
    return id.xy / (float)textureSize;
}

float3 UVW(uint3 id, int textureSize)
{
    return id.xyz / (float)textureSize;
}

float2 RavelUV(uint i, int textureSize)
{
    return float2(
        fmod(i, textureSize),
        floor(i / textureSize)
        ) / textureSize;
}

float3 RavelUVW(uint i, int textureSize)
{
    int z = i / (textureSize * textureSize);
    int y = fmod(i, (textureSize * textureSize)) / textureSize;
    int x = fmod(i, textureSize);
    return float3(x,y,z) / textureSize;
}

#endif
