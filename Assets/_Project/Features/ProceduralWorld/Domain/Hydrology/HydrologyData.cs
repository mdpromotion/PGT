using System;
using Unity.Collections;
using _Project.Features.ProceduralWorld.Domain.Chunks;

namespace _Project.Features.ProceduralWorld.Domain.Hydrology
{
    public sealed class HydrologyData
    {
        public ChunkCoordinate Coordinate { get; }
        public NativeArray<float> Accumulation { get; }
        public NativeArray<sbyte> FlowDirection { get; }
        public NativeArray<float> RiverMask { get; }
        public NativeArray<float> WaterSurfaceHeight { get; }
        public int Resolution { get; }

        private readonly Action _onDispose;
        private bool _disposed;

        public HydrologyData(
            ChunkCoordinate coordinate,
            NativeArray<float> accumulation,
            NativeArray<sbyte> flowDirection,
            NativeArray<float> riverMask,
            NativeArray<float> waterSurfaceHeight,
            int resolution,
            Action onDispose)
        {
            Coordinate = coordinate;
            Accumulation = accumulation;
            FlowDirection = flowDirection;
            RiverMask = riverMask;
            WaterSurfaceHeight = waterSurfaceHeight;
            Resolution = resolution;
            _onDispose = onDispose;
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;

            if (Accumulation.IsCreated) Accumulation.Dispose();
            if (FlowDirection.IsCreated) FlowDirection.Dispose();
            if (RiverMask.IsCreated) RiverMask.Dispose();
            if (WaterSurfaceHeight.IsCreated) WaterSurfaceHeight.Dispose();

            _onDispose?.Invoke();
        }
    }
}