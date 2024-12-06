using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CelestialBodies;
using Generation;
using JobQueries;
using MeshGeneration;
using NaughtyAttributes;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Used to generate sun and planets
/// </summary>
public class SolarSystemGenerator : GenericSingletonClass<SolarSystemGenerator>
{
    [Header("Solar System Settings")] [SerializeField]
    float sunSize = 2;

    [SerializeField] int planetsToSpawn = 10;
    [SerializeField] MinMaxValue planetDistanceRange;

    [SerializeField] MinMaxValue planetSizeRange;
    // [SerializeField] MinMaxValue distanceRatioBetweenPlanets;

    [Header("Planet Settings")] [SerializeField, Range(0, 8)]
    int planetResolution = 7;

    [SerializeField] NoiseMapSettings planetElevationNoiseSettings;

    [SerializeField, CurveRange(0, 0, 1, 1)]
    AnimationCurve planetTemperatureByDistance;

    [SerializeField, CurveRange(0, 0, 1, 1)]
    AnimationCurve planetTiltRepartition;

    [SerializeField, CurveRange(0, 0, 1, 1)]
    AnimationCurve planetRotationSpeedRepartition;

    [SerializeField] MinMaxValue planetRotationSpeed;

    [Header("References")] [SerializeField]
    Planet planetPrefab;

    [SerializeField] Sun sun;
    public List<Planet> Planets { get; private set; } = new List<Planet>();

    [SerializeField, Range(0f, 10f)] float timeScale = 1;

    void Start()
    {
        GenerateSun(sunSize);
        GeneratePlanets();
    }

    void Update() => Time.timeScale = timeScale;

    void GenerateSun(float size) => sun.SetSize(size);

    void GeneratePlanets()
    {
        //setup planets settings
        // List<PlanetCreationJob> planetQueries = new List<PlanetCreationJob>();
        List<Planet.TransformData> planetDatas = new List<Planet.TransformData>();
        List<Planet.GlobalWeatherConditions> weatherConditions = new List<Planet.GlobalWeatherConditions>();

        // PlanetCreationJob.TerrainSettings terrainSettings =
        //     new PlanetCreationJob.TerrainSettings(planetResolution, planetElevationNoiseSettings);

        float lastPlanetDist = 0;
        // float lastPlanetSize = sunSize;
        float spacing = (planetDistanceRange.max - planetDistanceRange.min) / planetsToSpawn;

        for (int i = 0; i < planetsToSpawn; i++)
        {
            float size = Random.Range(planetSizeRange.min, planetSizeRange.max);

            //puts it at a safe distance from the last planet (or sun, if none)
            float distance = lastPlanetDist + spacing;

            float tilt = planetTiltRepartition.Evaluate(Random.value) * 90;
            if (SRnd.NextBool()) tilt *= -1;

            float rotationSpeed = Mathf.Lerp(planetRotationSpeed.min, planetRotationSpeed.max,
                planetRotationSpeedRepartition.Evaluate(SRnd.NextFloat()));

            //adds planet generation data
            planetDatas.Add(new Planet.TransformData(size, distance, tilt, rotationSpeed));

            float t = planetTemperatureByDistance.Evaluate(Mathf.InverseLerp(planetDistanceRange.min, planetDistanceRange.max, distance));

            Planet.GlobalWeatherConditions conditions = new Planet.GlobalWeatherConditions(t, SRnd.NextFloat(),
                SRnd.NextFloat(), planetElevationNoiseSettings.multiplier);

            weatherConditions.Add(conditions);

            // PlanetCreationJob.OutputData outputData = new PlanetCreationJob.OutputData();

            // PlanetCreationJob newJob = new PlanetCreationJob(terrainSettings, conditions);
            // planetQueries.Add(newJob);

            // lastPlanetSize = size;
            lastPlanetDist = distance;
        }

        // NativeArray<JobHandle> handles = new NativeArray<JobHandle>(planetQueries.Count, Allocator.TempJob);

        //runs all planet queries in parallel
        for (int i = 0; i < planetDatas.Count; i++)
        {
            CreatePlanetAsync(planetDatas[i], weatherConditions[i]);
        }

        // Debug.Log("start finised");
    }

    async void CreatePlanetAsync(Planet.TransformData transformData, Planet.GlobalWeatherConditions globalWeatherConditions)
    {
        Planet planet = Instantiate(planetPrefab, Vector3.zero, Quaternion.identity);
        planet.SetTransformData(transformData);
        Planets.Add(planet);
        
        //generate ico
        Icosahedron ico = Icosahedron.GenerateIcoSphere(planetResolution);
        NativeArray<float> noiseValues = new NativeArray<float>(ico.Vertices.Length, Allocator.TempJob);
        NativeArray<Vector3> vertices = ico.Vertices;
        
        //compute and apply elevation
        Noise3DMapJob job = new Noise3DMapJob(planetElevationNoiseSettings, noiseValues, vertices);
        await Task.Run(() => GenerateNoise(job));
        for (int i = 0; i < ico.Vertices.Length; i++) ico.Vertices[i] *= (1 + noiseValues[i]);

        Planet.MeshData meshData = new Planet.MeshData(ico, noiseValues.ToArray());
        planet.GenerateMesh(meshData);

        // Debug.Log("Planet instance created");

        //compute biome and weather values
        Planet.VertexWeatherConditions vertexWeatherConditions =
            BiomeGenerator.Instance.GeneratePlanetBiomes(globalWeatherConditions, meshData);
        planet.SetVertexWeatherConditions(vertexWeatherConditions);
        planet.SetGlobalWeatherConditions(globalWeatherConditions);

        Debug.Log("Planet Generated");
    }

    Task GenerateNoise(Noise3DMapJob job)
    {
        job.Execute();
        // Debug.Log("task complete");
        // JobHandle handle = job.Schedule();
        // handle.Complete();
        return Task.CompletedTask;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, planetDistanceRange.min);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, planetDistanceRange.max);
    }
}