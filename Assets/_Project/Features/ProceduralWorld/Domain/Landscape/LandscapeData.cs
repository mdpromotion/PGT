using Unity.Collections;
using _Project.Features.ProceduralWorld.Domain.Chunks;

namespace _Project.Features.ProceduralWorld.Domain.Landscape
{
    public sealed class LandscapeData
    {
        public ChunkCoordinate Coordinate { get; }

        public NativeArray<float> Heights { get; }

        public int Resolution { get; }

        public LandscapeData(
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
            {
                Heights.Dispose();
            }
        }
    }
}