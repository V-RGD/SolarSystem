using System.Threading.Tasks;
using UnityEngine;

namespace CelestialBodies
{
    /// <summary>
    /// Base class for gaz planets
    /// </summary>
    public class GazPlanet : Planet
    {
        [SerializeField] MeshRenderer atmosphereRenderer;
        [SerializeField] Gradient colorGradient;
        [SerializeField] MinMaxValue sizeRange;

        // public float density;
        
        public override async Task InitialisePlanet()
        {
            //randomize size, atmosphere
            transform.localScale = sizeRange.Lerp(SRnd.NextFloat()) * Vector3.one;

            float saturation1 = SRnd.NextFloat();
            float saturation2 = SRnd.NextFloat();
            Color color1 = colorGradient.Evaluate(SRnd.NextFloat());
            Color color2 = colorGradient.Evaluate(SRnd.NextFloat());

            color1 = color1 * saturation1;
            color2 = color2 * saturation2;
            color1.a = 1;
            color2.a = 1;
            
            GetComponent<MeshRenderer>().material.SetColor("_BaseColor1", color1);
            GetComponent<MeshRenderer>().material.SetColor("_BaseColor2", color2);
        }
    }
}
