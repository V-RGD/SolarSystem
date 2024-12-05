using Unity.Mathematics;
using UnityEngine;

namespace TerrainGeneration
{
    public static class FbmNoise
    {
        static float _contribution;
        static float _noise;
        static float _rescaleFactor;

        public static float Fbm3D(float x, float y, float z, float frequency, int octaves)
        {
            _contribution = 1;
            _noise = 0.0f;

            for (int i = 0; i < octaves; ++i)
            {
                _noise += Perlin3D.Get3DPerlinNoise(x, y, z, frequency) * _contribution;
                _contribution *= 0.5f;
                frequency *= 2.0f;
            }

            return _noise;
        }

        static float _sample2D;

        public static float Fbm2D(Vector2 coordinates, float frequency, int octaves)
        {
            _contribution = 1;
            _noise = 0.0f;

            for (int i = 0; i < octaves; ++i)
            {
                _sample2D = Mathf.PerlinNoise(coordinates.x * frequency, coordinates.y * frequency) * _contribution;
                _noise += _sample2D;
                _contribution /= 2.0f;
                frequency *= 2.0f;
            }

            return _noise;
        }
    }
}