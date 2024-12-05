using System.Collections.Generic;
using CelestialBodies;
using Generation;
using MeshGeneration;
using NaughtyAttributes;
using TerrainGeneration;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Used to generate sun and planets
/// </summary>
public class SolarSystemGenerator : MonoBehaviour
{
    [Header("Solar System Settings")] [SerializeField]
    float sunSize = 2;

    [SerializeField] int planetsToSpawn = 10;
    [SerializeField] float distForZeroTemperature = 50;

    [SerializeField] MinMaxValue planetSizeRange;
    [SerializeField] MinMaxValue distanceRatioBetweenPlanets;

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
    List<Planet> _planets = new List<Planet>();

    [SerializeField, Range(0f, 1f)] float timeScale = 1;
    
    void Start()
    {
        GenerateSun(sunSize);
        GeneratePlanets();
    }

    void Update() => Time.timeScale = timeScale;

    void GenerateSun(float size) => sun.SetSize(size);

    public struct PlanetData
    {
        //planet object settings
        public float Size;
        public float Distance;
        public float Tilt;
        public float RotationSpeed;

        public PlanetData(float size, float distance, float tilt, float rotationSpeed)
        {
            Size = size;
            Distance = distance;
            Tilt = tilt;
            RotationSpeed = rotationSpeed;
        }
    }

    void GeneratePlanets()
    {
        //setup planets settings
        List<PlanetCreationJob> planetQueries = new List<PlanetCreationJob>();
        List<PlanetData> planetDatas = new List<PlanetData>();

        PlanetCreationJob.TerrainSettings terrainSettings =
            new PlanetCreationJob.TerrainSettings(planetResolution, planetElevationNoiseSettings);

        float lastPlanetDist = 0;
        float lastPlanetSize = sunSize;

        for (int i = 0; i < planetsToSpawn; i++)
        {
            float size = Random.Range(planetSizeRange.min, planetSizeRange.max);

            //puts it at a safe distance from the last planet (or sun, if none)
            float distance = lastPlanetDist + (distanceRatioBetweenPlanets.RandomValue() * lastPlanetSize);

            float tilt = planetTiltRepartition.Evaluate(Random.value) * 90;
            if (SRnd.NextBool()) tilt *= -1;

            float rotationSpeed = Mathf.Lerp(planetRotationSpeed.min, planetRotationSpeed.max,
                planetRotationSpeedRepartition.Evaluate(SRnd.NextFloat()));
            
            //adds planet generation data
            planetDatas.Add(new PlanetData(size, distance, tilt, rotationSpeed));

            float t = planetTemperatureByDistance.Evaluate(Mathf.InverseLerp(0, distForZeroTemperature, distance));

            PlanetCreationJob.GlobalConditions conditions = new PlanetCreationJob.GlobalConditions(t, SRnd.NextFloat(),
                SRnd.NextFloat(), planetElevationNoiseSettings.multiplier);

            PlanetCreationJob.OutputData outputData = new PlanetCreationJob.OutputData();

            PlanetCreationJob newJob = new PlanetCreationJob(terrainSettings, conditions, outputData);
            planetQueries.Add(newJob);

            lastPlanetSize = size;
            lastPlanetDist = distance;
        }

        NativeArray<JobHandle> handles = new NativeArray<JobHandle>(planetQueries.Count, Allocator.TempJob);
        
        //runs all planet queries in parallel
        for (int i = 0; i < planetQueries.Count; i++)
        {
            // handles[i] = planetQueries[i].Schedule();
            planetQueries[i].Execute();
        }
        
        JobHandle.CompleteAll(handles);
        
        // Debug.Log("all planets are created");
        
        //then creates the planets
        for (int i = 0; i < planetQueries.Count; i++)
        {
            Planet planet = Instantiate(planetPrefab, Vector3.zero, Quaternion.identity);
            planet.Init(planetDatas[i]);
            planet.GeneratePlanet(planetQueries[i].Output[0]);
        }
    }
}