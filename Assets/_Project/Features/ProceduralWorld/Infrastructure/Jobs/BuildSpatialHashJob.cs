using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs
{
    [BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    public struct BuildSpatialHashJob : IJob
    {
        [ReadOnly]
        public NativeArray<float2Point> Points;

        public float2 Origin;
        public float CellSize;
        public int GridWidth;
        public int GridHeight;

        public NativeArray<int> CellStart;
        public NativeArray<int> CellCount;
        public NativeList<int> PointIndices;

        public void Execute()
        {
            int cellsLength = CellCount.Length;

            for (int i = 0; i < cellsLength; i++)
                CellCount[i] = 0;

            int n = Points.Length;

            PointIndices.ResizeUninitialized(n);

            NativeArray<int> cellOf = new NativeArray<int>(n, Allocator.Temp);

            for (int i = 0; i < n; i++)
            {
                int cell = GetCell(Points[i]);
                cellOf[i] = cell;
                CellCount[cell]++;
            }

            int sum = 0;

            for (int c = 0; c < cellsLength; c++)
            {
                CellStart[c] = sum;
                sum += CellCount[c];
            }

            NativeArray<int> cursor = new NativeArray<int>(cellsLength, Allocator.Temp);

            for (int c = 0; c < cellsLength; c++)
                cursor[c] = CellStart[c];

            NativeArray<int> indices = PointIndices.AsArray();

            for (int i = 0; i < n; i++)
            {
                int cell = cellOf[i];
                indices[cursor[cell]] = i;
                cursor[cell]++;
            }

            cellOf.Dispose();
            cursor.Dispose();
        }

        private int GetCell(float2Point p)
        {
            int cx = (int)math.floor((p.X - Origin.x) / CellSize);
            int cz = (int)math.floor((p.Z - Origin.y) / CellSize);

            cx = math.clamp(cx, 0, GridWidth - 1);
            cz = math.clamp(cz, 0, GridHeight - 1);

            return cz * GridWidth + cx;
        }
    }
}