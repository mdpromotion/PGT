using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile]
    public struct ComputeWaterBoundsJob : IJob
    {
        [ReadOnly] public NativeArray<float> RiverMask;
        [ReadOnly] public NativeArray<float> WaterSurfaceHeight;
        public int Resolution;
        public int Padding;
        
        public NativeArray<int> Bounds;
        public NativeArray<float> AverageHeight;

        public void Execute()
        {
            int minX = Resolution;
            int minZ = Resolution;
            int maxX = -1;
            int maxZ = -1;

            float heightSum = 0f;
            int heightCount = 0;

            for (int z = 0; z < Resolution; z++)
            {
                int rowOffset = z * Resolution;
                for (int x = 0; x < Resolution; x++)
                {
                    float mask = RiverMask[rowOffset + x];
                    if (mask <= 0f)
                        continue;

                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (z < minZ) minZ = z;
                    if (z > maxZ) maxZ = z;

                    heightSum += WaterSurfaceHeight[rowOffset + x];
                    heightCount++;
                }
            }

            bool hasWater = maxX >= 0;
            Bounds[0] = hasWater ? 1 : 0;

            if (hasWater)
            {
                minX = math.max(0, minX - Padding);
                minZ = math.max(0, minZ - Padding);
                maxX = math.min(Resolution - 1, maxX + Padding);
                maxZ = math.min(Resolution - 1, maxZ + Padding);
            }

            Bounds[1] = minX;
            Bounds[2] = minZ;
            Bounds[3] = maxX;
            Bounds[4] = maxZ;

            AverageHeight[0] = heightCount > 0 ? heightSum / heightCount : 0f;
        }
    }
}