using _Project.Features.ProceduralWorld.Domain.Biomes;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Domain.World
{
    [CreateAssetMenu(
        menuName="Procedural World/World Settings")]
    public class WorldSettings : ScriptableObject
    {
        public int Seed;
        
        public int Octaves;
        public float Scale;
        public float Persistence;
        public float Lacunarity;
        public float RedistributionPower;
    }
}