using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile]
    public struct ComputeMacroFlowDirectionsJob : IJobParallelFor
    {
        public int PaddedSize;
        public int PaddingCells;
        public int TileCells;
        public int RiverZoneMargin;
        public float EdgeBiasStrength;

        [ReadOnly] public NativeArray<float> Heights;
        [WriteOnly] public NativeArray<sbyte> FlowDirection;

        public void Execute(int index)
        {
            int x = index % PaddedSize;
            int z = index / PaddedSize;

            float selfHeight = Heights[index];

            sbyte bestDir = -1;
            float bestScore = selfHeight;

            for (sbyte dir = 0; dir < 8; dir++)
            {
                GetOffset(dir, out int dx, out int dz);

                int nx = x + dx;
                int nz = z + dz;

                if ((uint)nx >= PaddedSize || (uint)nz >= PaddedSize)
                    continue;

                int neighborIndex = nz * PaddedSize + nx;
                float neighborHeight = Heights[neighborIndex];

                float neighborScore = neighborHeight + EdgePenalty(nx, nz);

                if (neighborScore < bestScore)
                {
                    bestScore = neighborScore;
                    bestDir = dir;
                }
            }

            FlowDirection[index] = bestDir;
        }
        
        private float EdgePenalty(int x, int z)
        {
            int coreMin = PaddingCells;
            int coreMax = PaddingCells + TileCells;

            int distX = math.min(x - coreMin, coreMax - 1 - x);
            int distZ = math.min(z - coreMin, coreMax - 1 - z);
            int dist = math.min(distX, distZ);

            if (dist >= RiverZoneMargin) return 0f;

            float t = 1f - math.saturate((float)dist / RiverZoneMargin);
            return t * t * EdgeBiasStrength;
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
                default: dx = 1; dz = 1; break;
            }
        }
    }
}