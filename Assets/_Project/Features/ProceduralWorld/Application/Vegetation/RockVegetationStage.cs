using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.Vegetation;
using _Project.Features.ProceduralWorld.Infrastructure.Vegetation;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Application.Vegetation
{
    internal sealed class RockVegetationStage
    {
        private readonly VegetationEntryPicker _picker;

        public RockVegetationStage(VegetationEntryPicker picker)
        {
            _picker = picker;
        }

        public void Apply(ChunkGenerationState state, Terrain terrain, VegetationHandle handle, VegetationCatalog catalog)
        {
            IReadOnlyList<VegetationCatalogEntry> rockEntries = catalog.GetByCategory(VegetationCategory.Rock);

            if (handle.Rocks.Root == null)
            {
                var root = new GameObject("Rocks");
                root.transform.SetParent(terrain.transform, false);
                handle.Rocks.Root = root.transform;
                handle.Rocks.Pool = new GameObjectInstancePool(handle.Rocks.Root);
            }

            VegetationData data = state.Vegetation;

            if (ReferenceEquals(handle.Rocks.LastAppliedData, data))
                return;

            GameObjectInstancePool pool = handle.Rocks.Pool;
            pool.BeginFrame();

            if (rockEntries.Count > 0)
            {
                var instances = data.Instances;

                for (int i = 0; i < instances.Length; i++)
                {
                    VegetationInstanceData candidate = instances[i];

                    VegetationCatalogEntry entry = _picker.Pick(
                        rockEntries,
                        candidate.NormalizedHeight01,
                        candidate.SlopeDegrees,
                        candidate.RandomSeed);

                    if (entry == null)
                        continue;

                    var rng = new Unity.Mathematics.Random(candidate.RandomSeed == 0 ? 1u : candidate.RandomSeed);

                    float scale = rng.NextFloat(entry.UniformScaleRange.x, entry.UniformScaleRange.y);
                    float rotationY = entry.RandomizeYRotation ? rng.NextFloat(0f, 360f) : 0f;
                    
                    Vector3 local = new Vector3(
                        candidate.WorldPosition.x,
                        candidate.WorldPosition.y,
                        candidate.WorldPosition.z);

                    GameObject instance = pool.Rent(entry.Prefab);
                    instance.transform.SetLocalPositionAndRotation(local, Quaternion.Euler(0f, rotationY, 0f));
                    instance.transform.localScale = Vector3.one * scale;
                }
            }

            pool.EndFrame();
            handle.Rocks.LastAppliedData = data;
        }
    }
}