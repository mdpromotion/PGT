using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    public struct HeightIndex : System.IComparable<HeightIndex>
    {
        public int Index;
        public float Height;
        
        public int CompareTo(HeightIndex other) => other.Height.CompareTo(Height);
    }

    [BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    public struct BuildFlowOrderJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<float> FilledHeights;

        [WriteOnly]
        public NativeArray<HeightIndex> Order;

        public void Execute(int index)
        {
            Order[index] = new HeightIndex { Index = index, Height = FilledHeights[index] };
        }
    }

    [BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    public struct SortFlowOrderJob : IJob
    {
        public NativeArray<HeightIndex> Order;

        public void Execute()
        {
            Order.Sort();
        }
    }

    [BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    public struct FlowAccumulationJob : IJob
    {
        [ReadOnly]
        public NativeArray<HeightIndex> Order;

        [ReadOnly]
        public NativeArray<int> FlowTarget;
        
        public NativeArray<float> Accumulation;

        public void Execute()
        {
            for (int i = 0; i < Order.Length; i++)
            {
                int index = Order[i].Index;
                int target = FlowTarget[index];

                if (target < 0)
                    continue;

                Accumulation[target] += Accumulation[index];
            }
        }
    }
}