using System;
using UnityEngine;

[Serializable]
public struct MinMaxValue
{
    public float min;
    public float max;

    public float RandomValue() => SRnd.RangeFloat(min, max);

    public float Lerp(float t) => Mathf.Lerp(min, max, t);

    public float RandomOnCurve(AnimationCurve curve) => Lerp(curve.Evaluate(SRnd.NextFloat()));
}