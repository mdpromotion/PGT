namespace _Project.Features.ProceduralWorld.Domain.Biomes
{
    public interface IBiomeResolver
    {
        BiomeDefinition Resolve(
            float temperature,
            float moisture);
    }
}