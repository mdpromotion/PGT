using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.Vegetation;
using _Project.Features.ProceduralWorld.Infrastructure.Vegetation;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Application.Vegetation
{
    internal sealed class TreeVegetationStage
    {
        private readonly VegetationEntryPicker _picker;
        private TreeCatalogIndex _index;

        public TreeVegetationStage(VegetationEntryPicker picker)
        {
            _picker = picker;
        }

        public void Apply(ChunkGenerationState state, Terrain terrain, VegetationHandle handle, VegetationCatalog catalog)
        {
            _index ??= new TreeCatalogIndex(
                VegetationCategoryCombiner.Combine(catalog, VegetationCategory.Tree, VegetationCategory.Bush));

            EnsurePrototypesAssigned(handle.Trees, terrain);

            if (_index.Entries.Count == 0)
                return;

            VegetationData data = state.Vegetation;
            
            if (ReferenceEquals(handle.Trees.LastAppliedData, data))
                return;

            var instances = data.Instances;

            List<TreeInstance> buffer = handle.Trees.InstanceBuffer;
            buffer.Clear();

            if (buffer.Capacity < instances.Length)
                buffer.Capacity = instances.Length;

            float3 terrainSize = terrain.terrainData.size;

            for (int i = 0; i < instances.Length; i++)
            {
                VegetationInstanceData candidate = instances[i];

                VegetationCatalogEntry entry = _picker.Pick(
                    _index.Entries,
                    candidate.NormalizedHeight01,
                    candidate.SlopeDegrees,
                    candidate.RandomSeed);

                if (entry == null)
                    continue;

                if (!_index.TryGetPrototypeIndex(entry.Prefab, out int prototypeIndex))
                    continue;

                var rng = new Unity.Mathematics.Random(candidate.RandomSeed == 0 ? 1u : candidate.RandomSeed);

                float scale = rng.NextFloat(entry.UniformScaleRange.x, entry.UniformScaleRange.y);
                float rotation = entry.RandomizeYRotation ? rng.NextFloat(0f, 2f * Mathf.PI) : 0f;
                
                Vector3 local = new Vector3(
                    candidate.WorldPosition.x,
                    candidate.WorldPosition.y,
                    candidate.WorldPosition.z);

                Vector3 normalized = new Vector3(
                    local.x / terrainSize.x,
                    0f,
                    local.z / terrainSize.z);

                buffer.Add(new TreeInstance
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
            terrain.terrainData.SetTreeInstances(buffer.ToArray(), true);

            if (handle.Collider)
            {
                handle.Collider.enabled = false;
                handle.Collider.enabled = true;
            }

            handle.Trees.LastAppliedData = data;
        }

        private void EnsurePrototypesAssigned(TreeVegetationHandle handle, Terrain terrain)
        {
            if (handle.PrototypesAssigned)
                return;

            terrain.terrainData.treePrototypes = _index.Prototypes;
            handle.PrototypesAssigned = true;
        }
    }
}