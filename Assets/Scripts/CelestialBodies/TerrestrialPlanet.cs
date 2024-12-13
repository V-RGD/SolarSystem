using System.Threading.Tasks;
using JobQueries;
using MeshGeneration;
using Unity.Collections;
using UnityEngine;

namespace CelestialBodies
{
    /// <summary>
    /// Base class for planets that have a solid ground, an atmosphere (optional), and oceans (optional)
    /// </summary>
    public class TerrestrialPlanet : Planet
    {
        [SerializeField] MeshFilter terrainFilter;
        // [SerializeField] MeshRenderer waterRenderer;
        // [SerializeField] MeshRenderer atmosphereRenderer;
        [SerializeField] MinMaxValue sizeRange;

        public override async Task InitialisePlanet()
        {
            transform.localScale = sizeRange.Lerp(SRnd.NextFloat()) * Vector3.one;
            await GenerateTerrain();
        }

        Task GenerateTerrain()
        {
            //generate terrain
            Icosahedron ico = Icosahedron.GenerateIcoSphere(PlanetGenerator.Instance.PlanetResolution);
            
            Noise3DMapJob job = new Noise3DMapJob(PlanetGenerator.Instance.PlanetTerrainSettings, new NativeArray<float>(ico.Vertices.Length, Allocator.TempJob),
                ico.Vertices);
            job.Execute();
            
            ico.Vertices = job.Positions;
            
            terrainFilter.sharedMesh = ico.ToMesh(false);
            
            return Task.CompletedTask;
        }
    }
}
