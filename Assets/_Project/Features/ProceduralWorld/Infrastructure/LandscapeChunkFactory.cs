using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Infrastructure.Interfaces;
using _Project.Features.ProceduralWorld.Presentation;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure
{
    public class LandscapeChunkFactory : ILandscapeFactory, System.IDisposable
    {
        private readonly Terrain _prefab;
        private readonly ChunkGrid _grid;

        private readonly Dictionary<Terrain, ChunkHandle> _handles = new();

        private readonly Queue<Terrain> _pool;

        public LandscapeChunkFactory(
            Terrain prefab,
            ChunkGrid grid,
            int expectedPoolCapacity = 32)
        {
            _prefab = prefab;
            _grid = grid;
            _pool = new Queue<Terrain>(expectedPoolCapacity);
        }

        public void Connect(Terrain terrain, Terrain left, Terrain top, Terrain right, Terrain bottom)
        {
            if (!terrain)
                return;

            terrain.SetNeighbors(left, top, right, bottom);
        }

        public Terrain Create(
            ChunkCoordinate coordinate,
            Transform parent)
        {
            Terrain terrain;
            ChunkHandle handle;

            if (_pool.Count > 0)
            {
                terrain = _pool.Dequeue();
                handle = _handles[terrain];

                Show(terrain, handle);
            }
            else
            {
                terrain = Object.Instantiate(_prefab, parent);

                terrain.drawHeightmap = true;
                terrain.drawTreesAndFoliage = true;
                terrain.drawInstanced = true;

                TerrainData data = CreateTerrainData();
                terrain.terrainData = data;

                TerrainCollider collider = terrain.GetComponent<TerrainCollider>();
                if (collider)
                    collider.terrainData = data;

                terrain.drawInstanced = true;
                terrain.heightmapPixelError = 20;

                TerrainChunkCoordinate marker = terrain.GetComponent<TerrainChunkCoordinate>();
                if (!marker)
                    marker = terrain.gameObject.AddComponent<TerrainChunkCoordinate>();


                handle = new ChunkHandle(
                    collider,
                    marker);

                _handles.Add(terrain, handle);
            }

            Vector2 worldOffset = _grid.ToWorldOffset(coordinate);

            terrain.transform.position = new Vector3(worldOffset.x, 0, worldOffset.y);

            handle.Marker.Initialize(coordinate);

            return terrain;
        }

        public void Show(Terrain terrain)
        {
            if (!_handles.TryGetValue(terrain, out ChunkHandle handle))
                return;

            Show(terrain, handle);
        }

        private void Show(Terrain terrain, ChunkHandle handle)
        {
            terrain.drawHeightmap = true;
            terrain.drawTreesAndFoliage = true;

            if (handle.Collider)
                handle.Collider.enabled = true;
        }

        public void Release(Terrain terrain)
        {
            terrain.drawHeightmap = false;
            terrain.drawTreesAndFoliage = false;

            if (_handles.TryGetValue(terrain, out ChunkHandle handle))
            {
                if (handle.Collider)
                    handle.Collider.enabled = false;
            }

            _pool.Enqueue(terrain);
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
        
        public void Dispose()
        {
            _handles.Clear();
            _pool.Clear();
        }
    }
}