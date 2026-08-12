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
        private readonly Dictionary<Terrain, VegetationHandle> _handles = new();

        private TreePrototype[] _prototypes;
        private bool _prototypesInitialized;
        
        private readonly List<CandidateWeight> _candidateBuffer = new();

        public VegetationApplier(VegetationCatalog catalog)
        {
            _catalog = catalog;
        }

        public void Apply(ChunkGenerationState state, Terrain terrain)
        {
            VegetationHandle handle = GetOrCreateHandle(terrain);

            EnsurePrototypesAssigned(handle, terrain);

            IReadOnlyList<VegetationCatalogEntry> treeEntries =
                _catalog.GetByCategory(VegetationCategory.Tree);

            if (treeEntries.Count == 0)
                return;

            var instances = state.Vegetation.Instances;

            List<TreeInstance> treeInstances = handle.TreeInstanceBuffer;
            treeInstances.Clear();

            if (treeInstances.Capacity < instances.Length)
                treeInstances.Capacity = instances.Length;

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

            TerrainCollider terrainCollider = handle.Collider;

            if (terrainCollider)
            {
                terrainCollider.enabled = false;
                terrainCollider.enabled = true;
            }
        }

        private VegetationHandle GetOrCreateHandle(Terrain terrain)
        {
            if (_handles.TryGetValue(terrain, out VegetationHandle handle))
                return handle;

            handle = new VegetationHandle(terrain.GetComponent<TerrainCollider>());
            _handles.Add(terrain, handle);

            return handle;
        }

        private VegetationCatalogEntry PickEntry(
            IReadOnlyList<VegetationCatalogEntry> entries,
            float height01,
            float slopeDegrees,
            uint seed)
        {
            _candidateBuffer.Clear();

            float totalWeight = 0f;
            for (int i = 0; i < entries.Count; i++)
            {
                var e = entries[i];
                if (!e.Matches(height01, slopeDegrees))
                    continue;

                totalWeight += e.Weight;
                _candidateBuffer.Add(new CandidateWeight(e, totalWeight));
            }

            if (totalWeight <= 0f)
                return null;

            var rng = new Unity.Mathematics.Random(seed == 0 ? 1u : seed);
            float roll = rng.NextFloat(0f, totalWeight);

            for (int i = 0; i < _candidateBuffer.Count; i++)
            {
                if (roll <= _candidateBuffer[i].AccumulatedWeight)
                    return _candidateBuffer[i].Entry;
            }

            return null;
        }

        private void EnsurePrototypesAssigned(VegetationHandle handle, Terrain terrain)
        {
            if (!_prototypesInitialized)
            {
                IReadOnlyList<VegetationCatalogEntry> treeEntries =
                    _catalog.GetByCategory(VegetationCategory.Tree);

                _prototypes = new TreePrototype[treeEntries.Count];
                _prototypeIndexByPrefab.Clear();

                for (int i = 0; i < treeEntries.Count; i++)
                {
                    _prototypes[i] = new TreePrototype { prefab = treeEntries[i].Prefab };
                    _prototypeIndexByPrefab[treeEntries[i].Prefab] = i;
                }

                _prototypesInitialized = true;
            }
            
            if (handle.PrototypesAssigned)
                return;

            terrain.terrainData.treePrototypes = _prototypes;
            handle.PrototypesAssigned = true;
        }

        private readonly struct CandidateWeight
        {
            public readonly VegetationCatalogEntry Entry;
            public readonly float AccumulatedWeight;

            public CandidateWeight(VegetationCatalogEntry entry, float accumulatedWeight)
            {
                Entry = entry;
                AccumulatedWeight = accumulatedWeight;
            }
        }
    }
}