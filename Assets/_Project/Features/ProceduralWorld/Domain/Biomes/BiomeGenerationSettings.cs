using UnityEngine;

namespace _Project.Features.ProceduralWorld.Domain.Biomes
{
    [System.Serializable]
    public class BiomeGenerationSettings
    {
        [Header("Noise")]

        public float Scale = 5000f;

        [Range(1,8)]
        public int Octaves = 4;

        [Range(0f,1f)]
        public float Persistence = 0.5f;

        public float Lacunarity = 2f;


        [Header("Terrain Shape")]

        [Range(0.1f,5f)]
        public float RedistributionPower = 1.5f;


        public float HeightMultiplier = 1f;
    }
}