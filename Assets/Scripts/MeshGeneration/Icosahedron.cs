using System.Collections.Generic;
using System.Threading.Tasks;
using JobQueries;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

namespace MeshGeneration
{
    public struct Icosahedron : IFlushNativeArrays
    {
        public NativeArray<int> Indices;
        public NativeArray<Vector3> Vertices;
        public NativeArray<Color> Colors;
        public int subdivisionCount;

        public Mesh ToMesh(bool smoothNormals = false)
        {
            Mesh m = new Mesh();
            
            //Creates submeshes
            m.indexFormat = IndexFormat.UInt32;
            m.SetVertices(Vertices);
            m.SetIndices(Indices, MeshTopology.Triangles, 0);
            m.SetColors(Colors);

            m.RecalculateNormals();

            return m;
        }

        public static Icosahedron GenerateDefaultIcosahedron()
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> indices = new List<int>();

            float angle = 26.56505117f; //atan 1/2 in degrees

            verts.Add(Vector3.up);
            verts.Add(Vector3.down);

            //adds ring 1 and 2
            for (int i = 0; i < 5; i++)
            {
                Vector3 v = Vector3.forward;
                //rotate x
                v = Quaternion.AngleAxis(-angle, Vector3.right) * v;
                //rotate y
                v = Quaternion.AngleAxis((float)360 / 5 * i, Vector3.up) * v;
                verts.Add(v);
            }

            for (int i = 0; i < 5; i++)
            {
                Vector3 v = Vector3.forward;

                v = Quaternion.AngleAxis(angle, Vector3.right) * v; //rotate x
                v = Quaternion.AngleAxis((float)360 / 5 * i + (float)360 / 10, Vector3.up) * v; //rotate y
                verts.Add(v);
            }

            //top pentagon
            for (int i = 2; i < 7; i++)
            {
                int ip = i + 1; //third vertex
                if (ip > 6) ip = 2; //if 3rd vertex overflows, goes back to first of top ring
                List<int> indicesToAdd = new List<int>()
                {
                    0,
                    i,
                    ip
                };
                indices.AddRange(indicesToAdd);
            }

            //bottom pentagon
            for (int i = 7; i < 12; i++)
            {
                int ip = i + 1;
                if (ip > 11) ip = 7;
                List<int> indicesToAdd = new List<int>()
                {
                    1,
                    ip,
                    i
                };
                indices.AddRange(indicesToAdd);
            }

            int firstRingVertexIndex = 2;
            int ringSize = 5;

            //in between - verts
            for (int r = 0; r < 5; r++)
            {
                int a1 = r + firstRingVertexIndex;
                int a2 = r + 1 + firstRingVertexIndex;
                if (a2 >= 7) a2 -= 5;

                int b1 = r + firstRingVertexIndex + ringSize;
                int b2 = r + 1 + firstRingVertexIndex + ringSize;
                if (b2 >= 12) b2 -= 5;

                //add top tri
                indices.AddRange(new[]
                {
                    a1, b1, a2
                });
                //add bottom tri
                indices.AddRange(new[]
                {
                    b1, b2, a2
                });
            }

            return new Icosahedron()
            {
                Vertices = verts.ToNativeArray(Allocator.TempJob),
                Indices = indices.ToNativeArray(Allocator.TempJob),
            };
        }

        public static async Task<Icosahedron> GenerateIcoSphere(int subdivisions)
        {
            //create icosahedron, then subdivides it
            Icosahedron ico = GenerateDefaultIcosahedron();
            for (int i = 0; i < subdivisions; i++)
            {
                await ico.SubdivideUnitJob();
                ico.subdivisionCount++;
            }

            return ico;
        }


        Task SubdivideUnitJob()
        {
            SubdivisionJob job = new SubdivisionJob(Indices, Vertices);
            JobHandle handle = job.Schedule();
            handle.Complete();

            Indices = job.OutputIndices;
            Vertices = job.OutputVerts;

            return Task.CompletedTask;
        }
        
        
        public void ReshadeFlat()
        {
            NativeArray<Vector3> newVertices = new NativeArray<Vector3>(Indices.Length, Allocator.TempJob);
            NativeArray<Color> newColors = new NativeArray<Color>(Indices.Length, Allocator.TempJob);
            NativeArray<int> newTris = new NativeArray<int>(Indices.Length, Allocator.TempJob);

            //for each triangle
            for (int i = 0; i < Indices.Length; i += 3)
            {
                //adds three new separate vertices and their indices
                newVertices[i] = Vertices[Indices[i]];
                newVertices[i + 1] = Vertices[Indices[i + 1]];
                newVertices[i + 2] = Vertices[Indices[i + 2]];

                //and their corresponding indices
                newTris[i] = i;
                newTris[i + 1] = i + 1;
                newTris[i + 2] = i + 2;
                
                //keep color data
                newColors[i] = Colors[Indices[i]];
                newColors[i + 1] = Colors[Indices[i + 1]];
                newColors[i + 2] = Colors[Indices[i + 2]];
            }

            Vertices = newVertices;
            Indices = newTris;
            Colors = newColors;
        }

        public void Flush()
        {
            Indices.Dispose();
            Vertices.Dispose();
        }

        // public Task Subdivide()
        // {
        //     NativeArray<int> newIndices = new NativeArray<int>(Indices.Length / 3 * 12, Allocator.TempJob);
        //     NativeArray<Vector3> newVerts = new NativeArray<Vector3>(Indices.Length / 3 * 6, Allocator.TempJob);
        //
        //     int indiceIndex = 0;
        //     int vertexArrayIndex = 0;
        //
        //     Vector3 a;
        //     Vector3 b;
        //     Vector3 c;
        //     Vector3 x;
        //     Vector3 y;
        //     Vector3 z;
        //
        //     int vertexIndex;
        //     int[] indicesToAdd;
        //
        //     //for each tri
        //     for (int i = 0; i < Indices.Length; i += 3)
        //     {
        //         //take the three vertices that form the triangle
        //         a = Vertices[Indices[i]];
        //         b = Vertices[Indices[i + 1]];
        //         c = Vertices[Indices[i + 2]];
        //
        //         //take middle points of the vertices
        //         x = ((a + b) * 0.5f).normalized;
        //         y = ((b + c) * 0.5f).normalized;
        //         z = ((c + a) * 0.5f).normalized;
        //
        //         //add new vertices to the array
        //         vertexIndex = vertexArrayIndex;
        //
        //         newVerts[vertexArrayIndex] = a;
        //         newVerts[vertexArrayIndex + 1] = b;
        //         newVerts[vertexArrayIndex + 2] = c;
        //         newVerts[vertexArrayIndex + 3] = x;
        //         newVerts[vertexArrayIndex + 4] = y;
        //         newVerts[vertexArrayIndex + 5] = z;
        //         vertexArrayIndex += 6;
        //
        //         //connects new triangles
        //         indicesToAdd = new int[]
        //         {
        //             0, 3, 5, 3, 1, 4, 3, 4, 5, 5, 4, 2
        //         };
        //         foreach (int index in indicesToAdd)
        //         {
        //             newIndices[indiceIndex] = index + vertexIndex;
        //             indiceIndex++;
        //         }
        //     }
        //
        //     Indices = new NativeArray<int>(newIndices.Length, Allocator.TempJob);
        //     Vertices = new NativeArray<Vector3>(newVerts.Length, Allocator.TempJob);
        //
        //     for (int i = 0; i < newIndices.Length; i++) Indices[i] = newIndices[i];
        //     for (int i = 0; i < newVerts.Length; i++) Vertices[i] = newVerts[i];
        //
        //     return Task.CompletedTask;
        // }
    }
}
