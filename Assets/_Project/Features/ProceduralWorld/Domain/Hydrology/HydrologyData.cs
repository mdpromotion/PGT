using System;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using Unity.Collections;

namespace _Project.Features.ProceduralWorld.Domain.Hydrology
{
    public sealed class HydrologyData
    {
        public ChunkCoordinate Coordinate { get; }
        public int Resolution { get; }

        public NativeArray<float> Accumulation { get; }
        public NativeArray<float> RiverMask { get; }
        public NativeArray<float> WaterSurfaceHeight { get; }
        public NativeArray<float> MacroHeightSample { get; }

        public NativeArray<sbyte> FlowDirection;

        private readonly Action _onDispose;
        private bool _disposed;

        public HydrologyData(ChunkCoordinate coordinate, int resolution, Action onDispose)
        {
            Coordinate = coordinate;
            Resolution = resolution;
            _onDispose = onDispose;

            int count = resolution * resolution;

            Accumulation = new NativeArray<float>(count, Allocator.Persistent);
            RiverMask = new NativeArray<float>(count, Allocator.Persistent);
            WaterSurfaceHeight = new NativeArray<float>(count, Allocator.Persistent);
            MacroHeightSample = new NativeArray<float>(count, Allocator.Persistent);
            FlowDirection = new NativeArray<sbyte>(count, Allocator.Persistent);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (Accumulation.IsCreated) Accumulation.Dispose();
            if (RiverMask.IsCreated) RiverMask.Dispose();
            if (WaterSurfaceHeight.IsCreated) WaterSurfaceHeight.Dispose();
            if (MacroHeightSample.IsCreated) MacroHeightSample.Dispose();
            if (FlowDirection.IsCreated) FlowDirection.Dispose();

            _onDispose?.Invoke();
        }
    }
}