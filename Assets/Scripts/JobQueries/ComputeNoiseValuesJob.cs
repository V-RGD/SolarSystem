﻿using Generation;
using TerrainGeneration;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace JobQueries
{
    /// <summary>
    /// Used to compute large amounts of noise
    /// </summary>
    public struct ComputeNoiseValuesJob : IJob
    {
        public NoiseMapSettings Settings;
        public NativeArray<float> NoiseValues;
        public NativeArray<Vector3> Positions;

        public ComputeNoiseValuesJob(NoiseMapSettings settings, NativeArray<float> noiseValues, NativeArray<Vector3> positions)
        {
            Settings = settings;
            NoiseValues = noiseValues;
            Positions = positions;
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
                value = NoiseMapSampler.SampleNoise3D(vert.x, vert.y, vert.z, Settings.freq, Settings.octaves,
                    Settings.seed);
                NoiseValues[i] = value * Settings.multiplier;
            }
        }
    }
}