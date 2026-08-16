using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Vegetation
{
    [CreateAssetMenu(fileName = "VegetationGridSettings", menuName = "ProceduralWorld/Vegetation Grid Settings")]
    public sealed class VegetationGridSettings : ScriptableObject
    {
        [Min(1)]
        public int ChunksPerTile = 32;

        [Min(0.5f)]
        public float PointSpacing = 20f;

        [Range(0f, 1f)]
        public float JitterStrength = 0.9f;
        
        [Min(1f)]
        public float ForestRegionScale = 3000f;

        [Range(0f, 1f)]
        public float ForestCoverage = 0.6f;

        [Range(1, 4)]
        public int ForestRegionOctaves = 2;
        
        [Min(1f)]
        public float PatchScale = 400f;

        [Range(1, 4)]
        public int PatchOctaves = 3;

        [Range(0f, 1f)]
        public float PatchDetailInfluence = 0.2f;

        [Range(0f, 1f)]
        public float PatchThreshold = 0.45f;

        [Range(0f, 1f)]
        public float RiverMaskThreshold = 0.05f;
        
        [Min(64)]
        public int DetailMapResolution = 256;

        [Range(8, 128)]
        public int DetailResolutionPerPatch = 16;
    }
}