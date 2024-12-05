using CelestialBodies;
using UnityEngine;

/// <summary>
/// Used to generate the sun of the solar system
/// </summary>
public class Sun : CelestialBody
{
    [SerializeField] Light sunLight;
    [SerializeField] Gradient colorRange;
    
    public void SetSize(float size)
    {
        // Color sunColor = colorRange.Evaluate(Random.value);
        // GetComponent<MeshRenderer>().sharedMaterial.color = sunColor;
        // sunLight.color = sunColor;
        // transform.localScale = Vector3.one * size;
    }
}
