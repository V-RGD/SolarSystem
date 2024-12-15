using System.Threading.Tasks;
using JobQueries;
using MeshGeneration;
using Unity.Collections;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CelestialBodies
{
    /// <summary>
    /// Base class for planets that have a solid ground, an atmosphere (optional), and oceans (optional)
    /// </summary>
    public class TerrestrialPlanet : Planet
    {
        [SerializeField] MeshFilter terrainFilter;
        [SerializeField] MinMaxValue sizeRange;

        public override async Task InitialisePlanet()
        {
            transform.localScale = sizeRange.Lerp(SRnd.NextFloat()) * Vector3.one;
            
            Icosahedron ico = await Task.Run((GenerateTerrainWithJobs));
            
            terrainFilter.sharedMesh = ico.ToMesh(false);
        }

        async Task<Icosahedron> GenerateTerrainWithJobs()
        {
            FlashClock duration = new FlashClock();
            duration.Start();
            
            Icosahedron ico = await GenerateSphere(PlanetGenerator.Instance.PlanetResolution);
            Debug.Log($"Sphere Creation : {duration.FlashReset()}");

            ico.Vertices = await GenerateTerrainNoise(ico, duration);
            Debug.Log($"Array Set : {duration.FlashReset()}");
            
            return ico;
        }

        async Task<NativeArray<Vector3>> GenerateTerrainNoise(Icosahedron ico, FlashClock duration)
        {
            Debug.Log($"A : {duration.FlashReset()}");
            //divides vertices in chunks
            NativeArrayDivider<Vector3> vertexChunks = new NativeArrayDivider<Vector3>(ico.Vertices);
            int chunks = Mathf.CeilToInt(Mathf.Sqrt(ico.Vertices.Length));
            vertexChunks.DivideIntensiveTask(chunks);
            Debug.Log($"B : {duration.FlashReset()}");
            
            //creates noise jobs for each
            NativeArray<Compute3DNoiseJob> noiseJobs = new NativeArray<Compute3DNoiseJob>(chunks, Allocator.TempJob);
            for (int i = 0; i < chunks; i++)
            {
                //create job
                noiseJobs[i] = new Compute3DNoiseJob(PlanetGenerator.Instance.PlanetTerrainSettings, vertexChunks.Chunks[i]);
            }
            
            Debug.Log($"C : {duration.FlashReset()}");
            
            //computes noise
            noiseJobs.ExecuteAll();
            
            Debug.Log($"Noise Compute : {duration.FlashReset()}");

            NativeArray<Vector3> newVerts = new NativeArray<Vector3>(ico.Vertices.Length, Allocator.TempJob);
            int index = 0;
            for (int i = 0; i < chunks; i++)
            {
                //chunk verts
                foreach (Vector3 vertex in noiseJobs[i].Positions)
                {
                    newVerts[index] = vertex;
                    index++;
                }
            }
            
            return newVerts;
        }

        async Task<Icosahedron> GenerateSphere(int resolution)
        {
            return await Task.Run((() => Icosahedron.GenerateIcoSphere(resolution)));
        }


        // async Task<Icosahedron> GenerateTerrainAsync()
        // {
        //     Stopwatch stopwatch = new Stopwatch();
        //     stopwatch.Start();
        //     
        //     Icosahedron ico = await Task.Run((() => Icosahedron.GenerateIcoSphere(PlanetGenerator.Instance.PlanetResolution)));
        //     
        //     stopwatch.Stop();
        //     Debug.Log(stopwatch.ElapsedMilliseconds + "to create sphere");
        //     stopwatch.Reset();
        //     stopwatch.Start();
        //     
        //     Noise3DMapJob job = new Noise3DMapJob(PlanetGenerator.Instance.PlanetTerrainSettings, 
        //         new NativeArray<float>(ico.Vertices.Length, Allocator.TempJob), 
        //         ico.Vertices);
        //     job.Execute();
        //     
        //     stopwatch.Stop();
        //     Debug.Log(stopwatch.ElapsedMilliseconds + "to compute noise");
        //     
        //     ico.Vertices = job.Positions;
        //     return ico;
        // }
    }
}
