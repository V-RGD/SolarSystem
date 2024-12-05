using Generation;
using UnityEngine;

namespace TerrainGeneration
{
    public static class NoiseMapSampler
    {
        static int _seed3D;
        static float _maxPossibleNoise3D;
        static float _noise3DSampled;

        public static float SampleNoise3D(float x, float y, float z, float freq, int octaves, int seed)
        {
            _maxPossibleNoise3D = 1;
            for (int i = 1; i < octaves; i++)
            {
                float a = 0.5f;
                for (int j = 1; j < i; j++)
                {
                    a *= 0.5f;
                }
                
                _maxPossibleNoise3D += a;
            }

            _seed3D = seed;

            //samples noises
            _noise3DSampled = FbmNoise.Fbm3D(x + _seed3D, y + _seed3D, z + _seed3D, freq, octaves);
            _noise3DSampled /= _maxPossibleNoise3D;

            return _noise3DSampled;
        }
    }
}