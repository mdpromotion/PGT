using _Project.Features.ProceduralWorld.Domain.Hydrology;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Application.Hydrology
{
    public interface IHydrologyProvider
    {
        bool Enabled { get; }


        HydrologyRegion GetOrCreateRegion(
            float2 worldPosition);


        bool TryGetRegion(
            float2 worldPosition,
            out HydrologyRegion region);
    }
}