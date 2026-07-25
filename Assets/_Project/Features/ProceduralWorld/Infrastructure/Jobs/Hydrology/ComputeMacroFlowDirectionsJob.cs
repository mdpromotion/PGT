using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile]
    public struct ComputeMacroFlowDirectionsJob : IJobParallelFor
    {
        public int PaddedSize;

        [ReadOnly] public NativeArray<float> Heights;
        [WriteOnly] public NativeArray<sbyte> FlowDirection;

        public void Execute(int index)
        {
            int x = index % PaddedSize;
            int z = index / PaddedSize;

            float selfHeight = Heights[index];

            sbyte bestDir = -1;
            float bestHeight = selfHeight;

            for (sbyte dir = 0; dir < 8; dir++)
            {
                GetOffset(dir, out int dx, out int dz);

                int nx = x + dx;
                int nz = z + dz;

                if ((uint)nx >= PaddedSize || (uint)nz >= PaddedSize)
                    continue;

                int neighborIndex = nz * PaddedSize + nx;
                float neighborHeight = Heights[neighborIndex];

                if (neighborHeight < bestHeight)
                {
                    bestHeight = neighborHeight;
                    bestDir = dir;
                }
            }

            FlowDirection[index] = bestDir;
        }

        public static void GetOffset(sbyte dir, out int dx, out int dz)
        {
            switch (dir)
            {
                case 0: dx = -1; dz = -1; break;
                case 1: dx = 0; dz = -1; break;
                case 2: dx = 1; dz = -1; break;
                case 3: dx = -1; dz = 0; break;
                case 4: dx = 1; dz = 0; break;
                case 5: dx = -1; dz = 1; break;
                case 6: dx = 0; dz = 1; break;
                default: dx = 1; dz = 1; break; // case 7
            }
        }
    }
}