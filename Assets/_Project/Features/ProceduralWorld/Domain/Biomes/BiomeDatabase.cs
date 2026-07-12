using UnityEngine;

namespace _Project.Features.ProceduralWorld.Domain.Biomes
{
    [CreateAssetMenu(
        fileName = "BiomeDatabase",
        menuName = "Procedural World/Biome Database")]
    public class BiomeDatabase : ScriptableObject
    {
        public BiomeDefinition[] Biomes;
    }
}