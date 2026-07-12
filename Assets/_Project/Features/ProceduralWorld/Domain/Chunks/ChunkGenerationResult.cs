using Unity.Collections;

namespace _Project.Features.ProceduralWorld.Domain.Chunks
{
    public struct ChunkGenerationResult
    {
        public ChunkCoordinate Coordinate;

        public NativeArray<float> Heights;

        public int Resolution;

        public ChunkGenerationResult(
            ChunkCoordinate coordinate,
            NativeArray<float> heights,
            int resolution)
        {
            Coordinate = coordinate;
            Heights = heights;
            Resolution = resolution;
        }
    }
}