using _Project.Features.ProceduralWorld.Domain.Landscape;
using _Project.Features.ProceduralWorld.Infrastructure.Vegetation;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public interface ITreeInstanceWriter
{
    void Write(Terrain terrain, LandscapeData data);
}

public sealed class UnityTreeInstanceWriter : ITreeInstanceWriter
{
    private readonly TreePlacementSettings _settings;

    public UnityTreeInstanceWriter(TreePlacementSettings settings)
    {
        _settings = settings;
    }

    public void Write(Terrain terrain, LandscapeData data)
    {
        NativeList<TreeInstanceRaw> trees = data.Trees;

        if (!trees.IsCreated || trees.Length == 0)
        {
            terrain.terrainData.treeInstances = System.Array.Empty<TreeInstance>();
            return;
        }

        Vector3 terrainSize = terrain.terrainData.size;
        TreeInstance[] instances = new TreeInstance[trees.Length];

        for (int i = 0; i < trees.Length; i++)
        {
            TreeInstanceRaw raw = trees[i];

            var rng = new Unity.Mathematics.Random(raw.Seed);

            float rotation = rng.NextFloat(0f, math.PI * 2f);
            float scale = rng.NextFloat(_settings.MinScale, _settings.MaxScale);

            instances[i] = new TreeInstance
            {
                position = new Vector3(
                    raw.LocalPosition.x / terrainSize.x,
                    0f,
                    raw.LocalPosition.y / terrainSize.z),
                widthScale = scale,
                heightScale = scale,
                rotation = rotation,
                color = Color.white,
                lightmapColor = Color.white,
                prototypeIndex = raw.PrototypeIndex
            };
        }

        terrain.terrainData.SetTreeInstances(instances, true);
        terrain.Flush();
    }
}