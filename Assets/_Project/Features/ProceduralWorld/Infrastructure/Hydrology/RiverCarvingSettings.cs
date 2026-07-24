using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    [CreateAssetMenu(menuName = "Procedural World/River Carving Settings")]
    public class RiverCarvingSettings : ScriptableObject
    {
        public float AccumulationThreshold = 40f;
        
        public float FalloffRange = 200f;
        
        public float MaxCarveDepth = 0.03f;
    }
}