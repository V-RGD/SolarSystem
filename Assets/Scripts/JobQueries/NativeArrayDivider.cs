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
            
            for (int i = 0; i < divisions; i++)
            {
                int size = elementsPerDivision;
                if (i < divisions) size = Original.Length % divisions;
                NativeArray<T> subdivision = new NativeArray<T>(size, Allocator.TempJob);
                
                for (int j = 0; j < size; j++)
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
