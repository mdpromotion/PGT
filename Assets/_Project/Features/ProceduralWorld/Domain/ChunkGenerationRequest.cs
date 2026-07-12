namespace _Project.Features.ProceduralWorld.Domain
{
    public readonly struct ChunkGenerationRequest
    {
        public readonly ChunkCoordinate Coordinate;
        public readonly NoiseSettings Settings;
        public readonly int Resolution;

        public ChunkGenerationRequest(
            ChunkCoordinate coordinate,
            NoiseSettings settings,
            int resolution)
        {
            Coordinate = coordinate;
            Settings = settings;
            Resolution = resolution;
        }
    }
}