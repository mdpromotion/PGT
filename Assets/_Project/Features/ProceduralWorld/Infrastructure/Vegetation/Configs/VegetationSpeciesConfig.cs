using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Domain.Vegetation;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Vegetation.Configs
{
    [CreateAssetMenu(menuName = "Procedural World/Vegetation/Species Config")]
    public class VegetationSpeciesConfig : ScriptableObject
    {
        [SerializeField] private GameObject[] prefabs;
        [SerializeField] private VegetationSpeciesType speciesType;
        
        [SerializeField] private float coverage;
        [SerializeField] private float density;
        
        [SerializeField] private float edgeSmooting;
        
        [SerializeField] private float minScale;
        [SerializeField] private float maxScale;
        
        [SerializeField] private float minSlopeAngle;
        [SerializeField] private float maxSlopeAngle;

        [SerializeField] private float patchNoiseFrequency;
        [SerializeField] private int patchNoiseOctaves;

        [SerializeField] private int priority;
        
        [SerializeField] private float occupancyRadius;

        [SerializeField] private bool isBreakable; // can be promoted to c# object in runtime
        [SerializeField] private bool isDetail;
        
        public IReadOnlyList<GameObject> Prefabs => prefabs;
        
        public VegetationSpeciesType SpeciesType => speciesType;
        
        public float Coverage => coverage;
        public float Density => density;
        
        public float EdgeSmooting => edgeSmooting;
        
        public float MinScale => minScale;
        public float MaxScale => maxScale;
        
        public float MinSlopeAngle => minSlopeAngle;
        public float MaxSlopeAngle => maxSlopeAngle;

        public int Priority => priority;

        public float PatchNoiseFrequency => patchNoiseFrequency;
        public int PatchNoiseOctaves => patchNoiseOctaves;
        
        public float OccupancyRadius => occupancyRadius;
        
        public bool IsBreakable => isBreakable;
        public bool IsDetail => isDetail; 
    }
}