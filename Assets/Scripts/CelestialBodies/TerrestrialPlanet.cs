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
        [SerializeField] Gradient colorGradient;

        public override async Task InitialisePlanet()
        {
            transform.localScale = sizeRange.Lerp(SRnd.NextFloat()) * Vector3.one;

            Icosahedron ico = await Task.Run((GenerateTerrain));
            
            ico.ReshadeFlat();
            Mesh m = ico.ToMesh(true);
            terrainFilter.sharedMesh = m;
        }

        async Task<Icosahedron> GenerateTerrain()
        {
            FlashClock duration = new FlashClock();
            duration.Start();

            Icosahedron ico = await GenerateSphere(PlanetGenerator.Instance.PlanetResolution);
            Debug.Log($"Sphere Creation : {duration.FlashReset()}");

            ico.Vertices = await GenerateTerrainNoise(ico, duration);
            ico.Colors = Colors;

            return ico;
        }

        NativeArray<Color> Colors;

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
            Colors = new NativeArray<Color>(ico.Vertices.Length, Allocator.TempJob);
            
            int index = 0;
            for (int i = 0; i < chunks; i++)
            {
                //batch verts
                for (int j = 0; j < noiseJobs[i].Positions.Length; j++)
                {
                    newVerts[index] = noiseJobs[i].Positions[j];
                    Colors[index] = GetElevationColor(noiseJobs[i].NoiseValues[j]);
                    index++;
                }
            }

            Debug.Log($"Array Set : {duration.FlashReset()}");
            return newVerts;
        }

        async Task<Icosahedron> GenerateSphere(int resolution)
        {
            return await Task.Run((() => Icosahedron.GenerateIcoSphere(resolution)));
        }

        public Color GetElevationColor(float noiseValue)
        {
            float scalar = 1 / PlanetGenerator.Instance.PlanetTerrainSettings.multiplier;
            float value = scalar * noiseValue + 0.5f;
            return colorGradient.Evaluate(value);
        }
    }
}
