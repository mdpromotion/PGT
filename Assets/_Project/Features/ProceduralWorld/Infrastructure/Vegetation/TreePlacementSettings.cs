using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Vegetation
{
    [CreateAssetMenu(menuName = "ProceduralWorld/TreePlacementSettings")]
    public sealed class TreePlacementSettings : ScriptableObject
    {
        public float CellSize = 4f;
        public float MaxSlopeDegrees = 35f;
        public float MinHeight = 0f;
        public float MaxHeight = 500f;
        public float DensityNoiseScale = 0.01f;
        [Range(0f, 1f)] public float DensityThreshold = 0.4f;
        public int PrototypeCount = 1;
        public uint WorldSeed = 1;

        public float MinScale = 0.8f;
        public float MaxScale = 1.2f;
    }
}