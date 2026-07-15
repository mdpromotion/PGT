using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using Unity.Collections;
using Unity.Jobs;

namespace _Project.Features.ProceduralWorld.Application.Chunks.Modifiers
{
    public sealed class HeightModifierPipeline
    {
        private readonly List<IHeightModifier> _modifiers = new();



        public void Add(
            IHeightModifier modifier)
        {
            _modifiers.Add(modifier);
        }



        public JobHandle Schedule(
            ChunkGenerationContext context,
            NativeArray<float> heights,
            JobHandle dependency)
        {
            JobHandle handle =
                dependency;


            foreach (IHeightModifier modifier in _modifiers)
            {
                if (!modifier.Enabled)
                    continue;


                handle =
                    modifier.Schedule(
                        context,
                        heights,
                        handle);
            }


            return handle;
        }
    }
}