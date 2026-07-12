using _Project.Features.ProceduralWorld.Domain.Biomes;
using _Project.Features.ProceduralWorld.Domain.World;
using _Project.Features.ProceduralWorld.Infrastructure.World;

public class WorldGenerator
{
    private readonly ClimateGenerator _climate;
    private readonly IBiomeResolver _biomeResolver;


    public WorldGenerator(
        ClimateGenerator climate,
        IBiomeResolver biomeResolver)
    {
        _climate = climate;
        _biomeResolver = biomeResolver;
    }


    public BiomeDefinition ResolveBiome(
        WorldPosition position)
    {
        ClimateSample climate =
            _climate.Sample(
                position.X,
                position.Z);


        return _biomeResolver.Resolve(
            climate.Temperature,
            climate.Moisture);
    }
}