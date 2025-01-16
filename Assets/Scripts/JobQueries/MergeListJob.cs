// using Unity.Collections;
// using Unity.Jobs;
//
// namespace JobQueries;
//
// /// <summary>
// /// Use this to create a temporary job that will read or write a lot of values in a list
// /// </summary>
// public struct MergeListJob<T> : IJob, IFlushNativeArrays where T : unmanaged
// {
//     NativeArray<T> OriginalA;
//     NativeArray<T> OriginalB;
//     
//     NativeArray<T> Output;
//
//     public void Execute() => Merge();
//
//     void Merge()
//     {
//         int size = list.Sum(na => na.Length);
//         NativeArray<T> output = new NativeArray<T>(size, Allocator.TempJob);
//             
//         int index = 0;
//         foreach (NativeArray<T> nativeArray in list)
//         {
//             foreach (T element in nativeArray)
//             {
//                 output[index] = element;
//                 index++;
//             }
//         }
//
//         return output;
//     }
//
//     public void Flush()
//     {
//         
//     }
// }
