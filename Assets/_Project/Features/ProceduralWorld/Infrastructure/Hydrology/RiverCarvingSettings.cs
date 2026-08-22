using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    [CreateAssetMenu(menuName = "Procedural World/River Carving Settings")]
    public class RiverCarvingSettings : ScriptableObject
    {
        [Range(0f, 1f)]
        public float AccumulationThreshold = 0.15f;

        [Range(0.01f, 1f)]
        public float FalloffRange = 0.35f;
        
        public float MaxCarveDepth = 1f;
        
    }
}