using Generation;
using UnityEngine;

namespace TerrainGeneration
{
    public static class NoiseMapSampler
    {
//         static Vector2Int _columnPos;
//         static float _noiseSampled2D;
//         static float[,] _map2D;
//         static Vector2Int _blockToUpdate2D;
//         static Vector2Int _sampledBlock2D;
//
//         public static float[,] SampleMap2D(Vector3Int chunkOffset, Vector2Int extents, NoiseMapParams noiseMapParams, int sampleSize)
//         {
//             _map2D = new float[extents.x, extents.y];
//
//             float maxPossibleNoise = 1;
//             for (int i = 1; i < noiseMapParams.octaves; i++)
//             {
//                 maxPossibleNoise += Mathf.Pow(0.5f, i);
//             }
//
//             Vector2Int rescaledMapExtents = new Vector2Int(extents.x / sampleSize, extents.y / sampleSize);
//
//             //for each tile
//             for (int x = 0; x < rescaledMapExtents.x; x++)
//             {
//                 for (int y = 0; y < rescaledMapExtents.y; y++)
//                 {
//                     //computes pos in noise texture (2D)
//                     _sampledBlock2D = new Vector2Int((x * sampleSize), (y * sampleSize));
//                     _columnPos = new Vector2Int(chunkOffset.x, chunkOffset.z) + _sampledBlock2D;
//
//                     //samples noises
//                     _noiseSampled2D = FbmNoise.Fbm2D(_columnPos + new Vector2Int(noiseMapParams.seed, noiseMapParams.seed), noiseMapParams.freq,
//                         noiseMapParams.octaves);
//                     _noiseSampled2D /= maxPossibleNoise;
//
//                     //updates noise for each block of the sample
//                     for (int sampleX = 0; sampleX < sampleSize; sampleX++)
//                     {
//                         for (int sampleY = 0; sampleY < sampleSize; sampleY++)
//                         {
//                             _blockToUpdate2D = new Vector2Int(sampleX, sampleY) + _sampledBlock2D;
//
//                             if (CR_ArrayHelpers.IsOutsideBounds(_blockToUpdate2D.x, _blockToUpdate2D.y, extents.x, extents.y)) continue;
//
//                             _map2D[_blockToUpdate2D.x, _blockToUpdate2D.y] = _noiseSampled2D;
//                         }
//                     }
//                 }
//             }
//
//             return _map2D;
//         }
//
//         static float _noiseSampled3D;
//         static float[,,] _map3D;
//
//         static int _extentsX;
//         static int _extentsY;
//         static int _extentsZ;
//
//         static int _sampledX;
//         static int _sampledY;
//         static int _sampledZ;
//
//         static int _blockPosX;
//         static int _blockPosY;
//         static int _blockPosZ;
//
//         static int _updatedBlockX;
//         static int _updatedBlockY;
//         static int _updatedBlockZ;
//
//         static Vector3Int _rescaledMapExtents3D;
//
//         // ReSharper disable Unity.PerformanceAnalysis
//         public static float[,,] SampleMap3D(Vector3Int chunkOffset, Vector3Int extents, NoiseMapParams noiseMapParams, int sampleSize)
//         {
//             _extentsX = extents.x;
//             _extentsY = extents.y;
//             _extentsZ = extents.z;
//
//             int seed = noiseMapParams.seed;
//
//             _map3D = new float[_extentsX, _extentsY, _extentsZ];
//
//             int chunkOffsetX = chunkOffset.x;
//             int chunkOffsetY = chunkOffset.y;
//             int chunkOffsetZ = chunkOffset.z;
//
//             float maxPossibleNoise = 1;
//             for (int i = 1; i < noiseMapParams.octaves; i++)
//             {
//                 maxPossibleNoise += Mathf.Pow(0.5f, i);
//             }
//
//             _rescaledMapExtents3D = new Vector3Int(_extentsX / sampleSize, _extentsY / sampleSize, _extentsZ / sampleSize);
//
//             //for each tile
//             for (int x = 0; x < _rescaledMapExtents3D.x; x++)
//             {
//                 for (int y = 0; y < _rescaledMapExtents3D.y; y++)
//                 {
//                     for (int z = 0; z < _rescaledMapExtents3D.z; z++)
//                     {
//                         //computes pos in noise texture (3D)
//                         _sampledX = x * sampleSize;
//                         _sampledY = y * sampleSize;
//                         _sampledZ = z * sampleSize;
//
//                         _blockPosX = _sampledX + chunkOffsetX;
//                         _blockPosY = _sampledY + chunkOffsetY;
//                         _blockPosZ = _sampledZ + chunkOffsetZ;
//
//                         //samples noises
//                         _noiseSampled3D = FbmNoise.Fbm3D(seed + _blockPosX, seed + _blockPosY, seed + _blockPosZ,
//                             noiseMapParams.freq, noiseMapParams.octaves);
//
//                         _noiseSampled2D /= maxPossibleNoise;
//
//                         //updates noise for each block of the sample
//                         for (int sampleX = 0; sampleX < sampleSize; sampleX++)
//                         {
//                             for (int sampleY = 0; sampleY < sampleSize; sampleY++)
//                             {
//                                 for (int sampleZ = 0; sampleZ < sampleSize; sampleZ++)
//                                 {
//                                     _updatedBlockX = sampleX + _sampledX;
//                                     _updatedBlockY = sampleY + _sampledY;
//                                     _updatedBlockZ = sampleZ + _sampledZ;
//
//                                     if (CR_ArrayHelpers.IsOutsideBounds(_updatedBlockX, _updatedBlockY, _updatedBlockZ,
//                                             _extentsX, _extentsY, _extentsZ)) continue;
//
//                                     _map3D[_updatedBlockX, _updatedBlockY, _updatedBlockZ] = _noiseSampled3D;
//                                 }
//                             }
//                         }
//                     }
//                 }
//             }
//
//             return _map3D;
//         }
//
//         static float _noiseSampled;
//         static float _maxPossibleNoise2D;
//
//         public static float SampleNoise2D(Vector2Int position, NoiseMapParams noiseMapParams)
//         {
//             _maxPossibleNoise2D = 1;
//             for (int i = 1; i < noiseMapParams.octaves; i++)
//             {
//                 float a = 0.5f;
//                 for (int j = 1; j < i; j++)
//                 {
//                     a *= 0.5f;
//                 }
//                 
//                 _maxPossibleNoise2D += a;
//             }
//
//             //samples noises
//             _noiseSampled = FbmNoise.Fbm2D(position + new Vector2Int(noiseMapParams.seed, noiseMapParams.seed), noiseMapParams.freq, noiseMapParams.octaves);
//             _noiseSampled /= _maxPossibleNoise2D;
//
//             return _noiseSampled;
//         }
//
        static int _seed3D;
        static float _maxPossibleNoise3D;
        static float _noise3DSampled;

        public static float SampleNoise3D(float x, float y, float z, NoiseMapParams noiseMapParams)
        {
            _maxPossibleNoise3D = 1;
            for (int i = 1; i < noiseMapParams.octaves; i++)
            {
                float a = 0.5f;
                for (int j = 1; j < i; j++)
                {
                    a *= 0.5f;
                }
                
                _maxPossibleNoise3D += a;
            }

            _seed3D = noiseMapParams.seed;

            //samples noises
            _noise3DSampled = FbmNoise.Fbm3D(x + _seed3D, y + _seed3D, z + _seed3D, noiseMapParams.freq, noiseMapParams.octaves);
            _noise3DSampled /= _maxPossibleNoise3D;

            return _noise3DSampled;
        }
    }
}