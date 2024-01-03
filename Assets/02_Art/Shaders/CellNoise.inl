//Should define T, DIMENSIONS, TILE and hash
//Should declare coord and tile_size (if TILE)

// Copied from BlenderMalt repository and adapted for HLSL
// Repository: https://github.com/bnpr/Malt

T real_coord = coord;
#if TILE != 0
T _tile_size = round((T)(tile_size));
coord = rmod(coord, _tile_size);
#endif
T real_origin = floor(real_coord) + 0.5;
T origin = floor(coord) + 0.5;
T delta = frac(coord);

float4 r_cell_color;
T r_cell_position;
float r_cell_distance = 10;

#if DIMENSIONS == 4
const int4 D = int4(1,1,1,1);
#elif DIMENSIONS == 3
const int4 D = int4(1,1,1,0);
#elif DIMENSIONS == 2
const int4 D = int4(1,1,0,0);
#elif DIMENSIONS == 1
const int4 D = int4(1,0,0,0);
#endif

for(int w = -D.w; w <= D.w; w++)
{
    for(int z = -D.z; z <= D.z; z++)
    {
        for(int y = -D.y; y <= D.y; y++)
        {
            for(int x = -D.x; x <= D.x; x++)
            {
                T offset = (T)(float4(x,y,z,w));
                T real_corner = real_origin + offset;
                T corner = origin + offset;
                #if TILE != 0
                corner = rmod(corner, _tile_size);
                #endif
                float4 corner_hash = hash(corner);
                T real_cell_position = real_corner + ((T)(corner_hash) - 0.5);
                T cell_position = corner + ((T)(corner_hash) - 0.5);
                float cell_distance = distance(real_coord, real_cell_position);
                if(cell_distance < r_cell_distance)
                {
                    r_cell_color = corner_hash;
                    r_cell_position = real_cell_position;
                    r_cell_distance = cell_distance;
                }
            }
        }
    }
}