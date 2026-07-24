using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile]
    public struct ComputeAccumulationJob : IJob
    {
        [ReadOnly] public NativeArray<float> Heights;
        [ReadOnly] public NativeArray<sbyte> FlowDirection;
        public int Resolution;
        
        public NativeArray<int> Order;

        public NativeArray<float> Accumulation;

        public void Execute()
        {
            int n = Resolution * Resolution;

            for (int i = 0; i < n; i++)
            {
                Order[i] = i;
                Accumulation[i] = 1f;
            }

            Order.Sort(new HeightDescendingComparer { Heights = Heights });

            for (int k = 0; k < n; k++)
            {
                int i = Order[k];
                int direction = FlowDirection[i];
                if (direction < 0)
                    continue;

                int x = i % Resolution;
                int y = i / Resolution;
                int2 offset = D8Directions.GetOffset(direction);
                int neighborIndex = (y + offset.y) * Resolution + (x + offset.x);

                Accumulation[neighborIndex] += Accumulation[i];
            }
        }

        private struct HeightDescendingComparer : IComparer<int>
        {
            [ReadOnly] public NativeArray<float> Heights;

            public int Compare(int a, int b) =>
                Heights[b].CompareTo(Heights[a]);
        }
    }
}