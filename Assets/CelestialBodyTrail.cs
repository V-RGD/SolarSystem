using System;
using CelestialBodies;
using HPC.Helpers.Unity;
using UnityEngine;

public class CelestialBodyTrail : MonoBehaviour
{
    [SerializeField] float defaultLength = 4;
    [SerializeField] float updateTick;
    TrailRenderer _trailRenderer;
    float _updateTicTimer;

    void Awake()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    void Update()
    {
        _updateTicTimer.DecreaseTimer();
        if (_updateTicTimer <= 0)
        {
            _updateTicTimer = updateTick;
            UpdateTrailLength();
        }
    }

    void UpdateTrailLength()
    {
        _trailRenderer.time = defaultLength / SolarSystemSimulator.Instance.SimulationSpeed;
    }
}
