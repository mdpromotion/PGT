using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.Vegetation;
using _Project.Features.ProceduralWorld.Infrastructure.Vegetation;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Application.Vegetation
{
    public sealed class VegetationApplier
    {
        private readonly VegetationCatalog _catalog;
        private readonly Dictionary<GameObject, int> _prototypeIndexByPrefab = new();

        private TreePrototype[] _prototypes;
        private bool _prototypesInitialized;

        public VegetationApplier(VegetationCatalog catalog)
        {
            _catalog = catalog;
        }

        public void Apply(ChunkGenerationState state, Terrain terrain)
        {
            EnsurePrototypesAssigned(terrain);

            IReadOnlyList<VegetationCatalogEntry> treeEntries =
                _catalog.GetByCategory(VegetationCategory.Tree);

            if (treeEntries.Count == 0)
                return;

            var instances = state.Vegetation.Instances;
            var treeInstances = new List<TreeInstance>(instances.Length);

            float3 terrainSize = terrain.terrainData.size;
            Vector3 terrainPosition = terrain.transform.position;

            for (int i = 0; i < instances.Length; i++)
            {
                VegetationInstanceData candidate = instances[i];

                VegetationCatalogEntry entry = PickEntry(
                    treeEntries,
                    candidate.NormalizedHeight01,
                    candidate.SlopeDegrees,
                    candidate.RandomSeed);

                if (entry == null)
                    continue;

                if (!_prototypeIndexByPrefab.TryGetValue(entry.Prefab, out int prototypeIndex))
                    continue;

                var rng = new Unity.Mathematics.Random(candidate.RandomSeed == 0 ? 1u : candidate.RandomSeed);

                float scale = rng.NextFloat(entry.UniformScaleRange.x, entry.UniformScaleRange.y);
                float rotation = entry.RandomizeYRotation ? rng.NextFloat(0f, 2f * Mathf.PI) : 0f;

                Vector3 worldPos = new Vector3(
                    candidate.WorldPosition.x,
                    candidate.WorldPosition.y,
                    candidate.WorldPosition.z);

                Vector3 local = worldPos - terrainPosition;
                Vector3 normalized = new Vector3(
                    local.x / terrainSize.x,
                    0f,
                    local.z / terrainSize.z);

                treeInstances.Add(new TreeInstance
                {
                    position = normalized,
                    prototypeIndex = prototypeIndex,
                    widthScale = scale,
                    heightScale = scale,
                    rotation = rotation,
                    color = Color.white,
                    lightmapColor = Color.white,
                });
            }

            terrain.preserveTreePrototypeLayers = true;

            terrain.terrainData.SetTreeInstances(treeInstances.ToArray(), true);

            TerrainCollider terrainCollider = terrain.GetComponent<TerrainCollider>();

            if (terrainCollider)
            {
                terrainCollider.enabled = false;
                terrainCollider.enabled = true;
            }
        }

        private VegetationCatalogEntry PickEntry(
            IReadOnlyList<VegetationCatalogEntry> entries,
            float height01,
            float slopeDegrees,
            uint seed)
        {
            float totalWeight = 0f;
            for (int i = 0; i < entries.Count; i++)
            {
                var e = entries[i];
                if (e.Matches(height01, slopeDegrees))
                    totalWeight += e.Weight;
            }

            if (totalWeight <= 0f)
                return null;

            var rng = new Unity.Mathematics.Random(seed == 0 ? 1u : seed);
            float roll = rng.NextFloat(0f, totalWeight);

            float accumulated = 0f;
            for (int i = 0; i < entries.Count; i++)
            {
                var e = entries[i];
                if (!e.Matches(height01, slopeDegrees))
                    continue;

                accumulated += e.Weight;
                if (roll <= accumulated)
                    return e;
            }

            return null;
        }
        
        private void EnsurePrototypesAssigned(Terrain terrain)
        {
            if (_prototypesInitialized)
            {
                terrain.terrainData.treePrototypes = _prototypes;
                return;
            }

            IReadOnlyList<VegetationCatalogEntry> treeEntries =
                _catalog.GetByCategory(VegetationCategory.Tree);

            _prototypes = new TreePrototype[treeEntries.Count];
            _prototypeIndexByPrefab.Clear();

            for (int i = 0; i < treeEntries.Count; i++)
            {
                _prototypes[i] = new TreePrototype { prefab = treeEntries[i].Prefab };
                _prototypeIndexByPrefab[treeEntries[i].Prefab] = i;
            }

            terrain.terrainData.treePrototypes = _prototypes;
            _prototypesInitialized = true;
        }
    }
}