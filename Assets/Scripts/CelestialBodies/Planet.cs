using Generation;
using MeshGeneration;
using UnityEditor;
using UnityEngine;

namespace CelestialBodies
{
    public class Planet : CelestialBody
    {
        [SerializeField] bool randomizeEclipticPos;
        [SerializeField] MeshFilter waterMeshFilter;
        Icosahedron _waterMesh;

        MeshFilter _meshFilter;
        Icosahedron _surfaceMesh;

        const float OrbitalPeriodDimmer = 1;

        

        void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        public void Init(SolarSystemGenerator.PlanetData data)
        {
            //init values
            Tilt = data.Tilt;
            RotationSpeed = data.RotationSpeed;
            OrbitalDistance = data.Distance;
            OrbitalPeriod = Mathf.Sqrt(Mathf.Pow((data.Distance / OrbitalPeriodDimmer), 3));
            
            if(randomizeEclipticPos) CurrentOrbitAngle = SRnd.RangeFloat(0, 360);
            
            transform.localScale = Vector3.one * data.Size;
        }

        public void GeneratePlanet(PlanetCreationJob.OutputData outputData)
        {
            _meshFilter.sharedMesh = outputData.Ico.ToMesh();
            // GenerateValues(temperature);
            // ApplyVertexColor();
        }
        
        // void ApplyVertexColor()
        // {
        //     int vertexCount = _meshFilter.sharedMesh.vertices.Length;
        //     Color[] vertexColors = new Color[vertexCount];
        //
        //     //applies biome color to each vertex
        //     for (int i = 0; i < vertexCount; i++)
        //     {
        //         vertexColors[i] = _generationData.Biomes.Map[i].Value.vertexColor;
        //
        //         // //for debug
        //         // float r = _generationData.Temperature.Map[i].Value;
        //         // float g = 0;
        //         // float b = _generationData.Humidity.Map[i].Value;
        //         // float a = _generationData.Height.Map[i].Value;
        //         //
        //         // b = 0;
        //         // a = 1;
        //         //
        //         // Vector3 c = new Vector3(r, g, b);
        //         // vertexColors[i] = new Color(c.x, c.y, c.z) * a;
        //     }
        //
        //     _meshFilter.sharedMesh.colors = vertexColors;
        // }
        
        //generates water
        // _waterMesh = Icosahedron.GenerateIcoSphere(resolution);
        // waterMeshFilter.sharedMesh = _waterMesh.ToMesh();
        // waterMeshFilter.transform.localScale = Vector3.one;
        // waterMeshFilter.transform.localScale = Vector3.one * _generationData.PlanetHumidity * _planetScale;

        void OnDrawGizmos()
        {
            return;
            if (_meshFilter == null) return;
            if (_meshFilter.sharedMesh != null)
            {
                for (int i = 0; i < _meshFilter.sharedMesh.vertices.Length; i++)
                {
                    var vert = _meshFilter.sharedMesh.vertices[i];
                    Handles.Label(vert, vert.ToString());
                }
            }
        }
    }
}