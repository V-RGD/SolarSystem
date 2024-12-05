using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace MeshGeneration
{
    public struct Icosahedron
    {
        public NativeArray<int> Indices;
        public NativeArray<Vector3> Vertices;

        public Mesh ToMesh()
        {
            Mesh m = new Mesh();

            //Creates submeshes
            m.indexFormat = IndexFormat.UInt32;
            m.vertices = Vertices.ToArray();
            m.SetIndices(Indices.ToArray(), MeshTopology.Triangles, 0);
            m.normals = SmoothNormals();
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
                
                v = Quaternion.AngleAxis(angle, Vector3.right) * v;  //rotate x
                v = Quaternion.AngleAxis((float)360 / 5 * i + (float)360 / 10, Vector3.up) * v; //rotate y
                verts.Add(v);
            }

            //top pentagon
            for (int i = 2; i < 7; i++)
            {
                int ip = i + 1; //third vertex
                if (ip > 6) ip = 2; //if 3rd vertex overflows, goes back to first of top ring
                List<int> indicesToAdd = new List<int>() { 0, i, ip };
                indices.AddRange(indicesToAdd);
            }

            //bottom pentagon
            for (int i = 7; i < 12; i++)
            {
                int ip = i + 1;
                if (ip > 11) ip = 7;
                List<int> indicesToAdd = new List<int>() { 1, ip, i };
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
                indices.AddRange(new[] { a1, b1, a2 });
                //add bottom tri
                indices.AddRange(new[] { b1, b2, a2 });
            }

            return new Icosahedron()
            {
                Vertices = verts.ToNativeArray(Allocator.TempJob),
                Indices = indices.ToNativeArray(Allocator.TempJob),
            };
        }

        public static Icosahedron GenerateIcoSphere(int subdivisions)
        {
            //create icosahedron, then subdivides it
            Icosahedron ico = GenerateDefaultIcosahedron();
            for (int i = 0; i < subdivisions; i++)
            {
                ico.Subdivide();
            }

            return ico;
        }

        Vector3[] SmoothNormals()
        {
            Vector3[] normals = new Vector3[Vertices.Length];
            
            for (int i = 0; i < Vertices.Length; i++) normals[i] = Vertices[i].normalized;
            
            return normals;
        }

        public void Subdivide()
        {
            NativeArray<int> newIndices = new NativeArray<int>(Indices.Length / 3 * 12, Allocator.TempJob);
            NativeArray<Vector3> newVerts = new NativeArray<Vector3>(Indices.Length / 3 * 6, Allocator.TempJob);

            int indiceIndex = 0;
            
            int vertexArrayIndex = 0;

            //for each tri
            for (int i = 0; i < Indices.Length; i += 3)
            {
                //take the three vertices that form the triangle
                Vector3 a = Vertices[Indices[i]];
                Vector3 b = Vertices[Indices[i + 1]];
                Vector3 c = Vertices[Indices[i + 2]];

                //take middle points of the vertices
                Vector3 x = ((a + b) * 0.5f).normalized;
                Vector3 y = ((b + c) * 0.5f).normalized;
                Vector3 z = ((c + a) * 0.5f).normalized;

                //add new vertices to the array
                int vertexIndex = vertexArrayIndex;
                
                newVerts[vertexArrayIndex] = a;
                newVerts[vertexArrayIndex+1] = b;
                newVerts[vertexArrayIndex+2] = c;
                newVerts[vertexArrayIndex+3] = x;
                newVerts[vertexArrayIndex+4] = y;
                newVerts[vertexArrayIndex+5] = z;
                vertexArrayIndex += 6;
                
                // newVerts.AddRange(new[] { a, b, c, x, y, z });

                //connects new triangles
                int[] indicesToAdd = { 0, 3, 5, 3, 1, 4, 3, 4, 5, 5, 4, 2 };
                foreach (int index in indicesToAdd)
                {
                    newIndices[indiceIndex] = index + vertexIndex;
                    indiceIndex++;
                }
            }

            Indices = new NativeArray<int>(newIndices.Length, Allocator.TempJob);
            Vertices = new NativeArray<Vector3>(newVerts.Length, Allocator.TempJob);

            for (int i = 0; i < newIndices.Length; i++) Indices[i] = newIndices[i];
            for (int i = 0; i < newVerts.Length; i++) Vertices[i] = newVerts[i];
        }
    }
}