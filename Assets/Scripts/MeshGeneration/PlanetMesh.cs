using Generation;

namespace MeshGeneration
{
    public static class PlanetMesh
    {
        public static void GenerateElevation(Icosahedron ico, SphereValueMap<float> elevation)
        {
            //updates vertex height depending on elevation
            for (int i = 0; i < ico.Vertices.Length; i++)
            {
                ico.Vertices[i] *= (1 + elevation.Map[i].Value);
            }
        }
    }
}