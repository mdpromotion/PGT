using System;
using _Project.Features.ProceduralWorld.Domain.Vegetation;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Vegetation
{
    [Serializable]
    public sealed class VegetationCatalogEntry
    {
        public VegetationCategory Category = VegetationCategory.Tree;
        
        public GameObject Prefab;
        
        [Min(0f)]
        public float Weight = 1f;
        
        [Range(0f, 1f)] public float MinHeight01 = 0f;
        [Range(0f, 1f)] public float MaxHeight01 = 1f;
        
        [Range(0f, 90f)] public float MinSlopeDegrees = 0f;
        [Range(0f, 90f)] public float MaxSlopeDegrees = 35f;
        
        public Vector2 UniformScaleRange = new Vector2(0.85f, 1.2f);
        public bool RandomizeYRotation = true;

        public bool Matches(float height01, float slopeDegrees)
        {
            return height01 >= MinHeight01 && height01 <= MaxHeight01 &&
                   slopeDegrees >= MinSlopeDegrees && slopeDegrees <= MaxSlopeDegrees;
        }
    }
}