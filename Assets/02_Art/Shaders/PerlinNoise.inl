//Should define T, DIMENSIONS and TILE
//Should declare coord and tile_size (if TILE)

// Copied from BlenderMalt repository and adapted for HLSL
// Repository: https://github.com/bnpr/Malt

#if TILE != 0
    T _tile_size = round(tile_size);
    coord = fmod(coord, _tile_size);
#endif
T origin = floor(coord);
T delta = frac(coord);
//quintic interpolation factor
T factor = delta*delta*delta*(delta*(delta*6.0-15.0)+10.0);

#if DIMENSIONS == 4
    const int4 D = int4(1,1,1,1);
#elif DIMENSIONS == 3
    const int4 D = int4(1,1,1,0);
#elif DIMENSIONS == 2
    const int4 D = int4(1,1,0,0);
#elif DIMENSIONS == 1
    const int4 D = int4(1,0,0,0);
#endif

float4 w_results[2];
float4 z_results[2];
float4 y_results[2];
float4 x_results[2];

for(int w = 0; w <= D.w; w++)
{
    for(int z = 0; z <= D.z; z++)
    {
        for(int y = 0; y <= D.y; y++)
        {
            for(int x = 0; x <= D.x; x++)
            {
                T offset = (T)float4(x,y,z,w);
                T corner = origin + offset;
                #if TILE != 0
                    corner = fmod(corner, _tile_size);
                #endif
                T corner_hash_r = (T)hash(corner) * 2.0 - 1.0; //(-1|+1) range
                T corner_hash_g = (T)hash(corner_hash_r) * 2.0 - 1.0;
                T corner_hash_b = (T)(hash(corner_hash_g)) * 2.0 - 1.0;
                T corner_hash_a = (T)(hash(corner_hash_b)) * 2.0 - 1.0;
                x_results[x].r = dot(corner_hash_r, delta-offset);
                x_results[x].g = dot(corner_hash_g, delta-offset);
                x_results[x].b = dot(corner_hash_b, delta-offset);
                x_results[x].a = dot(corner_hash_a, delta-offset);
            }
            #if DIMENSIONS >= 2
                y_results[y] = lerp(x_results[0], x_results[1], factor.x);
            #endif
        }
        #if DIMENSIONS >= 3
            z_results[z] = lerp(y_results[0], y_results[1], factor.y);
        #endif
    }
    #if DIMENSIONS >= 4
        w_results[w] = lerp(z_results[0], z_results[1], factor.z);
    #endif
}

float4 color;
#if DIMENSIONS == 4
    color = lerp(w_results[0], w_results[1], factor.w) * 0.5 + 0.5;
#elif DIMENSIONS == 3
    color = lerp(z_results[0], z_results[1], factor.z) * 0.5 + 0.5;
#elif DIMENSIONS == 2
    color = lerp(y_results[0], y_results[1], factor.y) * 0.5 + 0.5;
#elif DIMENSIONS == 1
    color = lerp(x_results[0], x_results[1], factor) * 0.5 + 0.5;
#endif
