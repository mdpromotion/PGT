using System;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Settings
{
    [Serializable]
    public struct HydrologySettings
    {
        public int RegionSizeInChunks;
        public int RegionCoarseResolution;
        public int RiverSourceCount;
        public float CarveDepth;
        public float RiverWidth;
        public int MaxTraceSteps;
        public int Seed;

        [Header("Water source (lake)")]
        public float LakeRadius;
        public float LakeDepth;
        
        public float SpringBasinFlatness;

        [Header("Continuous tracing / meander")]
        public float StepDistance;
        public float TurnSmoothing;
        public float MeanderStrength;
        public float GradientEpsilon;

        public float BaseStrengthGrowth;
        public float SlopeStrengthFactor;
        public float MaxRiverStrength;
        public float ConfluenceDistance;
        public float LakeMergeDistance;
        public float ConfluenceStrengthFactor;

        [Header("Fade in/out")]
        public int RiverStartFadeSteps;
        public float InitialRiverStrength;
        
        public int RiverEndFadeSteps;
    }
}