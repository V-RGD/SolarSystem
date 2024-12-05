using Generation;
using MeshGeneration;
using Unity.Collections;

public struct PlanetCreationJob 
{
    // public PlanetCreationJob(TerrainSettings terrainSettings, GlobalConditions globalConditions)
    // {
    //     InputNoiseSettings = new NativeReference<TerrainSettings>(terrainSettings, Allocator.TempJob);
    //     InputGlobalConditions = new NativeReference<GlobalConditions>(globalConditions, Allocator.TempJob);
    //     Displacements = new NativeArray<float>();
    //     Ico = new NativeReference<Icosahedron>(Allocator.TempJob);
    // }
    //
    // public NativeReference<TerrainSettings> InputNoiseSettings;
    // public NativeReference<GlobalConditions> InputGlobalConditions;
    //
    // //parameters
    // public struct TerrainSettings
    // {
    //     public int Resolution;
    //     public NoiseMapSettings Settings;
    //
    //     public TerrainSettings(int resolution, NoiseMapSettings settings)
    //     {
    //         Resolution = resolution;
    //         Settings = settings;
    //     }
    // }
    //
    // public struct GlobalConditions
    // {
    //     //biome conditions
    //     public float GlobalHumidity;
    //     public float GlobalTemperature;
    //     public float AtmosphericDensity;
    //     public float MaxElevation;
    //
    //     public GlobalConditions(float t, float h, float atmosphericDensity, float maxElevation)
    //     {
    //         GlobalTemperature = t;
    //         MaxElevation = maxElevation;
    //         AtmosphericDensity = atmosphericDensity;
    //         GlobalHumidity = h;
    //     }
    // }
    
    // public struct Output
    // {
    //     //returns data to construct mesh
    //     public NativeReference<Icosahedron> Ico;
    //     public NativeArray<float> Displacements;
    //     // public float[] ElevationRatios;
    //     // public float[] Temperatures;
    //     // public float[] Humidity;
    //     // public int[] BiomeIndices;
    // }
    //
    // public void Execute()
    // {
    //     Icosahedron ico = Icosahedron.GenerateIcoSphere(InputNoiseSettings.Value.Resolution);
    //     
    //     //generate vertices
    //     
    //     //compute noise values (heaviest operations)
    //     Displacements = ComputeNoiseValues(ico);
    //     for (int i = 0; i < Ico.Value.Vertices.Length; i++) ico.Vertices[i] *= (1 + Displacements[i]);
    //
    //     //ComputeVertexValues();
    //     Ico.Value = ico;
    // }

    // NativeArray<float> ComputeNoiseValues(Icosahedron ico)
    // {
    //     NoiseMapSettings settings = InputNoiseSettings.Value.Settings;
    //     int arrayLength = ico.Vertices.Length;
    //     Vector3 vert;
    //     float value;
    //     
    //     NativeArray<float> newMap = new NativeArray<float>(arrayLength, Allocator.TempJob);
    //
    //     for (int i = 0; i < arrayLength; i++)
    //     {
    //         vert = ico.Vertices[i];
    //         value = NoiseMapSampler.SampleNoise3D(vert.x, vert.y, vert.z, settings.freq, settings.octaves,
    //             settings.seed);
    //         newMap[i] = value * settings.multiplier;
    //     }
    //     
    //     return newMap;
    // }
}

// void ComputeVertexValues()
// {
//     PlanetJobInput input = Input[0];
//     PlanetJobOutput output = Output[0];
//     int arrayLength = output.Vertices.Length;
//
//     for (int i = 0; i < arrayLength; i++)
//     {
//         Vector3 vertex = output.Vertices[i];
//         float latitude = Mathf.Abs(vertex.y);
//
//         //compute base vertex values
//         float e = output.Displacements[i] / input.MaxElevation + 0.5f;
//         float t = input.GlobalTemperature;
//         float h = input.GlobalHumidity;
//         h = 0; //test
//
//         //elevation boosts cold temperatures if high and hot ones if low
//         t *= temperatureByAltitude.Evaluate(e);
//         // h *= temperatureByAltitude.Evaluate(e);
//
//         //latitude reduces temperature at poles
//         t *= temperatureByLatitude.Evaluate(latitude);
//         // h *= temperatureByLatitude.Evaluate(latitude);
//
//         //temperature reduces humidity
//         //h *= humidityByTemperature.Evaluate(t);
//
//         //for each vertex, computes biome that needs to be assigned
//         biome = GetClosestBiome(t, h);
//
//         //registers vertex data
//         output.BiomeIndices[i] = biome;
//         
//         biomeMap.Map[i] = new KeyValuePair<Vector3, Biome>(vertex, biome);
//         data.Height.Map[i] = new KeyValuePair<Vector3, float>(vertex, e);
//         data.Temperature.Map[i] = new KeyValuePair<Vector3, float>(vertex, t);
//         data.Humidity.Map[i] = new KeyValuePair<Vector3, float>(vertex, h);
//     }