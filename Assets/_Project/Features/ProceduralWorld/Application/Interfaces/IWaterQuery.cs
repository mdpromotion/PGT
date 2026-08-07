using _Project.Features.ProceduralWorld.Domain.Hydrology;

namespace _Project.Features.ProceduralWorld.Application.Interfaces
{
    public interface IWaterQuery
    {
        bool TryGetWaterState(UnityEngine.Vector3 worldPosition, out WaterSample sample);
    }
}