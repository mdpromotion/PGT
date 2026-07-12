using _Project.Features.ProceduralWorld.Domain.Biomes;

namespace _Project.Features.ProceduralWorld.Domain.Chunks
{
    public readonly struct ChunkGenerationRequest
    {
        public readonly ChunkCoordinate Coordinate;

        public readonly int Resolution;

        public readonly BiomeDefinition Biome;


        public ChunkGenerationRequest(
            ChunkCoordinate coordinate,
            int resolution,
            BiomeDefinition biome)
        {
            Coordinate = coordinate;
            Resolution = resolution;
            Biome = biome;
        }
    }
}