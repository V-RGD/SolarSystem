using System.Collections.Generic;
using System.Threading.Tasks;
using CelestialBodies;
using NaughtyAttributes;
using UnityEngine;

public class SunGenerator : MonoBehaviour
{
    [SerializeField] Sun sunPrefab;
    [CurveRange(0, 1, 1, 3)] public AnimationCurve sunCountRepartition;

    /// <summary>
    /// Generates 1 to 3 suns and their respective orbits
    /// </summary>
    public async Task GenerateSuns()
    {
        int sunCount = GenerateSunCount();
        
        List<Sun> suns = new List<Sun>();

        for (int i = 0; i < sunCount; i++)
        {
            Sun newSun = await CreateSunInstance();
            suns.Add(newSun);
        }

        await PositionSuns(suns);
    }

    async Task<Sun> CreateSunInstance()
    {
        Sun newSun = Instantiate(sunPrefab);
        newSun.SetSize(SRnd.NextFloat(), SRnd.NextFloat());
        return newSun;
    }

    async Task PositionSuns(List<Sun> suns)
    {
        //for each pair of suns / sole sun
        for (int i = 0; i < suns.Count; i += 2)
        {
            //if there is one sun left to position, puts it in a single orbit
            List<CelestialBody> sunOrbit = new List<CelestialBody>() {suns[i]};
            //if pair, puts it in a dual orbit
            if(i < suns.Count - 1) sunOrbit.Add(suns[i+1]);

            //adds orbit to system
            SolarSystemSimulator.Instance.MainOrbits.AddOrbit(SolarSystemSimulator.Instance.CreateNewOrbitSystem(sunOrbit));
        }
    }

    int GenerateSunCount() => Mathf.RoundToInt(sunCountRepartition.Evaluate(SRnd.NextFloat()));
}
