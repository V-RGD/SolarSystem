using CelestialBodies;
using NaughtyAttributes;
using UnityEngine;

public class BiomeGenerator : GenericSingletonClass<BiomeGenerator>
{
    [field:SerializeField, Expandable]public  Biome[] Biomes { get; private set; }

    [SerializeField, CurveRange(0, 0, 1, 1)]
    AnimationCurve temperatureByLatitude;

    [SerializeField, CurveRange(0, 0, 1, 1)]
    AnimationCurve temperatureByAltitude;

    [SerializeField, CurveRange(0, 0, 1, 1)]
    AnimationCurve humidityByTemperature;
    
    public Planet.VertexWeatherConditions GeneratePlanetBiomes(Planet.GlobalWeatherConditions globalConditions, Planet.MeshData meshData)
    {
        Planet.VertexWeatherConditions vwc = new Planet.VertexWeatherConditions(meshData.Displacements.Length);
    
        Vector3 vertex;
        float latitude;
    
        //compute base vertex values
        float e;
        float t;
        float h;
        int biome;
        
        //for each vertex
        for (int i = 0; i < meshData.Displacements.Length; i++)
        {
            vertex = meshData.Ico.Vertices[i];
            latitude = Mathf.Abs(vertex.y);
    
            //compute base vertex values
            e = meshData.Displacements[i] / globalConditions.MaxElevation + 0.5f;
            t = globalConditions.GlobalTemperature;
            h = globalConditions.GlobalHumidity;
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
            vwc.BiomeIndices[i] = biome;
            vwc.Elevations[i] = e;
            vwc.Temperatures[i] = t;
            vwc.Humidity[i] = h;
        }
    
        return vwc;
    }
    
    int GetClosestBiome(float t, float h)
    {
        int closestBiome = int.MaxValue;
        float bestScore = float.MaxValue;

        for (int i = 0; i < Biomes.Length; i++)
        {
            var biome = Biomes[i];
            float score = 0;
            score += Mathf.Abs(biome.temperature - t);
            // score += Mathf.Abs(biome.habitability - t);

            if (score < bestScore)
            {
                bestScore = score;
                closestBiome = i;
            }
        }

        return closestBiome;
    }
}