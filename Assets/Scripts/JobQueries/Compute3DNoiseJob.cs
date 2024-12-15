using Generation;
using TerrainGeneration;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace JobQueries
{
    /// <summary>
    /// Used to compute large amounts of noise
    /// </summary>
    [BurstCompile]
    public struct Compute3DNoiseJob : IJob, IFlushNativeArrays
    {
        public NoiseMapSettings Settings;
        public NativeArray<float> NoiseValues;
        public NativeArray<Vector3> Positions;

        public Compute3DNoiseJob(NoiseMapSettings settings, NativeArray<Vector3> positions)
        {
            Settings = settings;
            Positions = positions;
            NoiseValues = new NativeArray<float>(positions.Length, Allocator.TempJob);
        }

        public void Execute()
        {
            ComputeNoiseValues();

            for (int i = 0; i < NoiseValues.Length; i++)
            {
                Positions[i] *= (1 + NoiseValues[i]);
            }
        }

        void ComputeNoiseValues()
        {
            int arrayLength = Positions.Length;
            Vector3 vert;
            float value;

            for (int i = 0; i < arrayLength; i++)
            {
                vert = Positions[i];
                value = NoiseMapSampler.SampleNoise3D(vert.x, vert.y, vert.z, Settings.freq, Settings.octaves, Settings.seed);
                NoiseValues[i] = value * Settings.multiplier;
            }
        }

        public void Flush()
        {
            NoiseValues.Dispose();
            Positions.Dispose();
        }
    }
}
