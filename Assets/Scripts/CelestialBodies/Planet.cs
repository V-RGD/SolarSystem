using Generation;
using MeshGeneration;
using UnityEditor;
using UnityEngine;

namespace CelestialBodies
{
    public class Planet : CelestialBody
    {
        [SerializeField] MeshFilter waterMeshFilter;
        Icosahedron _waterMesh;

        MeshFilter _meshFilter;
        Icosahedron _surfaceMesh;
        PlanetGenerationData _generationData;

        public NoiseMapParams ElevationParams { get; set; }
        const float OrbitalPeriodDimmer = 1;
        float _planetScale;

        [SerializeField] bool randomizeEclipticPos;

        void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        public void Init(float planetSize, float distance, float tilt, float rotationSpeed)
        {
            //init values
            Tilt = tilt;
            RotationSpeed = rotationSpeed;
            if(randomizeEclipticPos) CurrentOrbitAngle = SRnd.RangeFloat(0, 360);
            OrbitalDistance = distance;

            // if (gravitationalLock) OrbitalPeriod = OrbitalDistance * 2 * Mathf.PI / OrbitalPeriodDimmer;
            OrbitalPeriod = Mathf.Sqrt(Mathf.Pow((distance / OrbitalPeriodDimmer), 3));
            
            //set planet size
            _planetScale = planetSize;
            transform.localScale = Vector3.one * planetSize;
        }

        public void GeneratePlanet(int resolution, float temperature)
        {
            _surfaceMesh = Icosahedron.GenerateIcoSphere(resolution);

            GenerateValues(temperature);

            //generates elevation
            PlanetMesh.GenerateElevation(_surfaceMesh, _generationData.VertexDisplacement);
            _meshFilter.sharedMesh = _surfaceMesh.ToMesh();

            //apply vertex colors
            ApplyVertexColor();
            
            // _meshFilter.sharedMesh.RecalculateNormals();
            // _meshFilter.sharedMesh.RecalculateTangents();

            return;
            //generates water
            _waterMesh = Icosahedron.GenerateIcoSphere(resolution);
            waterMeshFilter.sharedMesh = _waterMesh.ToMesh();
            waterMeshFilter.transform.localScale = Vector3.one;
            // waterMeshFilter.transform.localScale = Vector3.one * _generationData.PlanetHumidity * _planetScale;
        }

        void GenerateValues(float temperature)
        {
            //generate elevation
            _generationData = new PlanetGenerationData(_surfaceMesh.Vertices);
            _generationData.VertexDisplacement = SphereMapUtilities.SampleMap(_generationData.VertexPositions, ElevationParams);
            _generationData.MaxElevation = ElevationParams.multiplier;

            //temperature
            _generationData.PlanetTemperature = temperature;
            _generationData.PlanetHumidity = SRnd.RangeFloat(0f, 1f);
            _generationData.AtmosphericDensity = SRnd.RangeFloat(0f, 1f);
            _generationData.Biomes = BiomeGenerator.Instance.GeneratePlanetBiomes(_generationData);
        }
        
        void ApplyVertexColor()
        {
            int vertexCount = _meshFilter.sharedMesh.vertices.Length;
            Color[] vertexColors = new Color[vertexCount];

            //applies biome color to each vertex
            for (int i = 0; i < vertexCount; i++)
            {
                vertexColors[i] = _generationData.Biomes.Map[i].Value.vertexColor;

                // //for debug
                // float r = _generationData.Temperature.Map[i].Value;
                // float g = 0;
                // float b = _generationData.Humidity.Map[i].Value;
                // float a = _generationData.Height.Map[i].Value;
                //
                // b = 0;
                // a = 1;
                //
                // Vector3 c = new Vector3(r, g, b);
                // vertexColors[i] = new Color(c.x, c.y, c.z) * a;
            }

            _meshFilter.sharedMesh.colors = vertexColors;
        }

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