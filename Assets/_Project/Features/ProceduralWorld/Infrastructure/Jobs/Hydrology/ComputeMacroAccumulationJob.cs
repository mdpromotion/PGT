using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile]
    public struct ComputeMacroAccumulationJob : IJob
    {
        public int PaddedSize;

        [ReadOnly] public NativeArray<float> Heights;
        [ReadOnly] public NativeArray<sbyte> FlowDirection;
        
        public NativeArray<int> SortedIndices;

        public NativeArray<float> Accumulation;

        public void Execute()
        {
            int count = PaddedSize * PaddedSize;

            for (int i = 0; i < count; i++)
            {
                SortedIndices[i] = i;
                Accumulation[i] = 1f;
            }

            SortedIndices.Sort(new DescendingHeightComparer { Heights = Heights });

            for (int i = 0; i < count; i++)
            {
                int index = SortedIndices[i];
                sbyte dir = FlowDirection[index];

                if (dir < 0)
                    continue;

                int x = index % PaddedSize;
                int z = index / PaddedSize;

                ComputeMacroFlowDirectionsJob.GetOffset(dir, out int dx, out int dz);

                int nx = x + dx;
                int nz = z + dz;

                if ((uint)nx >= PaddedSize || (uint)nz >= PaddedSize)
                    continue;

                int neighborIndex = nz * PaddedSize + nx;
                Accumulation[neighborIndex] += Accumulation[index];
            }
        }

        private struct DescendingHeightComparer : System.Collections.Generic.IComparer<int>
        {
            [ReadOnly] public NativeArray<float> Heights;

            public int Compare(int a, int b) => Heights[b].CompareTo(Heights[a]);
        }
    }
}