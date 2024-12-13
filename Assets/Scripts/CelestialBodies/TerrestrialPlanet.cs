using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using JobQueries;
using MeshGeneration;
using Unity.Collections;
using Unity.Jobs;
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
        // [SerializeField] MeshRenderer waterRenderer;
        // [SerializeField] MeshRenderer atmosphereRenderer;

        public override async Task InitialisePlanet()
        {
            transform.localScale = sizeRange.Lerp(SRnd.NextFloat()) * Vector3.one;

            int verticesCount = Mathf.CeilToInt(20 * Mathf.Pow(4, PlanetGenerator.Instance.PlanetResolution));
            int chunks = Mathf.CeilToInt(Mathf.Sqrt(verticesCount));
            
            Icosahedron ico = await Task.Run((() => GenerateTerrainWithJobs(chunks)));
            // Icosahedron ico = await Task.Run((() => GenerateTerrainAsync()));
            terrainFilter.sharedMesh = ico.ToMesh(false);
        }

        async Task<Icosahedron> GenerateTerrainWithJobs(int chunks)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            Icosahedron ico = await Task.Run((() => Icosahedron.GenerateIcoSphere(PlanetGenerator.Instance.PlanetResolution)));
            
            stopwatch.Stop();
            Debug.Log(stopwatch.ElapsedMilliseconds + "to create sphere");
            stopwatch.Reset();
            stopwatch.Start();

            int chunkSize = (int)ico.Vertices.Length / chunks;

            NativeArray<ComputeNoiseValuesJob> jobs = new NativeArray<ComputeNoiseValuesJob>(chunkSize, Allocator.TempJob);
            
            for (int i = 0; i < chunks; i++)
            {
                List<Vector3> verts = new List<Vector3>();
                
                //chunk verts
                for (int j = 0; j < chunkSize; j++)
                {
                    int index = i * chunkSize + j;
                    if(index >= ico.Vertices.Length) break;
                    verts.Add(ico.Vertices[index]);
                }
                
                //create job
                jobs[i] = new ComputeNoiseValuesJob(PlanetGenerator.Instance.PlanetTerrainSettings, new NativeArray<float>(verts.Count, Allocator.TempJob), 
                    verts.ToNativeArray(Allocator.TempJob));
            }

            NativeArray<JobHandle> handles = new NativeArray<JobHandle>(chunks, Allocator.TempJob);

            for (int i = 0; i < handles.Length; i++)
            {
                handles[i] = jobs[i].Schedule();
            }
            
            JobHandle.CompleteAll(handles);
            
            stopwatch.Stop();
            Debug.Log(stopwatch.ElapsedMilliseconds + "to compute noise");
            stopwatch.Reset();
            stopwatch.Start();

            NativeArray<Vector3> elevatedVerts = new NativeArray<Vector3>(ico.Vertices.Length, Allocator.TempJob);
            
            for (int i = 0; i < chunks; i++)
            {
                //chunk verts
                for (int j = 0; j < chunkSize; j++)
                {
                    int index = i * chunkSize + j;
                    if(index >= ico.Vertices.Length) break;

                    elevatedVerts[index] = jobs[i].Positions[j];
                }
            }
            
            stopwatch.Stop();
            Debug.Log(stopwatch.ElapsedMilliseconds + "to set array");
            
            ico.Vertices = elevatedVerts;
            return ico;
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
