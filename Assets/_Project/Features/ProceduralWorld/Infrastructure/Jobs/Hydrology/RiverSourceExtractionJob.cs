using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    public struct RiverSourceExtractionJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<float> Accumulation;

        [ReadOnly]
        public NativeArray<int> FlowTarget;

        [ReadOnly]
        public NativeArray<int2> Neighbors8;

        public int Size;
        public float Threshold;

        [WriteOnly]
        public NativeList<int>.ParallelWriter Sources;

        public void Execute(int index)
        {
            if (Accumulation[index] < Threshold)
                return;

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

                bool neighborIsRiver = Accumulation[nIndex] >= Threshold;
                bool neighborFlowsHere = FlowTarget[nIndex] == index;

                if (neighborIsRiver && neighborFlowsHere)
                    return;
            }

            Sources.AddNoResize(index);
        }
    }
}