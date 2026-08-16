using UnityEngine;

namespace _Project.Features.ProceduralWorld.Domain.Vegetation
{
    public sealed class RockVegetationHandle
    {
        public Transform Root;
        public GameObjectInstancePool Pool;
        
        public VegetationData LastAppliedData;
    }
}