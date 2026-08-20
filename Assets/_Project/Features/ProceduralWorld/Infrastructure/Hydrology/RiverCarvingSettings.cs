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

        [Header("Channel / Embankment (relative to water level)")]
        [Tooltip("Насколько дно русла ниже уровня воды в центре реки.")]
        public float MaxCarveDepth = 1f;

        [Tooltip("Насколько пик насыпи выше уровня воды.")]
        public float EmbankmentHeight = 1.5f;

        [Range(0.01f, 0.99f)]
        public float EmbankmentPeakPosition = 0.25f;

        [Header("Shore")]
        [Range(0f, 1f)]
        [Tooltip("1 = рельеф у берега полностью подтягивается к уровню воды " +
                 "(рекомендуется — устраняет расхождение шума суши и воды).")]
        public float ShoreConformStrength = 1f;
    }
}