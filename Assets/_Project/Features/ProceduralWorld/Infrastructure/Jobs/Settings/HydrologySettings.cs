using System;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Settings
{
    [Serializable]
    public struct HydrologySettings
    {
        [Header("Region")]
        public int RegionSizeInChunks;
        public int RegionCoarseResolution;

        [Header("Flow")]
        public float FlowAccumulationThreshold;
        public float AccumulationToStrengthScale;
        public float MaxRiverStrength;

        [Header("Carving")]
        public float CarveDepth;
        public float RiverWidth;
        public float InitialCarveDepthFactor;
        public float EdgeSinkFactor;
        public float EdgeOverlapFactor;

        [Header("Tracing")]
        public int MaxTraceSteps;
        public float GradientEpsilon;

        [Header("Fade in/out")]
        public int RiverStartFadeSteps;
        public float InitialRiverStrength;
    }
}