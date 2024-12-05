using SimplexNoise;
using UnityEngine;

namespace TerrainGeneration
{
    public static class FbmNoise
    {
        public static float Fbm3D(float x, float y, float z, float frequency, int octaves)
        {
            float contribution = 1;
            float noise = 0.0f;

            for (int i = 0; i < octaves; ++i)
            {
                noise += Perlin3D.Get3DPerlinNoise(x, y, z, frequency) * contribution;
                // noise += Noise.CalcPixel3D(x, y, z, frequency) *contribution;
                contribution *= 0.5f;
                frequency *= 2.0f;
            }

            return noise;
        }
        
        public static float Fbm2D(Vector2 coordinates, float frequency, int octaves)
        {
            float contribution = 1;
            float noise = 0.0f;
            float sample2D;

            for (int i = 0; i < octaves; ++i)
            {
                sample2D = Mathf.PerlinNoise(coordinates.x * frequency, coordinates.y * frequency) * contribution;
                noise += sample2D;
                contribution /= 2.0f;
                frequency *= 2.0f;
            }

            return noise;
        }
    }
}