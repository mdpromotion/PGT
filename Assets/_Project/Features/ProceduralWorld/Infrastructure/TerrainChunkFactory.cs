using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Presentation;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure
{
    public class TerrainChunkFactory : ITerrainFactory
    {
        private readonly Terrain _prefab;
        private readonly ChunkGrid _grid;

        private readonly Queue<Terrain> _pool = new();

        public TerrainChunkFactory(
            Terrain prefab,
            ChunkGrid grid)
        {
            _prefab = prefab;
            _grid = grid;
        }

        public Terrain Create(
            ChunkCoordinate coordinate,
            Transform parent)
        {
            Terrain terrain;

            if (_pool.Count > 0)
            {
                terrain = _pool.Dequeue();

                terrain.transform.SetParent(parent, false);
                terrain.gameObject.SetActive(true);
            }
            else
            {
                terrain = Object.Instantiate(
                    _prefab,
                    parent);

                TerrainData data = CreateTerrainData();

                terrain.terrainData = data;

                TerrainCollider collider =
                    terrain.GetComponent<TerrainCollider>();

                if (collider != null)
                    collider.terrainData = data;

                terrain.drawInstanced = true;
                terrain.heightmapPixelError = 20;

                TerrainChunkCoordinate marker =
                    terrain.GetComponent<TerrainChunkCoordinate>();

                if (marker == null)
                    marker = terrain.gameObject.AddComponent<TerrainChunkCoordinate>();
            }

            Vector2 worldOffset =
                _grid.ToWorldOffset(coordinate);

            terrain.transform.position =
                new Vector3(
                    worldOffset.x,
                    0,
                    worldOffset.y);

            terrain.name =
                $"Chunk [{coordinate.X},{coordinate.Y}]";

            terrain
                .GetComponent<TerrainChunkCoordinate>()
                .Initialize(coordinate);

            return terrain;
        }

        public void Release(
            Terrain terrain)
        {
            terrain.gameObject.SetActive(false);

            _pool.Enqueue(terrain);
        }

        private TerrainData CreateTerrainData()
        {
            TerrainData source =
                _prefab.terrainData;

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