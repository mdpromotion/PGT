using System.Collections.Generic;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Domain.Vegetation
{
    public sealed class VegetationHandle
    {
        public readonly TerrainCollider Collider;
        public readonly List<TreeInstance> TreeInstanceBuffer = new();
        public bool PrototypesAssigned;

        public VegetationHandle(TerrainCollider collider)
        {
            Collider = collider;
        }
    }
}