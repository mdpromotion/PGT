using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.TerrainGeneration.Presentation;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure
{
    public class TerrainChunkFactory
    {
        private readonly Terrain _prefab;
        private readonly IHeightmapGenerator _heightmapGenerator;
        private readonly NoiseSettings _noiseSettings;


        public TerrainChunkFactory(
            Terrain prefab,
            IHeightmapGenerator heightmapGenerator,
            NoiseSettings noiseSettings)
        {
            _prefab = prefab;
            _heightmapGenerator = heightmapGenerator;
            _noiseSettings = noiseSettings;
        }


        public Terrain Create(
            ChunkCoordinate coordinate,
            Transform parent)
        {
            TerrainData data =
                CreateTerrainData();


            Vector2 worldOffset =
                new Vector2(
                    coordinate.X * data.size.x,
                    coordinate.Y * data.size.z);


            data.SetHeights(
                0,
                0,
                _heightmapGenerator.Generate(
                    data.heightmapResolution,
                    new Vector2(
                        data.size.x,
                        data.size.z),
                    _noiseSettings,
                    worldOffset));


            Terrain terrain =
                Object.Instantiate(
                    _prefab,
                    new Vector3(
                        worldOffset.x,
                        0,
                        worldOffset.y),
                    Quaternion.identity,
                    parent);


            terrain.name =
                $"Chunk [{coordinate.X},{coordinate.Y}]";


            terrain.terrainData =
                data;


            TerrainCollider collider =
                terrain.GetComponent<TerrainCollider>();


            if (collider != null)
            {
                collider.terrainData =
                    data;
            }


            TerrainChunkOffset offset =
                terrain.GetComponent<TerrainChunkOffset>();


            if (offset == null)
            {
                offset =
                    terrain.gameObject
                        .AddComponent<TerrainChunkOffset>();
            }


            offset.Initialize(worldOffset);


            terrain.Flush();


            return terrain;
        }



        private TerrainData CreateTerrainData()
        {
            TerrainData source =
                _prefab.terrainData;


            return new TerrainData
            {
                heightmapResolution =
                    source.heightmapResolution,

                alphamapResolution =
                    source.alphamapResolution,

                baseMapResolution =
                    source.baseMapResolution,

                size =
                    source.size,

                terrainLayers =
                    source.terrainLayers
            };
        }
    }
}