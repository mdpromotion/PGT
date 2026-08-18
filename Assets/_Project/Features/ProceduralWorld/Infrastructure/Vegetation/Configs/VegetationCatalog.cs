using System.Collections.Generic;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Vegetation.Configs
{ 
    public sealed class VegetationCatalog : ScriptableObject
    {
        [SerializeField] private VegetationSpeciesConfig[] species;

        public IReadOnlyList<VegetationSpeciesConfig> Species => species;
    }
}