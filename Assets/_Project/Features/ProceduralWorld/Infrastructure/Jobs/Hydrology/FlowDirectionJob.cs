using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    public struct FlowDirectionJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<float> FilledHeights;

        [ReadOnly]
        public NativeArray<int2> Neighbors8;

        public int Size;
        public float StepX;
        public float StepZ;
        
        [WriteOnly]
        public NativeArray<int> FlowTarget;

        public void Execute(int index)
        {
            int x = index % Size;
            int z = index / Size;

            if (x == 0 || z == 0 || x == Size - 1 || z == Size - 1)
            {
                FlowTarget[index] = -1;
                return;
            }

            float h = FilledHeights[index];

            float steepest = 0f;
            int bestIndex = -1;

            for (int k = 0; k < Neighbors8.Length; k++)
            {
                int2 offset = Neighbors8[k];
                int nx = x + offset.x;
                int nz = z + offset.y;
                int nIndex = nz * Size + nx;

                float distance = math.length(new float2(offset.x * StepX, offset.y * StepZ));
                float drop = (h - FilledHeights[nIndex]) / distance;

                if (drop > steepest)
                {
                    steepest = drop;
                    bestIndex = nIndex;
                }
            }
            
            FlowTarget[index] = bestIndex;
        }
    }
}