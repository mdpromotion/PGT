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

        // --- Новое ---

        [Header("Water source (lake)")]
        public float LakeRadius;
        public float LakeDepth;

        [Header("Continuous tracing / meander")]
        public float StepDistance;      // длина одного шага реки в мировых единицах
        public float TurnSmoothing;     // 0..1, чем ближе к 1 — тем плавнее повороты (инерция направления)
        public float MeanderStrength;   // 0..1, случайное виляние поперёк направления течения
        public float GradientEpsilon;   // шаг для численного градиента (обычно ~0.5–1 юнита)
    }
}