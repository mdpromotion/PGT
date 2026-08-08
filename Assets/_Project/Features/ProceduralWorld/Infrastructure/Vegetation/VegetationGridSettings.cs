// Infrastructure/Vegetation/VegetationGridSettings.cs
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Vegetation
{
    [CreateAssetMenu(fileName = "VegetationGridSettings", menuName = "ProceduralWorld/Vegetation Grid Settings")]
    public sealed class VegetationGridSettings : ScriptableObject
    {
        [Min(1)] 
        public int ChunksPerTile = 4;
        
        [Min(0.5f)] 
        public float PointSpacing = 2.5f;

        [Range(0f, 1f)]
        public float JitterStrength = 0.9f;
        
        [Min(1f)] public float PatchScale = 220f;

        [Range(1, 4)]
        public int PatchOctaves = 3;

        [Range(0f, 1f)]
        public float PatchThreshold = 0.55f;
        
        [Range(0f, 1f)]
        public float RiverMaskThreshold = 0.05f;
    }
}