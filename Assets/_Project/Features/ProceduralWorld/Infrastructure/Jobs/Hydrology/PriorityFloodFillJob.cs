using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile]
    public struct PriorityFloodFillJob : IJob
    {
        public int PaddedSize;

        public NativeArray<float> Heights;

        public void Execute()
        {
            int count = PaddedSize * PaddedSize;

            var visited = new NativeArray<bool>(count, Allocator.Temp);
            var heap = new NativeMinHeap(count, Allocator.Temp);
            
            for (int x = 0; x < PaddedSize; x++)
            {
                PushBorder(x, 0, ref visited, ref heap);
                PushBorder(x, PaddedSize - 1, ref visited, ref heap);
            }
            for (int z = 1; z < PaddedSize - 1; z++)
            {
                PushBorder(0, z, ref visited, ref heap);
                PushBorder(PaddedSize - 1, z, ref visited, ref heap);
            }

            Span8 offsets = default;

            while (heap.Count > 0)
            {
                heap.Pop(out int index, out float poppedHeight);

                int x = index % PaddedSize;
                int z = index / PaddedSize;

                for (int dir = 0; dir < 8; dir++)
                {
                    ComputeMacroFlowDirectionsJob.GetOffset((sbyte)dir, out int dx, out int dz);

                    int nx = x + dx;
                    int nz = z + dz;

                    if ((uint)nx >= PaddedSize || (uint)nz >= PaddedSize)
                        continue;

                    int neighborIndex = nz * PaddedSize + nx;
                    if (visited[neighborIndex])
                        continue;

                    visited[neighborIndex] = true;
                    
                    float neighborHeight = Heights[neighborIndex];
                    
                    const float Epsilon = 0.0001f;
                    
                    float minStep = poppedHeight + Epsilon;
                    float filledHeight = neighborHeight < minStep ? minStep : neighborHeight;

                    Heights[neighborIndex] = filledHeight;
                    heap.Push(neighborIndex, filledHeight);
                }
            }

            visited.Dispose();
            heap.Dispose();
        }

        private void PushBorder(int x, int z, ref NativeArray<bool> visited, ref NativeMinHeap heap)
        {
            int index = z * PaddedSize + x;
            if (visited[index])
                return;

            visited[index] = true;
            heap.Push(index, Heights[index]);
        }

        private struct Span8 { }
    }
}