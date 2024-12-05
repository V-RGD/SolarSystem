using System;
using System.Collections.Generic;
using TerrainGeneration;
using UnityEngine;

namespace Generation
{
    [Serializable]
    public class NoiseMapParams
    {
        [SerializeField] public float freq;
        [SerializeField] public int octaves;
        [SerializeField] public float multiplier;
        [HideInInspector] public int seed;

        public void InitSeed()
        {
            seed = SRnd.RangeInt(0, 10000);
        }
    }
    
    public struct PlanetGenerationData
    {
        public PlanetGenerationData(Vector3[] vertexPositions)
        {
            VertexPositions = new Vector3[vertexPositions.Length];
            for (int i = 0; i < vertexPositions.Length; i++) VertexPositions[i] = vertexPositions[i];
            
            VertexDisplacement = new SphereValueMap<float>(VertexPositions.Length);
            Humidity = new SphereValueMap<float>(VertexPositions.Length);
            Temperature = new SphereValueMap<float>(VertexPositions.Length);
            Height = new SphereValueMap<float>(VertexPositions.Length);
            Biomes = new SphereValueMap<Biome>(VertexPositions.Length);
            
            PlanetHumidity = 0;
            AtmosphericDensity = 0;
            PlanetTemperature = 0;
            MaxElevation = 0;
        }
        
        public readonly Vector3[] VertexPositions;
        
        //per-vertex stat
        public SphereValueMap<float> VertexDisplacement;
        public SphereValueMap<float> Height;
        public SphereValueMap<float> Temperature;
        public SphereValueMap<float> Humidity;
        public SphereValueMap<Biome> Biomes;
        
        //global values
        public float PlanetHumidity;
        public float PlanetTemperature;
        public float AtmosphericDensity;
        public float MaxElevation;
    }
    
    public class SphereValueMap<T>
    {
        public SphereValueMap(int a)
        {
            Map = new KeyValuePair<Vector3, T>[a];
        }
        
        public KeyValuePair<Vector3, T>[] Map;
    }

    public static class SphereMapUtilities
    {
        public static SphereValueMap<float> SampleMap(Vector3[] positions, NoiseMapParams noiseMapParams)
        {
            SphereValueMap<float> newMap = new SphereValueMap<float>(positions.Length);

            for (int i = 0; i < positions.Length; i++)
            {
                var pos = positions[i];
                float value = NoiseMapSampler.SampleNoise3D(pos.x, pos.y, pos.z, noiseMapParams);
                value *= noiseMapParams.multiplier;
                newMap.Map[i] = new KeyValuePair<Vector3, float>(pos, value);
            }

            return newMap;
        }
    }
}

