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

        public float MaxCarveDepth = 0.03f;

        [Header("Embankment")]
        [Tooltip("Насколько насыпь выше исходного рельефа в пике.")]
        public float EmbankmentHeight = 0.05f;

        [Range(0.01f, 0.99f)]
        [Tooltip("Позиция пика насыпи по carveMask (0 = у самой кромки суши, 1 = в русле).")]
        public float EmbankmentPeakPosition = 0.25f;

        [Tooltip("Во сколько раз глубина карва должна быть меньше насыпи, чтобы дно оставалось ниже воды.")]
        public float MinDepthBelowWaterFactor = 0.3f;
    }
}