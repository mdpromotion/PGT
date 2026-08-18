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

        [ReadOnly] public NativeArray<float> Heights;
        [ReadOnly] public NativeArray<float> Accumulation;

        public NativeArray<float> WaterLevels;

        public void Execute()
        {
            int count = PaddedSize * PaddedSize;
            var distances = new NativeArray<float>(count, Allocator.Temp);
            
            for (int i = 0; i < count; i++)
            {
                if (Accumulation[i] >= RiverAccumulationThreshold)
                {
                    WaterLevels[i] = Heights[i];
                    distances[i] = 0f;
                }
                else
                {
                    WaterLevels[i] = Heights[i];
                    distances[i] = 1000000f;
                }
            }
            
            for (int z = 0; z < PaddedSize; z++)
            {
                for (int x = 0; x < PaddedSize; x++)
                {
                    int idx = z * PaddedSize + x;
                    float minDist = distances[idx];
                    float bestLevel = WaterLevels[idx];
                    
                    if (x > 0) UpdateDistance(idx - 1, 1f, ref minDist, ref bestLevel, distances, WaterLevels);
                    
                    if (z > 0) UpdateDistance(idx - PaddedSize, 1f, ref minDist, ref bestLevel, distances, WaterLevels);
                    
                    if (x > 0 && z > 0) UpdateDistance(idx - PaddedSize - 1, 1.414f, ref minDist, ref bestLevel, distances, WaterLevels);
                    
                    if (x < PaddedSize - 1 && z > 0) UpdateDistance(idx - PaddedSize + 1, 1.414f, ref minDist, ref bestLevel, distances, WaterLevels);

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
                    
                    if (x < PaddedSize - 1) UpdateDistance(idx + 1, 1f, ref minDist, ref bestLevel, distances, WaterLevels);
                    
                    if (z < PaddedSize - 1) UpdateDistance(idx + PaddedSize, 1f, ref minDist, ref bestLevel, distances, WaterLevels);
                    
                    if (x < PaddedSize - 1 && z < PaddedSize - 1) UpdateDistance(idx + PaddedSize + 1, 1.414f, ref minDist, ref bestLevel, distances, WaterLevels);
                    
                    if (x > 0 && z < PaddedSize - 1) UpdateDistance(idx + PaddedSize - 1, 1.414f, ref minDist, ref bestLevel, distances, WaterLevels);

                    distances[idx] = minDist;
                    WaterLevels[idx] = bestLevel;
                }
            }

            distances.Dispose();
        }

        private void UpdateDistance(int neighborIdx, float distOffset, ref float minDist, ref float bestLevel, NativeArray<float> distances, NativeArray<float> levels)
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