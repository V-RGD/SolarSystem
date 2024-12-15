using JobQueries;
using MeshGeneration;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public int resolution;
    
    void Start()
    {
        FlashClock clock = new FlashClock();
        clock.Start();
        Icosahedron ico = Icosahedron.GenerateIcoSphere(resolution).Result;
        
        Debug.Log(clock.FlashReset());
        
        Debug.Log(ico.Vertices.Length);
        Debug.Log(ico.Indices.Length);
    }
}
