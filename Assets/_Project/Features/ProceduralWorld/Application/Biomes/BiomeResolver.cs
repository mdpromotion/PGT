using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Biomes;
using Unity.Mathematics;


public class BiomeResolver : IBiomeResolver
{
    private readonly BiomeDatabase _database;


    public BiomeResolver(
        BiomeDatabase database)
    {
        _database = database;
    }


    public BiomeDefinition Resolve(
        float temperature,
        float moisture)
    {
        BiomeDefinition result = null;

        float bestDistance = float.MaxValue;


        foreach(var biome in _database.Biomes)
        {
            float distance =
                math.distance(
                    new float2(
                        temperature,
                        moisture),

                    new float2(
                        biome.Climate.Temperature,
                        biome.Climate.Moisture));


            if(distance < bestDistance)
            {
                bestDistance = distance;
                result = biome;
            }
        }


        return result;
    }
    
}