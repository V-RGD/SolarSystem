using System;

[Serializable]
public struct MinMaxValue
{
    public float min;
    public float max;

    public float RandomValue() => SRnd.RangeFloat(min, max);
}