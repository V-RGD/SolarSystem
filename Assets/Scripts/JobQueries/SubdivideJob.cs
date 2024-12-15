using JobQueries;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct SubdivisionJob : IJob, IFlushNativeArrays
{
    public NativeArray<Vector3> InputVerts;
    public NativeArray<int> InputIndices;
    public NativeArray<Vector3> OutputVerts;
    public NativeArray<int> OutputIndices;

    public SubdivisionJob(NativeArray<int> inputIndices, NativeArray<Vector3> inputVerts)
    {
        InputVerts = inputVerts;
        InputIndices = inputIndices;
        OutputVerts = new NativeArray<Vector3>(inputIndices.Length * 2, Allocator.TempJob);
        OutputIndices = new NativeArray<int>(inputIndices.Length * 4, Allocator.TempJob);
    }

    static readonly int[] IndexOrder = new int[]
    {
        0, 3, 5, 3, 1, 4, 3, 4, 5, 5, 4, 2
    };

    public void Execute()
    {
        int indiceIndex = 0;
        int vertexArrayIndex = 0;

        Vector3 a;
        Vector3 b;
        Vector3 c;
        Vector3 x;
        Vector3 y;
        Vector3 z;

        int vertexIndex;
        
        //for each tri
        for (int i = 0; i < InputIndices.Length; i += 3)
        {
            //take the three vertices that form the triangle
            a = InputVerts[InputIndices[i]];
            b = InputVerts[InputIndices[i + 1]];
            c = InputVerts[InputIndices[i + 2]];

            //take middle points of the vertices
            x = ((a + b) * 0.5f).normalized;
            y = ((b + c) * 0.5f).normalized;
            z = ((c + a) * 0.5f).normalized;

            //add new vertices to the array
            vertexIndex = vertexArrayIndex;

            OutputVerts[vertexArrayIndex] = a;
            OutputVerts[vertexArrayIndex + 1] = b;
            OutputVerts[vertexArrayIndex + 2] = c;
            OutputVerts[vertexArrayIndex + 3] = x;
            OutputVerts[vertexArrayIndex + 4] = y;
            OutputVerts[vertexArrayIndex + 5] = z;
            vertexArrayIndex += 6;

            //connects new triangles
            foreach (int index in IndexOrder)
            {
                OutputIndices[indiceIndex] = index + vertexIndex;
                indiceIndex++;
            }
        }
    }

    public void Flush()
    {
    }
}

// using Helpers;
// using Unity.Collections;
// using Unity.Jobs;
// using UnityEngine;
//
// namespace JobQueries
// {
//     /// <summary>
//     /// Used to subdivide 
//     /// </summary>
//     public struct SubdivisionJob : IJob, IFlushNativeArrays
//     {
//         public int inputVertexOffset;
//         public int outputVertexOffset;
//         public NativeArray<Vector3> InputVerts;
//         public NativeArray<int> InputIndices;
//         public NativeArray<Vector3> OutputVerts;
//         public NativeArray<int> OutputIndices;
//
//         /// <summary>
//         /// Used to subdivide 
//         /// </summary>
//         public SubdivisionJob(NativeArray<int> inputIndices, NativeArray<Vector3> inputVerts, int inputVertexOffset, int outputVertexOffset)
//         {
//             InputVerts = inputVerts;
//             InputIndices = inputIndices;
//             OutputVerts = new NativeArray<Vector3>(inputVerts.Length * 4, Allocator.TempJob);
//             OutputIndices = new NativeArray<int>(inputIndices.Length * 6, Allocator.TempJob);
//             this.inputVertexOffset = inputVertexOffset;
//             this.outputVertexOffset = outputVertexOffset;
//         }
//
//         public void Execute()
//         {
//             ComputeTopology();
//             FixIndexMismatch();
//         }
//         
//         static readonly int[] IndicesToAdd = { 0, 3, 5, 3, 1, 4, 3, 4, 5, 5, 4, 2 };
//
//         void ComputeTopology()
//         {
//             int indiceIndex = 0;
//             int vertexArrayIndex = 0;
//             
//             PrettyLogs.LogMultiple(new []{inputVertexOffset, outputVertexOffset}, new []{"Input Index", "Output Index"});
//
//             for (int i = 0; i < InputIndices.Length; i++)
//             {
//                 InputIndices[i] -= inputVertexOffset;
//             }
//
//             //for each tri
//             for (int i = 0; i < InputIndices.Length; i += 3)
//             {
//                 //take the three vertices that form the triangle
//                 Vector3 a = InputVerts[InputIndices[i]];
//                 Vector3 b = InputVerts[InputIndices[i + 1]];
//                 Vector3 c = InputVerts[InputIndices[i + 2]];
//
//                 //take middle points of the vertices
//                 Vector3 x = ((a + b) * 0.5f).normalized;
//                 Vector3 y = ((b + c) * 0.5f).normalized;
//                 Vector3 z = ((c + a) * 0.5f).normalized;
//
//                 //add new vertices to the array
//                 int vertexIndex = vertexArrayIndex;
//
//                 OutputVerts[vertexArrayIndex] = a;
//                 OutputVerts[vertexArrayIndex + 1] = b;
//                 OutputVerts[vertexArrayIndex + 2] = c;
//                 OutputVerts[vertexArrayIndex + 3] = x;
//                 OutputVerts[vertexArrayIndex + 4] = y;
//                 OutputVerts[vertexArrayIndex + 5] = z;
//                 
//                 vertexArrayIndex += 6;
//
//                 //connects new triangles
//                 foreach (int index in IndicesToAdd)
//                 {
//                     OutputIndices[indiceIndex] = index + vertexIndex;
//                     indiceIndex++;
//                 }
//             }
//         }
//
//         void FixIndexMismatch()
//         {
//             //for each indices, repositions it
//             for (int i = 0; i < OutputIndices.Length; i++)
//             {
//                 OutputIndices[i] += outputVertexOffset;
//             }
//         }
//
//
//         public void Flush()
//         {
//             InputVerts.Dispose();
//             InputIndices.Dispose();
//             OutputVerts.Dispose();
//             OutputIndices.Dispose();
//         }
//     }
// }
