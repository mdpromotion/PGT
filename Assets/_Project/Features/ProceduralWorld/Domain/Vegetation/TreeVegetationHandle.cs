using System.Collections.Generic;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Domain.Vegetation
{
    public sealed class TreeVegetationHandle
    {
        public readonly List<TreeInstance> InstanceBuffer = new();

        public bool PrototypesAssigned;
        
        public VegetationData LastAppliedData;
    }
}