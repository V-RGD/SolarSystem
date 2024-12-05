// using System.Collections.Generic;
// using Unity.Collections;
// using Unity.Jobs;
// using UnityEngine;
//
// namespace JobQueries
// {
//     public class PlanetCreator
//     {
//         public void CreatePlanets()
//         {
//             //setup planets settings
//             List<PlanetQuery> playerQueries = new List<PlanetQuery>();
//             
//             float lastPlanetDist = 0;
//             float lastPlanetSize = sunSize;
//             
//             for (int i = 0; i < planetsToSpawn; i++)
//             {
//                 float planetSize = Random.Range(planetSizeRange.min, planetSizeRange.max);
//             
//                 //puts it at a safe distance from the last planet (or sun, if none)
//                 float distance = lastPlanetDist + (distanceRatioBetweenPlanets.RandomValue() * lastPlanetSize);
//                 lastPlanetDist = distance;
//             
//                 float tilt = planetTiltRepartition.Evaluate(Random.value) * 90;
//                 if (SRnd.NextBool()) tilt *= -1;
//             
//                 float rotationSpeed = Mathf.Lerp(planetRotationSpeed.min, planetRotationSpeed.max,
//                     planetRotationSpeedRepartition.Evaluate(SRnd.NextFloat()));
//             
//                 float t = planetTemperatureByDistance.Evaluate(Mathf.InverseLerp(0, distForZeroTemperature, distance));
//             
//                 PlanetCreationJob newJob = new PlanetCreationJob(planetSize, distance, tilt, rotationSpeed, t,
//                     planetResolution, planetElevationMap, planetPrefab);
//                 planetCreationJobs.Add(newJob);
//             
//                 lastPlanetSize = planetSize;
//             }
//             
//             //compute data
//             
//             
//             //instantiates planets on job complete
//         }
//     }
//     
//     public struct PlanetQuery : IJobParallelFor
//     {
//         //results of the job (every data needed for the planet)
//         public NativeArray<PlanetVertexData> PlanetData;
//         
//         public float GlobalHumidity;
//         public float GlobalTemperature;
//         public float AtmosphericDensity;
//         public float MaxElevation;
//         
//         //parameters (every data needed to create the planet)
//         
//         public void Execute(int index)
//         {
//             //for each vertex, does a parallel job to compute its values
//             
//             //create ico sphere
//             
//             //compute vertices values
//         }
//     }
//
//     public struct PlanetVertexQuery : IJobParallelFor
//     {
//         
//         //returns all the data that a planet vertex should have
//         public NativeArray<PlanetVertexData> output;
//         
//         public void Execute(int index)
//         {
//             
//         }
//     }
//     
//     public struct PlanetVertexData
//     {
//         public Vector3 Position;
//         public float Displacement;
//         public float Elevation;
//         public float Temperature;
//         public float Humidity;
//         public int BiomeIndex;
//     }
// }