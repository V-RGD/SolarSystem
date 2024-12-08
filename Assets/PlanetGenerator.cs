using System.Collections.Generic;
using System.Threading.Tasks;
using CelestialBodies;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour
{
    [SerializeField] TerrestrialPlanet terrestrialPlanetPrefab;
    [SerializeField] GazPlanet gazPlanetPrefab;
    [SerializeField] MinMaxValue planetsToGenerate;

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
        return Instantiate(terrestrialPlanetPrefab);
    }

    async Task<Planet> CreateGazPlanet()
    {
        return Instantiate(gazPlanetPrefab);
    }
}
