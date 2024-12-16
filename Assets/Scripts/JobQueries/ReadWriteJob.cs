using Unity.Collections;
using Unity.Jobs;

namespace JobQueries;

public struct ReadWriteJob<T> : IJob where T : unmanaged
{
    public NativeArray<T> Input;
    
    public void Execute()
    {
        
    }

    public T Access(int index)
    {
        return Input[index];
    }

    public void Write(int index, T value)
    {
        Input[index] = value;
    }
}

public struct IWriteNoise : ReadWriteJob<float>
{
    
}
