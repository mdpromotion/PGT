using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile]
    public struct ComputeMacroWaterLevelJob : IJob
    {
        public int PaddedSize;
        public float RiverAccumulationThreshold;
        public int PaddingCells;

        [ReadOnly] public NativeArray<float> Heights;
        [ReadOnly] public NativeArray<float> Accumulation;

        public NativeArray<float> WaterLevels;

        public void Execute()
        {
            int count = PaddedSize * PaddedSize;

            var isRiver = new NativeArray<byte>(count, Allocator.Temp);
            var parent = new NativeArray<int>(count, Allocator.Temp);
            var rank = new NativeArray<byte>(count, Allocator.Temp);
            var minBank = new NativeArray<float>(count, Allocator.Temp);
            
            for (int i = 0; i < count; i++)
            {
                bool river = Accumulation[i] >= RiverAccumulationThreshold;
                isRiver[i] = (byte)(river ? 1 : 0);
                parent[i] = i;
                rank[i] = 0;
                minBank[i] = float.MaxValue;
            }
            
            for (int z = 0; z < PaddedSize; z++)
            {
                for (int x = 0; x < PaddedSize; x++)
                {
                    int i = z * PaddedSize + x;
                    if (isRiver[i] == 0)
                        continue;

                    for (int dz = -1; dz <= 1; dz++)
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dz == 0)
                            continue;

                        int nx = x + dx;
                        int nz = z + dz;
                        if ((uint)nx >= PaddedSize || (uint)nz >= PaddedSize)
                            continue;

                        int n = nz * PaddedSize + nx;
                        if (isRiver[n] != 0)
                            Union(i, n, parent, rank);
                    }
                }
            }
            
            for (int z = 0; z < PaddedSize; z++)
            {
                for (int x = 0; x < PaddedSize; x++)
                {
                    int i = z * PaddedSize + x;
                    if (isRiver[i] != 0)
                        continue;

                    float h = Heights[i];

                    for (int dz = -1; dz <= 1; dz++)
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dz == 0)
                            continue;

                        int nx = x + dx;
                        int nz = z + dz;
                        if ((uint)nx >= PaddedSize || (uint)nz >= PaddedSize)
                            continue;

                        int n = nz * PaddedSize + nx;
                        if (isRiver[n] == 0)
                            continue;

                        int root = Find(n, parent);
                        if (h < minBank[root])
                            minBank[root] = h;
                    }
                }
            }

            var minChannel = new NativeArray<float>(count, Allocator.Temp);
            for (int i = 0; i < count; i++)
                minChannel[i] = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                if (isRiver[i] == 0)
                    continue;

                int root = Find(i, parent);
                float h = Heights[i];
                if (h < minChannel[root])
                    minChannel[root] = h;
            }

            for (int i = 0; i < count; i++)
            {
                if (isRiver[i] == 0)
                    continue;

                int root = Find(i, parent);
                if (minBank[root] == float.MaxValue)
                    minBank[root] = minChannel[root];
            }
            
            const float BankMargin = 0.15f;

            for (int i = 0; i < count; i++)
            {
                if (isRiver[i] != 0)
                {
                    int root = Find(i, parent);
                    float level = minBank[root] - BankMargin;
                    level = math.min(level, minChannel[root] - 0.05f);
                    WaterLevels[i] = level;
                }
                else
                {
                    WaterLevels[i] = Heights[i];
                }
            }
            
            var distances = new NativeArray<float>(count, Allocator.Temp);

            for (int i = 0; i < count; i++)
            {
                distances[i] = isRiver[i] != 0 ? 0f : 1000000f;
            }
            
            for (int z = 0; z < PaddedSize; z++)
            {
                for (int x = 0; x < PaddedSize; x++)
                {
                    int idx = z * PaddedSize + x;
                    float minDist = distances[idx];
                    float bestLevel = WaterLevels[idx];

                    if (x > 0) Update(idx - 1, 1f, ref minDist, ref bestLevel, distances, WaterLevels);
                    if (z > 0) Update(idx - PaddedSize, 1f, ref minDist, ref bestLevel, distances, WaterLevels);
                    if (x > 0 && z > 0) Update(idx - PaddedSize - 1, 1.414f, ref minDist, ref bestLevel, distances, WaterLevels);
                    if (x < PaddedSize - 1 && z > 0) Update(idx - PaddedSize + 1, 1.414f, ref minDist, ref bestLevel, distances, WaterLevels);

                    distances[idx] = minDist;
                    WaterLevels[idx] = bestLevel;
                }
            }
            
            for (int z = PaddedSize - 1; z >= 0; z--)
            {
                for (int x = PaddedSize - 1; x >= 0; x--)
                {
                    int idx = z * PaddedSize + x;
                    float minDist = distances[idx];
                    float bestLevel = WaterLevels[idx];

                    if (x < PaddedSize - 1) Update(idx + 1, 1f, ref minDist, ref bestLevel, distances, WaterLevels);
                    if (z < PaddedSize - 1) Update(idx + PaddedSize, 1f, ref minDist, ref bestLevel, distances, WaterLevels);
                    if (x < PaddedSize - 1 && z < PaddedSize - 1) Update(idx + PaddedSize + 1, 1.414f, ref minDist, ref bestLevel, distances, WaterLevels);
                    if (x > 0 && z < PaddedSize - 1) Update(idx + PaddedSize - 1, 1.414f, ref minDist, ref bestLevel, distances, WaterLevels);

                    distances[idx] = minDist;
                    WaterLevels[idx] = bestLevel;
                }
            }

            isRiver.Dispose();
            parent.Dispose();
            rank.Dispose();
            minBank.Dispose();
            minChannel.Dispose();
            distances.Dispose();
        }

        private static int Find(int i, NativeArray<int> parent)
        {
            while (parent[i] != i)
            {
                parent[i] = parent[parent[i]];
                i = parent[i];
            }
            return i;
        }

        private static void Union(int a, int b, NativeArray<int> parent, NativeArray<byte> rank)
        {
            a = Find(a, parent);
            b = Find(b, parent);
            if (a == b)
                return;

            if (rank[a] < rank[b])
            {
                parent[a] = b;
            }
            else if (rank[a] > rank[b])
            {
                parent[b] = a;
            }
            else
            {
                parent[b] = a;
                rank[a]++;
            }
        }

        private static void Update(
            int neighborIdx,
            float distOffset,
            ref float minDist,
            ref float bestLevel,
            NativeArray<float> distances,
            NativeArray<float> levels)
        {
            float d = distances[neighborIdx] + distOffset;
            if (d < minDist)
            {
                minDist = d;
                bestLevel = levels[neighborIdx];
            }
        }
    }
}