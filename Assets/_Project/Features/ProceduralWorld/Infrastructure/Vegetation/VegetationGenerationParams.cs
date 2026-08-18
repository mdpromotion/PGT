using Unity.Collections;

namespace _Project.Features.ProceduralWorld.Infrastructure.Vegetation
{
    public struct VegetationGenerationParams
    {
        public float Coverage;
        public float Density;
        
        public float EdgeSmoothing;
        
        public float MinScale;
        public float MaxScale;
        
        public float MinSlopeAngle;
        public float MaxSlopeAngle;
        
        public float PatchNoiseFrequency;
        public int PatchNoiseOctaves;

        public int Priority;    

        public float OccupancyRadius;
        
        public VegetationGenerationParams(
            float coverage,
            float density,
            float edgeSmoothing,
            float minScale,
            float maxScale,
            float minSlopeFalloff,
            float maxSlopeFalloff,
            float patchNoiseFrequency,
            int patchNoiseOctaves,
            int priority,
            float occupancyRadius)
        {
            Coverage = coverage;
            Density = density;
            EdgeSmoothing = edgeSmoothing;
            MinScale = minScale;
            MaxScale = maxScale;
            MinSlopeAngle = minSlopeFalloff;
            MaxSlopeAngle = maxSlopeFalloff;
            PatchNoiseFrequency = patchNoiseFrequency;
            PatchNoiseOctaves = patchNoiseOctaves;
            Priority = priority;
            OccupancyRadius = occupancyRadius;
        }
    }
}