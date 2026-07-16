using System.Collections.Generic;

namespace _Project.Features.ProceduralWorld.Domain.Chunks
{
    public sealed class ChunkGenerationPipeline
    {
        private readonly List<IChunkGenerationStage> _stages;


        public ChunkGenerationPipeline(
            IEnumerable<IChunkGenerationStage> stages)
        {
            _stages =
                new List<IChunkGenerationStage>(
                    stages);
        }



        public ChunkGenerationState Execute(
            ChunkGenerationState state)
        {
            foreach(IChunkGenerationStage stage in _stages)
            {
                stage.Execute(state);
            }


            return state;
        }
    }
}