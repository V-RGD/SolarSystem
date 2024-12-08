using System.Threading.Tasks;
using UnityEngine;

namespace CelestialBodies
{
    /// <summary>
    /// Base class for planets that have a solid ground, an atmosphere (optional), and oceans (optional)
    /// </summary>
    public class TerrestrialPlanet : Planet
    {
        [SerializeField] MeshFilter rockFilter;
        [SerializeField] MeshRenderer waterRenderer;
        [SerializeField] MeshRenderer atmosphereRenderer;
        [SerializeField] MinMaxValue sizeRange;

        public override async Task InitialisePlanet()
        {
            transform.localScale = sizeRange.Lerp(SRnd.NextFloat()) * Vector3.one;
        }
    }
}
