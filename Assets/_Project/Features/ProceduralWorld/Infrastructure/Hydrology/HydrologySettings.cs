using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    [CreateAssetMenu(
        menuName = "Procedural World/Hydrology Settings")]
    public class HydrologySettings : ScriptableObject
    {
        [Header("General")]

        public bool Enabled = true;


        [Min(64f)]
        public float RegionSize = 1024f;

        

        [Min(1)]
        public int SourceAttempts = 32;


        [Range(0f,1f)]
        public float MinimumSourceHeight = 0.65f;
    }
}