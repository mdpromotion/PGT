using UnityEngine;

namespace _Project.Features.ProceduralWorld.Domain.World
{
    public interface IWorldSettings
    {
        void SetSeed(int seed);
    }
    
    public class WorldSettings : IWorldSettings
    {
        public int Seed { get; private set; }
        
        public readonly int Octaves;
        public readonly float Scale;
        public readonly float Persistence;
        public readonly float Lacunarity;
        public readonly float RedistributionPower;

        public WorldSettings(WorldSettingsConfig config)
        {
            Octaves = config.Octaves;
            Scale = config.Scale;
            Persistence = config.Persistence;
            Lacunarity = config.Lacunarity;
            RedistributionPower = config.RedistributionPower;
        }

        public void SetSeed(int seed)
        {
            Debug.Log($"Setting seed {seed}");
            Seed = seed;
        }
    }
}