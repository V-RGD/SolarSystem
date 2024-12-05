using System;
using System.Collections.Generic;
using Generation;
using NaughtyAttributes;
using UnityEngine;

public class BiomeGenerator : GenericSingletonClass<BiomeGenerator>
{
    [SerializeField, Expandable] Biome[] biomes;

    [SerializeField, CurveRange(0, 0, 1, 1)]
    AnimationCurve temperatureByLatitude;

    [SerializeField, CurveRange(0, 0, 1, 1)]
    AnimationCurve temperatureByAltitude;

    [SerializeField, CurveRange(0, 0, 1, 1)]
    AnimationCurve humidityByTemperature;

    public SphereValueMap<Biome> GeneratePlanetBiomes(PlanetGenerationData data)
    {
        SphereValueMap<Biome> biomeMap = new SphereValueMap<Biome>(data.VertexPositions.Length);
        // data.Height = new SphereValueMap<float>(data.VertexPositions.Count);
        // data.Temperature = new SphereValueMap<float>(data.VertexPositions.Count);
        // data.Humidity = new SphereValueMap<float>(data.VertexPositions.Count);

        Vector3 vertex;

        float latitude;

        //compute base vertex values
        float e;
        float t;
        float h;
        Biome biome;
        
        //for each vertex
        for (int i = 0; i < data.VertexPositions.Length; i++)
        {
            vertex = data.VertexPositions[i];

            latitude = Mathf.Abs(vertex.y);

            //compute base vertex values
            e = data.VertexDisplacement.Map[i].Value / data.MaxElevation + 0.5f;
            t = data.PlanetTemperature;
            h = data.PlanetHumidity;
            h = 0;

            //elevation boosts cold temperatures if high and hot ones if low
            t *= temperatureByAltitude.Evaluate(e);
            // h *= temperatureByAltitude.Evaluate(e);

            //latitude reduces temperature at poles
            t *= temperatureByLatitude.Evaluate(latitude);
            // h *= temperatureByLatitude.Evaluate(latitude);

            //temperature reduces humidity
            //h *= humidityByTemperature.Evaluate(t);

            //for each vertex, computes biome that needs to be assigned
            biome = GetClosestBiome(t, h);

            //registers vertex data
            biomeMap.Map[i] = new KeyValuePair<Vector3, Biome>(vertex, biome);
            data.Height.Map[i] = new KeyValuePair<Vector3, float>(vertex, e);
            data.Temperature.Map[i] = new KeyValuePair<Vector3, float>(vertex, t);
            data.Humidity.Map[i] = new KeyValuePair<Vector3, float>(vertex, h);
        }

        return biomeMap;
    }

    Biome GetClosestBiome(float t, float h)
    {
        Biome closestBiome = null;
        float bestScore = float.MaxValue;

        foreach (var biome in biomes)
        {
            float score = 0;
            // score += biome.habitability - h;
            score += Mathf.Abs(biome.temperature - t);
            score += Mathf.Abs(biome.habitability - t);

            if (score < bestScore)
            {
                bestScore = score;
                closestBiome = biome;
            }
        }

        // if (closestBiome == null) Debug.LogWarning("no biome found");
        //if (closestBiome != null) Debug.Log($"{closestBiome.name}");
        // else Debug.Log("biome found");

        return closestBiome;
    }
}