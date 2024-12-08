using System.Collections.Generic;
using UnityEngine;

namespace CelestialBodies
{
    /// <summary>
    /// Simulates 
    /// </summary>
    public class SolarSystemSimulator : GenericSingletonClass<SolarSystemSimulator>
    {
        [SerializeField] OrbitSystem baryCenterPrefab;
        [field: SerializeField] public Transform Origin { get; private set; }
        [field: SerializeField] public float SimulationSpeed { get; private set; } = 2;
        [SerializeField] float maxSystemSize;
        [SerializeField] float eclipticAngle = 0;

        public OrbitSystem MainOrbits { get; private set; }

        public OrbitSystem CreateNewOrbitSystem(List<CelestialBody> mainBodies)
        {
            OrbitSystem orbitSystem = Instantiate(baryCenterPrefab);
            orbitSystem.SetMainBodies(mainBodies);
            return orbitSystem;
        }
        
        public override void Awake()
        {
            base.Awake();
            MainOrbits = CreateNewOrbitSystem(new List<CelestialBody>());
            MainOrbits.OuterMass = null;
            MainOrbits.name = "System Center";
        }

        void Update() => UpdateSimulation();
        
        void UpdateSimulation()
        {
            MainOrbits.ForceMaxOrbitalDistance = maxSystemSize;
            MainOrbits.CurrentOrbitAngle = eclipticAngle;
            
            float elapsedTime = Time.deltaTime * SimulationSpeed;
            MainOrbits.ResolveSystem(elapsedTime);
        }
    }
}
