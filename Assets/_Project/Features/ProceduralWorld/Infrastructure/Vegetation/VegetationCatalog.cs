using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Domain.Vegetation;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Vegetation
{
    [CreateAssetMenu(fileName = "VegetationCatalog", menuName = "Procedural World/Vegetation Catalog")]
    public sealed class VegetationCatalog : ScriptableObject
    {
        [SerializeField] private List<VegetationCatalogEntry> _entries = new();

        public IReadOnlyList<VegetationCatalogEntry> Entries => _entries;

        private Dictionary<VegetationCategory, List<VegetationCatalogEntry>> _byCategory;

        public IReadOnlyList<VegetationCatalogEntry> GetByCategory(VegetationCategory category)
        {
            EnsureIndexBuilt();
            return _byCategory.TryGetValue(category, out var list)
                ? list
                : System.Array.Empty<VegetationCatalogEntry>();
        }

        private void EnsureIndexBuilt()
        {
            if (_byCategory != null)
                return;

            _byCategory = new Dictionary<VegetationCategory, List<VegetationCatalogEntry>>();

            foreach (var entry in _entries)
            {
                if (entry?.Prefab == null)
                    continue;

                if (!_byCategory.TryGetValue(entry.Category, out var list))
                {
                    list = new List<VegetationCatalogEntry>();
                    _byCategory[entry.Category] = list;
                }

                list.Add(entry);
            }
        }

        private void OnValidate() => _byCategory = null;
    }
}