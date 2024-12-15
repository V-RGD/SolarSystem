using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;

namespace JobQueries
{
    public static class JobList
    {
        public static void ExecuteAll<T>(this NativeArray<T> jobs) where T : unmanaged, IJob
        {
            NativeArray<JobHandle> handles = new NativeArray<JobHandle>(jobs.Length, Allocator.TempJob);

            for (int i = 0; i < handles.Length; i++)
            {
                handles[i] = jobs[i].Schedule();
            }

            JobHandle.CompleteAll(handles);
            handles.Dispose();
        }

        public static NativeArray<T> Merge<T>(this List<NativeArray<T>> list) where T : unmanaged
        {
            int size = list.Sum(na => na.Length);
            NativeArray<T> output = new NativeArray<T>(size, Allocator.TempJob);
            
            int index = 0;
            foreach (NativeArray<T> nativeArray in list)
            {
                foreach (T element in nativeArray)
                {
                    output[index] = element;
                    index++;
                }
            }

            return output;
        }
    }
}
