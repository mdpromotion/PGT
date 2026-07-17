using System;
using Unity.Collections;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public readonly struct SpatialHashData : IDisposable
    {
        public readonly NativeArray<int> CellStart;
        public readonly NativeArray<int> CellCount;
        public readonly NativeList<int> PointIndices;

        public readonly float2 Origin;
        public readonly float CellSize;
        public readonly int GridWidth;
        public readonly int GridHeight;

        public SpatialHashData(
            NativeArray<int> cellStart,
            NativeArray<int> cellCount,
            NativeList<int> pointIndices,
            float2 origin,
            float cellSize,
            int gridWidth,
            int gridHeight)
        {
            CellStart = cellStart;
            CellCount = cellCount;
            PointIndices = pointIndices;
            Origin = origin;
            CellSize = cellSize;
            GridWidth = gridWidth;
            GridHeight = gridHeight;
        }

        public JobHandleDisposeResult DisposeWith(Unity.Jobs.JobHandle handle)
        {
            Unity.Jobs.JobHandle h = CellStart.Dispose(handle);
            h = CellCount.Dispose(h);
            h = PointIndices.Dispose(h);
            return new JobHandleDisposeResult(h);
        }

        public void Dispose()
        {
            CellStart.Dispose();
            CellCount.Dispose();
            PointIndices.Dispose();
        }
    }

    public readonly struct JobHandleDisposeResult
    {
        public readonly Unity.Jobs.JobHandle Handle;

        public JobHandleDisposeResult(Unity.Jobs.JobHandle handle)
        {
            Handle = handle;
        }
    }
}