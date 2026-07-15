using _Project.Features.ProceduralWorld.Domain.Biomes.Settings;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Domain.Biomes
{
    public class BiomeDefinition : ScriptableObject
    {
        [Header("Identity")]

        public BiomeType Type;

        public string Id;

        public string DisplayName;


        [Header("Generation")]

        public BiomeGenerationSettings Generation;
        
        [Header("Climate")]

        public BiomeClimateSettings Climate;
    }
}