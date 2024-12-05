using Generation;
using UnityEngine;

namespace TerrainGeneration
{
    public static class NoiseMapSampler
    {
        public static float SampleNoise3D(float x, float y, float z, float freq, int octaves, int seed)
        {
            float maxPossibleNoise3D = 1;
            for (int i = 1; i < octaves; i++)
            {
                float a = 0.5f;
                for (int j = 1; j < i; j++)
                {
                    a *= 0.5f;
                }
                
                maxPossibleNoise3D += a;
            }

            float seed3D = seed;

            //samples noises
            float noise3DSampled = FbmNoise.Fbm3D(x + seed3D, y + seed3D, z + seed3D, freq, octaves);
            noise3DSampled /= maxPossibleNoise3D;

            return noise3DSampled;
        }
    }
}