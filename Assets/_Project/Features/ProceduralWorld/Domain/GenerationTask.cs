using _Project.Features.ProceduralWorld.Domain.Landscape;
using Unity.Jobs;

namespace _Project.Features.ProceduralWorld.Domain
{
    public class GenerationTask
    {
        public JobHandle Handle;

        public LandscapeData Result;

        public bool Cancelled;


        public GenerationTask(
            JobHandle handle,
            LandscapeData result)
        {
            Handle = handle;

            Result = result;
        }
    }
}