using UnityEngine;

namespace _Project.Features.ProceduralWorld.Domain.Vegetation
{
    public sealed class VegetationHandle
    {
        public readonly TerrainCollider Collider;

        public readonly TreeVegetationHandle Trees = new();
        public readonly DetailVegetationHandle Details = new();
        public readonly RockVegetationHandle Rocks = new();

        public VegetationHandle(TerrainCollider collider)
        {
            Collider = collider;
        }
    }
}