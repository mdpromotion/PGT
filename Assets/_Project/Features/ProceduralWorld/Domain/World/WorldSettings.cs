using _Project.Features.ProceduralWorld.Domain.Biomes;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Domain.World
{
    [CreateAssetMenu(
        menuName="Procedural World/World Settings")]
    public class WorldSettings : ScriptableObject
    {
        public int Seed;
        
        public WorldNoiseSettings Noise;
    }
}