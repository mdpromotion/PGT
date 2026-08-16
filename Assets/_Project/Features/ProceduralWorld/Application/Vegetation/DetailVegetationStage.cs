using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.Vegetation;
using _Project.Features.ProceduralWorld.Infrastructure.Vegetation;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Application.Vegetation
{
    internal sealed class DetailVegetationStage
    {
        private readonly VegetationEntryPicker _picker;
        private readonly int _resolution;
        private readonly int _resolutionPerPatch;

        private DetailCatalogIndex _index;

        public DetailVegetationStage(VegetationEntryPicker picker, int resolution, int resolutionPerPatch)
        {
            _picker = picker;
            _resolution = resolution;
            _resolutionPerPatch = resolutionPerPatch;
        }

        public void Apply(ChunkGenerationState state, Terrain terrain, VegetationHandle handle, VegetationCatalog catalog)
        {
            _index ??= new DetailCatalogIndex(
                VegetationCategoryCombiner.Combine(catalog, VegetationCategory.Grass, VegetationCategory.Flower));

            if (_index.Entries.Count == 0)
                return;

            TerrainData terrainData = terrain.terrainData;

            if (!handle.Details.ResolutionAssigned)
            {
                terrainData.SetDetailResolution(_resolution, _resolutionPerPatch);
                handle.Details.ResolutionAssigned = true;
            }

            if (!handle.Details.PrototypesAssigned)
            {
                terrainData.detailPrototypes = _index.Prototypes;
                handle.Details.PrototypesAssigned = true;
            }

            VegetationData data = state.Vegetation;

            if (ReferenceEquals(handle.Details.LastAppliedData, data))
                return;

            int layerCount = _index.Prototypes.Length;

            var densityMaps = new int[layerCount][,];
            for (int i = 0; i < layerCount; i++)
                densityMaps[i] = new int[_resolution, _resolution];

            var instances = data.Instances;
            Vector3 terrainSize = terrainData.size;

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

                if (!_index.TryGetLayerIndex(entry.Prefab, out int layerIndex))
                    continue;

                float normalizedX = candidate.WorldPosition.x / terrainSize.x;
                float normalizedZ = candidate.WorldPosition.z / terrainSize.z;

                int cellX = Mathf.Clamp(Mathf.FloorToInt(normalizedX * _resolution), 0, _resolution - 1);
                int cellZ = Mathf.Clamp(Mathf.FloorToInt(normalizedZ * _resolution), 0, _resolution - 1);

                var rng = new Unity.Mathematics.Random(candidate.RandomSeed == 0 ? 1u : candidate.RandomSeed);
                int increment = rng.NextInt(1, 4);

                int[,] map = densityMaps[layerIndex];
                map[cellZ, cellX] = Mathf.Min(map[cellZ, cellX] + increment, 16);
            }

            for (int i = 0; i < layerCount; i++)
                terrainData.SetDetailLayer(0, 0, i, densityMaps[i]);

            handle.Details.LastAppliedData = data;
        }
    }
}