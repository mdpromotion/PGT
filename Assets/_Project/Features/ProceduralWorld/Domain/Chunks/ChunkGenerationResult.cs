using System;
using Unity.Collections;

namespace _Project.Features.ProceduralWorld.Domain.Chunks
{
    public class ChunkGenerationResult : IDisposable
    {
        public ChunkCoordinate Coordinate { get; }
        public NativeArray<float> Heights { get; }
        public int Resolution { get; }

        public ChunkGenerationResult(
            ChunkCoordinate coordinate,
            NativeArray<float> heights,
            int resolution)
        {
            Coordinate = coordinate;
            Heights = heights;
            Resolution = resolution;
        }

        public void Dispose()
        {
            if (Heights.IsCreated)
                Heights.Dispose();
            
        }
    }
}