using _Project.Features.ProceduralWorld.Application.Chunks.Modifiers;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.Hydrology;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;


namespace _Project.Features.ProceduralWorld.Application.Hydrology
{
    public sealed class RiverHeightModifier : IHeightModifier
    {
        private readonly IHydrologyProvider _hydrology;


        public bool Enabled { get; }



        public RiverHeightModifier(
            IHydrologyProvider hydrology,
            bool enabled)
        {
            _hydrology = hydrology;

            Enabled = enabled;
        }



        public JobHandle Schedule(
            ChunkGenerationContext context,
            NativeArray<float> heights,
            JobHandle dependency)
        {
            HydrologyRegion region =
                _hydrology.GetOrCreateRegion(
                    context.WorldPosition);



            if (region != null)
            {
                Debug.Log(
                    $"[RiverModifier] Chunk {context.Coordinate.X}:{context.Coordinate.Y} -> Region {region.Coordinate.X}:{region.Coordinate.Y}");
            }



            return dependency;
        }
    }
}