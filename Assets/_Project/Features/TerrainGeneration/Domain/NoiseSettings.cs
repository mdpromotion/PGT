using UnityEngine;

namespace _Project.Features.TerrainGeneration.Domain
{
    [System.Serializable]
    public class NoiseSettings
    {
        public int Seed = 0;
        public float Scale = 50f;
        public int Octaves = 4;
        [Range(0f, 1f)] public float Persistence = 0.5f;
        public float Lacunarity = 2f;
        public float HeightMultiplier = 1f;
    }
}