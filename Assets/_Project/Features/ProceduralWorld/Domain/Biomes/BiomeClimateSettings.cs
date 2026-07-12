using UnityEngine;


namespace _Project.Features.ProceduralWorld.Domain.Biomes
{
    [System.Serializable]
    public class BiomeClimateSettings
    {
        [Range(0f,1f)]
        public float Temperature;


        [Range(0f,1f)]
        public float Moisture;
    }
}