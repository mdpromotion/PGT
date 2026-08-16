using System.Collections.Generic;
using UnityEngine;
using _Project.Features.ProceduralWorld.Infrastructure.Vegetation;

namespace _Project.Features.ProceduralWorld.Application.Vegetation
{
    public sealed class TreeCatalogIndex
    {
        public IReadOnlyList<VegetationCatalogEntry> Entries { get; }
        public TreePrototype[] Prototypes { get; }

        private readonly Dictionary<GameObject, int> _indexByPrefab;

        public TreeCatalogIndex(IReadOnlyList<VegetationCatalogEntry> entries)
        {
            Entries = entries;
            Prototypes = new TreePrototype[entries.Count];
            _indexByPrefab = new Dictionary<GameObject, int>(entries.Count);

            for (int i = 0; i < entries.Count; i++)
            {
                Prototypes[i] = new TreePrototype { prefab = entries[i].Prefab };
                _indexByPrefab[entries[i].Prefab] = i;
            }
        }

        public bool TryGetPrototypeIndex(GameObject prefab, out int index)
        {
            return _indexByPrefab.TryGetValue(prefab, out index);
        }
    }
}