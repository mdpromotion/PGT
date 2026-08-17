using Unity.Collections;
using UnityEngine;
using _Project.Features.ProceduralWorld.Domain.Hydrology;
using _Project.Features.ProceduralWorld.Domain.Landscape;

namespace _Project.Features.ProceduralWorld.Domain.Chunks
{
    public sealed class ChunkGenerationState
    {
        public ChunkGenerationContext Context { get; }

        public LandscapeData Landscape { get; set; }
        public HydrologyData Hydrology { get; set; }

        public NativeArray<Color32> WaterMaskPixels { get; set; }
        public NativeArray<int> WaterBounds { get; set; }
        public NativeArray<float> WaterAverageHeight { get; set; }

        public ChunkGenerationState(ChunkGenerationContext context)
        {
            Context = context;
        }
        
        public void DisposeAll()
        {
            Landscape?.Dispose();
            Hydrology?.Dispose();

            if(WaterMaskPixels.IsCreated)
                WaterMaskPixels.Dispose();

            if(WaterBounds.IsCreated)
                WaterBounds.Dispose();

            if(WaterAverageHeight.IsCreated)
                WaterAverageHeight.Dispose();
        }
    }
}