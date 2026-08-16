using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.Vegetation;
using _Project.Features.ProceduralWorld.Infrastructure.Vegetation;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Application.Vegetation
{
    /// <summary>
    /// Точка входа: получает/создаёт VegetationHandle для Terrain'а и
    /// прогоняет три независимые стадии (Tree+Bush, Grass+Flower, Rock).
    /// Каждая стадия сама решает, нужно ли ей что-то пересчитывать
    /// (см. Last*Applied* поля в соответствующих sub-handle).
    /// </summary>
    public sealed class VegetationApplier
    {
        private readonly VegetationCatalog _catalog;
        private readonly Dictionary<Terrain, VegetationHandle> _handles = new();

        private readonly TreeVegetationStage _treeStage;
        private readonly DetailVegetationStage _detailStage;
        private readonly RockVegetationStage _rockStage;

        public VegetationApplier(VegetationCatalog catalog, VegetationGridSettings settings)
        {
            _catalog = catalog;

            var picker = new VegetationEntryPicker();

            _treeStage = new TreeVegetationStage(picker);
            _detailStage = new DetailVegetationStage(picker, settings.DetailMapResolution, settings.DetailResolutionPerPatch);
            _rockStage = new RockVegetationStage(picker);
        }

        public void Apply(ChunkGenerationState state, Terrain terrain)
        {
            VegetationHandle handle = GetOrCreateHandle(terrain);

            _treeStage.Apply(state, terrain, handle, _catalog);
            _detailStage.Apply(state, terrain, handle, _catalog);
            _rockStage.Apply(state, terrain, handle, _catalog);
        }

        private VegetationHandle GetOrCreateHandle(Terrain terrain)
        {
            if (_handles.TryGetValue(terrain, out VegetationHandle handle))
                return handle;

            handle = new VegetationHandle(terrain.GetComponent<TerrainCollider>());
            _handles.Add(terrain, handle);

            return handle;
        }
    }
}