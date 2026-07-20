using UnityEngine;
using _Project.Features.ProceduralWorld.Domain.Landscape;

namespace _Project.Features.ProceduralWorld.Application.Interfaces
{
    public interface IWaterQuery
    {
        bool TryGetWaterState(Vector3 worldPosition, out WaterSample sample);
    }
}