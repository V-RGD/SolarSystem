using UnityEngine;

[CreateAssetMenu]
public class Biome : ScriptableObject
{
    public string label;
    public Color vertexColor;

    public float temperature;

    // public float humidity;
    public float habitability;
}