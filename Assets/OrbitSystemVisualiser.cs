using HPC.Helpers.Unity;
using UnityEngine;

public class OrbitSystemVisualiser : MonoBehaviour
{
    [SerializeField] float updateTic = 1;
    [SerializeField] int sampleCount = 64;
    LineRenderer _lineRenderer;
    Vector3[] _positions;

    OrbitSystem _orbitSystem;
    float _updateTicTimer;
    
    void Awake()
    {
        _orbitSystem = GetComponent<OrbitSystem>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        _updateTicTimer.DecreaseTimer();
        if (_updateTicTimer <= 0)
        {
            _updateTicTimer = updateTic;
            UpdateOrbitRing();
        }
    }

    void UpdateOrbitRing()
    {
        if(_orbitSystem.OuterMass == null) return;
        
        _positions = new Vector3[sampleCount];
        _lineRenderer.positionCount = sampleCount;

        float ratio = 360f / sampleCount;
        float dist = (_orbitSystem.OuterMass.Center.position - _orbitSystem.Center.position).magnitude;
        
        for (int i = 0; i < sampleCount; i++)
        {
            Vector3 angularPosition = Vector3.forward * dist;
            angularPosition = Quaternion.AngleAxis(ratio * i, Vector3.up) * angularPosition; //will need to change vector3.up to local coordinates
            _positions[i] = angularPosition + _orbitSystem.OuterMass.Center.position;
        }
        
        _lineRenderer.SetPositions(_positions);
    }
}
