using System;
using System.Collections.Generic;
using CelestialBodies;
using UnityEngine;

public class OrbitSystem : MonoBehaviour
{
    public OrbitSystem OuterMass { get; set; }
    public float ForceMaxOrbitalDistance { get; set; }
    public List<OrbitSystem> Orbits { get; private set; }
    // public float eclipticAngle;

    public List<CelestialBody> MainBodies { get; set; }
    public float CurrentOrbitAngle { get; set; } //
    public float MainBodiesAngle { get; set; }

    public Transform Center { get; private set; }

    void Awake()
    {
        Center = transform;
        CurrentOrbitAngle = SRnd.RangeFloat(0, 360);
        MainBodiesAngle = SRnd.RangeFloat(0, 360);
    }


    public void AddOrbit(OrbitSystem orbitSystem)
    {
        Orbits.Add(orbitSystem);
        orbitSystem.OuterMass = this;
    }

    public void SetMainBodies(List<CelestialBody> mainBodies)
    {
        this.MainBodies = new List<CelestialBody>();
        foreach (CelestialBody body in mainBodies)
        {
            this.MainBodies.Add(body);
        }
        
        Orbits = new List<OrbitSystem>();
    }

    public void ResolveSystem(float elapsedTime)
    {
        //position of the outer body which the orbit of this system is based on
        Vector3 outerMassPosition = Vector3.zero;
        if (OuterMass == null) ;
        else outerMassPosition = OuterMass.Center.position;

        for (int i = 0; i < Orbits.Count; i++)
        {
            //places all elements of the orbit at a safe distance
            float distanceFromOuterMass = GetMaxOrbitalDist() / Orbits.Count * i;
            float angularSpeed = GetAngularSpeed(distanceFromOuterMass);

            //update angle in reference to the outer body
            float angle = Orbits[i].CurrentOrbitAngle + angularSpeed * elapsedTime;
            angle = angle % 360;
            Orbits[i].CurrentOrbitAngle = angle;

            //updates position
            Vector3 angularPosition = Vector3.forward * distanceFromOuterMass;
            angularPosition = Quaternion.AngleAxis(Orbits[i].CurrentOrbitAngle, Vector3.up) * angularPosition; //will need to change vector3.up to local coordinates

            Orbits[i].Center.position = outerMassPosition + angularPosition;
        }

        UpdateMainBodies(elapsedTime);

        //then does the same for each of the satellite orbits of the main orbits 
        foreach (var orbit in Orbits)
        {
            orbit.ResolveSystem(elapsedTime);
        }
    }

    void UpdateMainBodies(float elapsedTime)
    {
        if (MainBodies.Count == 1)
        {
            MainBodies[0].transform.position = Center.position;
        }
        else if(MainBodies.Count != 0)
        {
            Vector3 centerPos = Vector3.zero;
            if (Center != null) centerPos = Center.position;
            
            float mainBodiesDist = GetOrbitDist(1, MainBodies.Count, GetMaxOrbitalDist());
            MainBodiesAngle += GetAngularSpeed(mainBodiesDist) * elapsedTime;
            
            //then positions the main bodies accordingly
            for (int i = 0; i < MainBodies.Count; i++)
            {
                int maxIndex = Orbits.Count > 0 ? Orbits.Count : 1;
                float angle = 360f / MainBodies.Count * i;
                float dist = GetOrbitDist(1, maxIndex, GetMaxOrbitalDist());
                
                Vector3 angularPosition = GetAngularPosition(dist, MainBodiesAngle + angle);
                MainBodies[i].transform.position = centerPos + angularPosition;
            }
        }
    }

    const float OrbitalSpeedMultiplier = 100;
    
    float GetAngularSpeed(float dist)
    {
        if (dist == 0) return 0;
        return 1/Mathf.Sqrt(Mathf.Pow((dist), 3)) * OrbitalSpeedMultiplier;
    }

    public float GetMaxOrbitalDist()
    {
        if (OuterMass == null) return ForceMaxOrbitalDistance;
        return OuterMass.GetMaxOrbitalDist() / OuterMass.Orbits.Count * 0.25f;
    }

    static float GetOrbitDist(int index, int indexCount, float maxDist)
    {
        return maxDist / indexCount * index;
    }

    static Vector3 GetAngularPosition(float dist, float currentAngle)
    {
        //update angle in reference to the outer body
        currentAngle = currentAngle % 360;

        //updates position
        Vector3 angularPosition = Vector3.forward * dist;
        angularPosition = Quaternion.AngleAxis(currentAngle, Vector3.up) * angularPosition; //will need to change vector3.up to local coordinates

        return angularPosition;
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < MainBodies.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(Center.position, (MainBodies[i].transform.position - Center.position).magnitude);
        }

        for (int i = 0; i < Orbits.Count; i++)
        {
            Gizmos.color = Color.green;
            //Gizmos.DrawWireSphere(Center.position, (Orbits[i].transform.position - Center.position).magnitude);
        }
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Center.position, GetMaxOrbitalDist());
    }
}
