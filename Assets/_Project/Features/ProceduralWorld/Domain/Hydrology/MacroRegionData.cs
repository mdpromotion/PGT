using Unity.Collections;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Domain.Hydrology
{
    public sealed class MacroRegionData
    {
        public MacroRegionCoordinate Coordinate { get; }
        public int PaddedSize { get; }
        public float CellSize { get; }
        public double2 WorldOrigin { get; }

        public NativeArray<float> Heights;
        public NativeArray<sbyte> FlowDirection;
        public NativeArray<float> Accumulation;
        public NativeArray<float> WaterLevels;
        public NativeArray<float> SmoothedAccumulation;
        public NativeArray<float> RiverStrengthRaw;
        public NativeArray<float> RiverStrengthSmoothed;

        private bool _disposed;

        public MacroRegionData(
            MacroRegionCoordinate coordinate,
            int paddedSize,
            float cellSize,
            double2 worldOrigin)
        {
            Coordinate = coordinate;
            PaddedSize = paddedSize;
            CellSize = cellSize;
            WorldOrigin = worldOrigin;

            int count = paddedSize * paddedSize;
            Heights = new NativeArray<float>(count, Allocator.Persistent);
            FlowDirection = new NativeArray<sbyte>(count, Allocator.Persistent);
            Accumulation = new NativeArray<float>(count, Allocator.Persistent);
            WaterLevels = new NativeArray<float>(count, Allocator.Persistent);
            SmoothedAccumulation = new NativeArray<float>(count, Allocator.Persistent);
            RiverStrengthRaw = new  NativeArray<float>(count, Allocator.Persistent);
            RiverStrengthSmoothed = new NativeArray<float>(count, Allocator.Persistent);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (Heights.IsCreated) Heights.Dispose();
            if (FlowDirection.IsCreated) FlowDirection.Dispose();
            if (Accumulation.IsCreated) Accumulation.Dispose();
            if (WaterLevels.IsCreated) WaterLevels.Dispose();
            if (SmoothedAccumulation.IsCreated) SmoothedAccumulation.Dispose();
            if (RiverStrengthRaw.IsCreated)  RiverStrengthRaw.Dispose();
            if (RiverStrengthSmoothed.IsCreated)  RiverStrengthSmoothed.Dispose();
        }
    }
}