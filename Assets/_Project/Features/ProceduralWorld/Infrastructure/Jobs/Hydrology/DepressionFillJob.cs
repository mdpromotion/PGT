using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    public struct DepressionFillJob : IJob
    {
        [ReadOnly]
        public NativeArray<float> RawHeights;

        [ReadOnly]
        public NativeArray<int2> Neighbors8;

        public int Size;
        
        [WriteOnly]
        public NativeArray<float> FilledHeights;

        public void Execute()
        {
            int n = Size * Size;

            NativeArray<bool> visited = new NativeArray<bool>(n, Allocator.Temp);
            NativeMinHeap heap = new NativeMinHeap(n, Allocator.Temp);
            
            for (int x = 0; x < Size; x++)
            {
                PushSeed(x, 0, visited, ref heap);
                PushSeed(x, Size - 1, visited, ref heap);
            }

            for (int z = 1; z < Size - 1; z++)
            {
                PushSeed(0, z, visited, ref heap);
                PushSeed(Size - 1, z, visited, ref heap);
            }
            
            while (heap.Count > 0)
            {
                heap.Pop(out int index, out float filledHeight);

                int x = index % Size;
                int z = index / Size;

                for (int k = 0; k < Neighbors8.Length; k++)
                {
                    int2 offset = Neighbors8[k];
                    int nx = x + offset.x;
                    int nz = z + offset.y;

                    if (nx < 0 || nz < 0 || nx >= Size || nz >= Size)
                        continue;

                    int nIndex = nz * Size + nx;

                    if (visited[nIndex])
                        continue;

                    visited[nIndex] = true;
                    
                    float neighborRaw = RawHeights[nIndex];
                    float neighborFilled = math.max(neighborRaw, filledHeight);

                    FilledHeights[nIndex] = neighborFilled;
                    heap.Push(nIndex, neighborFilled);
                }
            }

            heap.Dispose();
            visited.Dispose();
        }

        private void PushSeed(int x, int z, NativeArray<bool> visited, ref NativeMinHeap heap)
        {
            int index = z * Size + x;

            if (visited[index])
                return;

            visited[index] = true;

            float h = RawHeights[index];
            FilledHeights[index] = h;
            heap.Push(index, h);
        }
    }
}