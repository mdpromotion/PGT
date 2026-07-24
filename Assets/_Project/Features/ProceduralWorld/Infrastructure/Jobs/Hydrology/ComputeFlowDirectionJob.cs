using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile]
    public struct ComputeFlowDirectionJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float> Heights;
        public int Resolution;
        public float MinSlopeEpsilon;

        [WriteOnly] public NativeArray<sbyte> FlowDirection;

        public void Execute(int index)
        {
            int x = index % Resolution;
            int y = index / Resolution;

            if (x == 0 || y == 0 || x == Resolution - 1 || y == Resolution - 1)
            {
                FlowDirection[index] = -1;
                return;
            }

            float myHeight = Heights[index];
            float bestSlope = MinSlopeEpsilon;
            int bestDirection = -1;

            for (int d = 0; d < D8Directions.Count; d++)
            {
                int2 offset = D8Directions.GetOffset(d);
                int nx = x + offset.x;
                int ny = y + offset.y;
                int neighborIndex = ny * Resolution + nx;

                float drop = myHeight - Heights[neighborIndex];
                float slope = drop / D8Directions.GetDistance(d);

                if (slope > bestSlope)
                {
                    bestSlope = slope;
                    bestDirection = d;
                }
            }

            FlowDirection[index] = (sbyte)bestDirection;
        }
    }
}