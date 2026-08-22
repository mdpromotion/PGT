using UnityEngine;

namespace _Project.Features.ProceduralWorld.Domain.World
{
    [CreateAssetMenu(menuName="Procedural World/World Settings")]
    public class WorldSettingsConfig : ScriptableObject
    {
        public int Octaves;
        public float Scale;
        public float Persistence;
        public float Lacunarity;
        public float RedistributionPower;
    }
}
