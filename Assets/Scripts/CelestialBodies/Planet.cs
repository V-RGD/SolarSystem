using System;
using MeshGeneration;
using UnityEditor;
using UnityEngine;

namespace CelestialBodies
{
    public class Planet : CelestialBody
    {
        [SerializeField] bool randomizeEclipticPos;
        [SerializeField] MeshFilter waterMeshFilter;
        [SerializeField] MeshFilter atmosphereMeshFilter;

        [Header("Atmosphere")] 
        [SerializeField] Material lightAtmosphereMat;
        [SerializeField] Material mediumAtmosphereMat;
        [SerializeField] Material denseAtmosphereMat;

        MeshFilter _terrainFilter;
        // MeshFilter _waterFilter;

        MeshData _meshData;
        GlobalWeatherConditions _globalWeatherConditions;
        VertexWeatherConditions _vertexWeatherConditions;
        TransformData _transformData;

        const float OrbitalPeriodDimmer = 1;

        void Awake()
        {
            _terrainFilter = GetComponent<MeshFilter>();
        }

        public void SetTransformData(TransformData data)
        {
            _transformData = data;

            //init values
            Tilt = data.Tilt;
            RotationSpeed = data.RotationSpeed;
            OrbitalDistance = data.Distance;
            OrbitalPeriod = Mathf.Sqrt(Mathf.Pow((data.Distance / OrbitalPeriodDimmer), 3));

            if (randomizeEclipticPos) CurrentOrbitAngle = SRnd.RangeFloat(0, 360);

            transform.localScale = Vector3.one * data.Size;
        }

        public void GenerateMesh(MeshData meshData)
        {
            _meshData = meshData;
            _terrainFilter.sharedMesh = meshData.Ico.ToMesh(false);
        }

        public void SetGlobalWeatherConditions(GlobalWeatherConditions globalWeatherConditions)
        {
            _globalWeatherConditions = globalWeatherConditions;

            MeshRenderer meshRenderer = atmosphereMeshFilter.GetComponent<MeshRenderer>();

            float a = globalWeatherConditions.AtmosphericDensity;

            if (a < 0.25f) meshRenderer.enabled = false;
            else if (a < 0.5f) meshRenderer.sharedMaterial = lightAtmosphereMat;
            else if (a < 0.75f) meshRenderer.sharedMaterial = mediumAtmosphereMat;
            else if (a <= 1) meshRenderer.sharedMaterial = denseAtmosphereMat;
        }

        public void SetVertexWeatherConditions(VertexWeatherConditions weatherConditions)
        {
            _vertexWeatherConditions = weatherConditions;
            ApplyVertexColor();
        }

        void ApplyVertexColor()
        {
            int vertexCount = _terrainFilter.sharedMesh.vertices.Length;
            Color[] vertexColors = new Color[vertexCount];

            //applies biome color to each vertex
            for (int i = 0; i < vertexCount; i++)
            {
                int biomeIndex = _vertexWeatherConditions.BiomeIndices[i];
                if (biomeIndex == int.MaxValue) throw new NotImplementedException(); //error
                Biome biome =  BiomeGenerator.Instance.Biomes[biomeIndex];
                vertexColors[i] = biome.vertexColor;

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

            _terrainFilter.sharedMesh.colors = vertexColors;
        }

        //generates water
        // _waterMesh = Icosahedron.GenerateIcoSphere(resolution);
        // waterMeshFilter.sharedMesh = _waterMesh.ToMesh();
        // waterMeshFilter.transform.localScale = Vector3.one;
        // waterMeshFilter.transform.localScale = Vector3.one * _generationData.PlanetHumidity * _planetScale;

        void OnDrawGizmos()
        {
            return;
            if (_terrainFilter == null) return;
            if (_terrainFilter.sharedMesh != null)
            {
                for (int i = 0; i < _terrainFilter.sharedMesh.vertices.Length; i++)
                {
                    var vert = _terrainFilter.sharedMesh.vertices[i];
                    Handles.Label(vert, vert.ToString());
                }
            }
        }

        public struct TransformData
        {
            //planet object settings
            public float Size;
            public float Distance;
            public float Tilt;
            public float RotationSpeed;

            public TransformData(float size, float distance, float tilt, float rotationSpeed)
            {
                Size = size;
                Distance = distance;
                Tilt = tilt;
                RotationSpeed = rotationSpeed;
            }
        }

        public struct MeshData
        {
            public MeshData(Icosahedron ico, float[] displacements)
            {
                Ico = ico;
                Displacements = displacements;
            }

            //returns data to construct mesh
            public Icosahedron Ico;
            public float[] Displacements;
        }

        public struct VertexWeatherConditions
        {
            public VertexWeatherConditions(int vertexCount)
            {
                VertexCount = vertexCount;
                Elevations = new float[vertexCount];
                Temperatures = new float[vertexCount];
                Humidity = new float[vertexCount];
                BiomeIndices = new int[vertexCount];
            }

            public int VertexCount;

            //data to create biomes
            public float[] Elevations;
            public float[] Temperatures;
            public float[] Humidity;
            public int[] BiomeIndices;
        }

        public struct GlobalWeatherConditions
        {
            //biome conditions
            public float GlobalHumidity;
            public float GlobalTemperature;
            public float AtmosphericDensity;
            public float MaxElevation;

            public GlobalWeatherConditions(float t, float h, float atmosphericDensity, float maxElevation)
            {
                GlobalTemperature = t;
                MaxElevation = maxElevation;
                AtmosphericDensity = atmosphericDensity;
                GlobalHumidity = h;
            }
        }
    }
}