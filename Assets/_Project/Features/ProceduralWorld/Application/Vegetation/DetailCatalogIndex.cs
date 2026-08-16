using System.Collections.Generic;
using UnityEngine;
using _Project.Features.ProceduralWorld.Infrastructure.Vegetation;

namespace _Project.Features.ProceduralWorld.Application.Vegetation
{
    public sealed class DetailCatalogIndex
    {
        public IReadOnlyList<VegetationCatalogEntry> Entries { get; }
        public DetailPrototype[] Prototypes { get; }

        private readonly Dictionary<GameObject, int> _indexByPrefab;

        public DetailCatalogIndex(IReadOnlyList<VegetationCatalogEntry> entries)
        {
            Entries = entries;
            Prototypes = new DetailPrototype[entries.Count];
            _indexByPrefab = new Dictionary<GameObject, int>(entries.Count);

            for (int i = 0; i < entries.Count; i++)
            {
                VegetationCatalogEntry entry = entries[i];

                Prototypes[i] = new DetailPrototype
                {
                    usePrototypeMesh = true,
                    prototype = entry.Prefab,
                    renderMode = DetailRenderMode.VertexLit,
                    useInstancing = true,
                    minWidth = entry.UniformScaleRange.x,
                    maxWidth = entry.UniformScaleRange.y,
                    minHeight = entry.UniformScaleRange.x,
                    maxHeight = entry.UniformScaleRange.y,
                    healthyColor = Color.white,
                    dryColor = Color.white,
                };

                _indexByPrefab[entry.Prefab] = i;
            }
        }

        public bool TryGetLayerIndex(GameObject prefab, out int index)
        {
            return _indexByPrefab.TryGetValue(prefab, out index);
        }
    }
}