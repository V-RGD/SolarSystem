using System.Collections.Generic;
using CelestialBodies;
using Generation;
using NaughtyAttributes;
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

    [SerializeField] NoiseMapParams planetElevationMap;

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

    // void GeneratePlanets()
    // {
    //     //setup planets settings
    //     List<PlanetQuery> playerQueries = new List<PlanetQuery>();
    //         
    //     float lastPlanetDist = 0;
    //     float lastPlanetSize = sunSize;
    //         
    //     for (int i = 0; i < planetsToSpawn; i++)
    //     {
    //         float planetSize = Random.Range(planetSizeRange.min, planetSizeRange.max);
    //         
    //         //puts it at a safe distance from the last planet (or sun, if none)
    //         float distance = lastPlanetDist + (distanceRatioBetweenPlanets.RandomValue() * lastPlanetSize);
    //         lastPlanetDist = distance;
    //         
    //         float tilt = planetTiltRepartition.Evaluate(Random.value) * 90;
    //         if (SRnd.NextBool()) tilt *= -1;
    //         
    //         float rotationSpeed = Mathf.Lerp(planetRotationSpeed.min, planetRotationSpeed.max,
    //             planetRotationSpeedRepartition.Evaluate(SRnd.NextFloat()));
    //         
    //         float t = planetTemperatureByDistance.Evaluate(Mathf.InverseLerp(0, distForZeroTemperature, distance));
    //         
    //         PlanetQuery newJob = new PlanetQuery();
    //         newJob.
    //         
    //         playerQueries.Add(newJob);
    //         
    //         lastPlanetSize = planetSize;
    //     }
    // }

    void GeneratePlanets()
    {
        List<PlanetCreationJob> planetCreationJobs = new List<PlanetCreationJob>();
    
        float lastPlanetDist = 0;
        float lastPlanetSize = sunSize;
    
        for (int i = 0; i < planetsToSpawn; i++)
        {
            float planetSize = Random.Range(planetSizeRange.min, planetSizeRange.max);
    
            //puts it at a safe distance from the last planet (or sun, if none)
            float distance = lastPlanetDist + (distanceRatioBetweenPlanets.RandomValue() * lastPlanetSize);
            lastPlanetDist = distance;
    
            float tilt = planetTiltRepartition.Evaluate(Random.value) * 90;
            if (SRnd.NextBool()) tilt *= -1;
    
            float rotationSpeed = Mathf.Lerp(planetRotationSpeed.min, planetRotationSpeed.max,
                planetRotationSpeedRepartition.Evaluate(SRnd.NextFloat()));
    
            float t = planetTemperatureByDistance.Evaluate(Mathf.InverseLerp(0, distForZeroTemperature, distance));
    
            PlanetCreationJob newJob = new PlanetCreationJob(planetSize, distance, tilt, rotationSpeed, t,
                planetResolution, planetElevationMap, planetPrefab);
            planetCreationJobs.Add(newJob);
    
            lastPlanetSize = planetSize;
        }
        
        foreach (var job in planetCreationJobs)
        {
            job.Execute();
            // JobHandle handle = job.Schedule();
        }
    }
    
    public class PlanetCreationJob : IJob
    {
        public float PlanetSize;
        public float Distance;
        public float Tilt;
        public float RotationSpeed;
        public float Temperature;
        public int Resolution;
    
        public PlanetCreationJob(float planetSize, float distance, float tilt, float rotationSpeed, float temperature,
            int resolution, NoiseMapParams elevationParams, Planet planetPrefab)
        {
            PlanetSize = planetSize;
            Distance = distance;
            Tilt = tilt;
            RotationSpeed = rotationSpeed;
            this.ElevationParams = elevationParams;
            this.PlanetPrefab = planetPrefab;
            Temperature = temperature;
            Resolution = resolution;
        }
        
        public Planet PlanetPrefab;
        public NoiseMapParams ElevationParams;
    
        public void Execute()
        {
            Planet planet = Instantiate(PlanetPrefab, Vector3.zero, Quaternion.identity);
            planet.Init(PlanetSize, Distance, Tilt, RotationSpeed);
            planet.ElevationParams = ElevationParams;
            planet.GeneratePlanet(Resolution, Temperature);
            Debug.Log("job completed");
        }
    }
}