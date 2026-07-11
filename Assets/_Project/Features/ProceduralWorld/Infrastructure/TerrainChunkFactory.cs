using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Presentation;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure
{
    public class TerrainChunkFactory
    {
        private readonly Terrain _prefab;
        private readonly ChunkGrid _grid;

        public TerrainChunkFactory(Terrain prefab, ChunkGrid grid)
        {
            _prefab = prefab;
            _grid = grid;
        }

        public Terrain Create(ChunkCoordinate coordinate, Transform parent)
        {
            TerrainData data = CreateTerrainData();

            Vector2 worldOffset = _grid.ToWorldOffset(coordinate);

            Terrain terrain = Object.Instantiate(
                _prefab,
                new Vector3(worldOffset.x, 0, worldOffset.y),
                Quaternion.identity,
                parent);

            terrain.name = $"Chunk [{coordinate.X},{coordinate.Y}]";
            terrain.terrainData = data;

            TerrainCollider collider = terrain.GetComponent<TerrainCollider>();
            if (collider != null)
                collider.terrainData = data;

            TerrainChunkCoordinate marker = terrain.GetComponent<TerrainChunkCoordinate>();
            if (marker == null)
                marker = terrain.gameObject.AddComponent<TerrainChunkCoordinate>();
            marker.Initialize(coordinate);

            return terrain;
        }

        private TerrainData CreateTerrainData()
        {
            TerrainData source = _prefab.terrainData;

            return new TerrainData
            {
                heightmapResolution = source.heightmapResolution,
                alphamapResolution = source.alphamapResolution,
                baseMapResolution = source.baseMapResolution,
                size = source.size,
                terrainLayers = source.terrainLayers
            };
        }
    }
}