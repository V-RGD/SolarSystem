using System.Collections.Generic;
using System.Threading.Tasks;
using CelestialBodies;
using Generation;
using JobQueries;
using UnityEngine;

public class PlanetGenerator : GenericSingletonClass<PlanetGenerator>
{
    [SerializeField] TerrestrialPlanet terrestrialPlanetPrefab;
    [SerializeField] GazPlanet gazPlanetPrefab;
    [SerializeField] MinMaxValue planetsToGenerate;

    [field: SerializeField, Range(0, 12)] public int PlanetResolution { get; private set; } = 9;
    [field: SerializeField] public NoiseMapSettings PlanetTerrainSettings { get;private set; }

    public async Task GeneratePlanets()
    {
        FlashClock clock = new FlashClock();
        clock.Start();
        
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
        
        Debug.Log($"total generation time {clock.FlashReset()}");
    }

    async Task<Planet> CreateNewPlanet()
    {
        // return SRnd.NextBool() ? await CreateGazPlanet() : await CreateTerrestrialPlanet();
        return await CreateTerrestrialPlanet();
    }

    async Task<Planet> CreateTerrestrialPlanet()
    {
        Planet planet = Instantiate(terrestrialPlanetPrefab);
        return planet;
    }

    async Task<Planet> CreateGazPlanet()
    {
        return Instantiate(gazPlanetPrefab);
    }
}
