using System.Collections.Generic;
using System.Threading.Tasks;
using CelestialBodies;
using Generation;
using UnityEngine;

public class PlanetGenerator : GenericSingletonClass<PlanetGenerator>
{
    [SerializeField] TerrestrialPlanet terrestrialPlanetPrefab;
    [SerializeField] GazPlanet gazPlanetPrefab;
    [SerializeField] MinMaxValue planetsToGenerate;

    [field: SerializeField, Range(0, 8)] public int PlanetResolution { get; private set; } = 5;
    [field: SerializeField]public NoiseMapSettings PlanetTerrainSettings { get;private set; }

    public async Task GeneratePlanets()
    {
        int planetsAmount = Mathf.RoundToInt(planetsToGenerate.RandomValue());

        for (int i = 0; i < planetsAmount; i++)
        {
            Planet planet = await CreateNewPlanet();
            await planet.InitialisePlanet();

            SolarSystemSimulator.Instance.MainOrbits.AddOrbit(SolarSystemSimulator.Instance.CreateNewOrbitSystem(new List<CelestialBody>()
            {
                planet
            }));
        }
    }

    async Task<Planet> CreateNewPlanet()
    {
        return SRnd.NextBool() ? await CreateGazPlanet() : await CreateTerrestrialPlanet();
    }

    async Task<Planet> CreateTerrestrialPlanet()
    {
        Planet planet = Instantiate(terrestrialPlanetPrefab);
        await planet.InitialisePlanet();
        return planet;
    }

    async Task<Planet> CreateGazPlanet()
    {
        return Instantiate(gazPlanetPrefab);
    }
}
