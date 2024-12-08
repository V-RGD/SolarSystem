using UnityEngine;

namespace CelestialBodies
{
    /// <summary>
    /// Base class for physical objects that can be put into orbit
    /// </summary>
    public class CelestialBody : MonoBehaviour
    {
        // [field: SerializeField] public Transform Parent { get; set; }
        //
        // public Orbit OrbitSettings { get; set; }
        //
        // public class Orbit
        // {
        //     public int Index;
        //     public Transform Center;
        //
        //     public float BarycenterAngle;
        //     public float OrbitAngle;
        //
        //     // public float Period;
        // }
        //
        // public void SetOrbit(Transform center, int index)
        // {
        //     OrbitSettings.Center = center;
        //     OrbitSettings.Index = index;
        // }
        //
        // // public float OrbitalDistance { get; set; } = 0;
        // // public float OrbitalPeriod { get; set; } = 60;
        // // float _axialRotation;
        // // protected float currentOrbitAngle;
        //
        // public void UpdateSimulation(float speed)
        // {
        //     UpdateRotation(speed);
        //     UpdatePos(speed);
        // }
        //
        // void UpdateRotation(float speed)
        // {
        //     // _axialRotation += speed * AxialRotationSpeed;
        //     // transform.localRotation = Quaternion.Euler(RotationTilt, 0, 0) * Quaternion.Euler(0, _axialRotation, 0);
        //     // if (_axialRotation > 360) _axialRotation -= 360;
        // }
        //
        // void UpdatePos(float speed)
        // {
        //     // if (OrbitalPeriod == 0 || OrbitalDistance == 0) return;
        //     //
        //     // currentOrbitAngle += speed;
        //     // float orbitalAngle = currentOrbitAngle % 360 / ((float)OrbitalPeriod / 360);
        //     // if (currentOrbitAngle > 360) currentOrbitAngle -= 360;
        //     //
        //     // Vector3 position = Vector3.forward * OrbitalDistance;
        //     // position = Quaternion.AngleAxis(orbitalAngle, Vector3.up) * position;
        //     //
        //     // transform.position = position;
        // }
    }
}
