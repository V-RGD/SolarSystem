using UnityEngine;

namespace CelestialBodies
{
    /// <summary>
    /// Base class for physical objects that can be put into orbit
    /// </summary>
    public class CelestialBody : MonoBehaviour
    {
        [field: SerializeField] public float RotationSpeed { get; protected set; } = 25;
        public float Tilt { get; protected set; } = 24;
        public float OrbitalDistance { get; protected set; } = 0;
        public float OrbitalPeriod { get; protected set; } = 60;

        float _axialRotation;
        protected float CurrentOrbitAngle;

        void Update()
        {
            UpdateRotation();
            UpdatePos();
        }

        void UpdateRotation()
        {
            _axialRotation += Time.deltaTime * RotationSpeed;
            transform.localRotation = Quaternion.Euler(Tilt, 0, 0) * Quaternion.Euler(0, _axialRotation, 0);
            if (_axialRotation > 360) _axialRotation -= 360;
        }

        void UpdatePos()
        {
            if(OrbitalPeriod == 0 || OrbitalDistance == 0) return;
            
            CurrentOrbitAngle += Time.deltaTime;
            float orbitalAngle = CurrentOrbitAngle % 360 / ((float)OrbitalPeriod / 360);
            if (CurrentOrbitAngle > 360) CurrentOrbitAngle -= 360;
            
            Vector3 position = Vector3.forward * OrbitalDistance;
            position = Quaternion.AngleAxis(orbitalAngle, Vector3.up) * position;

            transform.position = position;
        }
    }
}