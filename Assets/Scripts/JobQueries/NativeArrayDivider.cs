using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace JobQueries
{
    public class NativeArrayDivider<T> : IFlushNativeArrays where T : unmanaged
    {
        public NativeArrayDivider(NativeArray<T> original)
        {
            Original = original;
        }

        NativeArray<T> Original;
        public NativeArray<T>[] Chunks;

        public void DivideIntensiveTask(int divisions)
        {
            int elementsPerDivision = Mathf.CeilToInt((float)Original.Length / divisions);
            Chunks = new NativeArray<T>[divisions];

            int element = 0;
            int elementsLeft = Original.Length;
            
            for (int i = 0; i < divisions; i++)
            {
                int elementsToSet = elementsLeft < elementsPerDivision ? elementsLeft : elementsPerDivision;
                elementsLeft -= elementsPerDivision;
                
                // int size = elementsPerDivision;
                // if (i >= divisions - 1) size = Original.Length % divisions;
                NativeArray<T> subdivision = new NativeArray<T>(elementsToSet, Allocator.TempJob);
                
                for (int j = 0; j < elementsToSet; j++)
                {
                    subdivision[j] = Original[element];
                    element++;
                }

                Chunks[i] = subdivision;
            }
        }

        public void Flush()
        {
            foreach (NativeArray<T> nativeArray in Chunks) nativeArray.Dispose();
            Original.Dispose();
        }
    }
}
