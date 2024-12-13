using System;
using CelestialBodies;
using UnityEngine;

/// <summary>
/// Used to generate the sun of the solar system
/// </summary>
public class Sun : CelestialBody
{
    [SerializeField] Gradient colorGradient;
    [SerializeField] MinMaxValue sizeRange;
    [SerializeField] Light sunLight;

    public void SetSize(float sizeRatio, float colorRatio)
    {
        //set size
        transform.localScale = sizeRange.Lerp(sizeRatio) * Vector3.one;

        //set color
        Color sunColor = colorGradient.Evaluate(colorRatio);
        GetComponent<MeshRenderer>().material.SetColor("_BaseColor", sunColor);
        // sunLight.color = sunColor;
    }

    // public enum GrowthType
    // {
    //     Temperature,
    //     Size
    // }
    //
    // public void UpdateMass(float mass, GrowthType growthType)
    // {
    //     if (growthType is GrowthType.Size)
    //     {
    //         //increases sun size
    //     }
    //     if (growthType is GrowthType.Temperature)
    //     {
    //         //updates color
    //     }
    // }
}
