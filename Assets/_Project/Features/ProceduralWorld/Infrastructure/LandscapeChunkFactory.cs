using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;
using _Project.Features.ProceduralWorld.Infrastructure.Interfaces;
using _Project.Features.ProceduralWorld.Presentation;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure
{
    public class LandscapeChunkFactory : ILandscapeFactory, System.IDisposable
    {
        private readonly Terrain _prefab;
        private readonly ChunkGrid _grid;
        private readonly float _treeDistance;

        private readonly Dictionary<Terrain, ChunkHandle> _handles = new();

        private readonly Queue<Terrain> _pool;

        private const string WaterChildName = "Water";
        private const int WaterMeshSubdivisions = 64;

        public LandscapeChunkFactory(
            Terrain prefab,
            ChunkGrid grid,
            int viewDistance,
            int expectedPoolCapacity = 32)
        {
            _prefab = prefab;
            _grid = grid;
            _pool = new Queue<Terrain>(expectedPoolCapacity);

            float chunkSize = Mathf.Max(grid.ChunkSizeX, grid.ChunkSizeZ);
            _treeDistance = viewDistance * chunkSize;
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
                terrain.treeDistance = _treeDistance;
                terrain.treeBillboardDistance = _treeDistance;
                terrain.treeCrossFadeLength = 0f;

                TerrainChunkCoordinate marker = terrain.GetComponent<TerrainChunkCoordinate>();
                if (!marker)
                    marker = terrain.gameObject.AddComponent<TerrainChunkCoordinate>();

                MeshRenderer waterRenderer = CreateWaterRenderer(terrain);

                handle = new ChunkHandle(
                    collider,
                    marker,
                    waterRenderer,
                    new WaterState());

                _handles.Add(terrain, handle);
            }

            Vector2 worldOffset = _grid.ToWorldOffset(coordinate);

            terrain.transform.position = new Vector3(worldOffset.x, 0, worldOffset.y);

            handle.Marker.Initialize(coordinate);

            return terrain;
        }

        private MeshRenderer CreateWaterRenderer(Terrain terrain)
        {
            Transform waterTransform = terrain.transform.Find(WaterChildName);

            if (waterTransform == null)
            {
                return null;
            }
            
            waterTransform.localPosition = Vector3.zero;
            waterTransform.localRotation = Quaternion.identity;
            waterTransform.localScale = Vector3.one;

            MeshFilter meshFilter = waterTransform.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = waterTransform.GetComponent<MeshRenderer>();

            if (meshFilter != null)
            {
                meshFilter.sharedMesh = WaterQuadMeshProvider.Get(
                    _grid.ChunkSizeX,
                    _grid.ChunkSizeZ,
                    WaterMeshSubdivisions);
            }

            return meshRenderer;
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

            if (handle.WaterRenderer)
                handle.WaterRenderer.enabled = true;
        }

        public void Release(Terrain terrain)
        {
            terrain.drawHeightmap = false;
            terrain.drawTreesAndFoliage = false;

            if (_handles.TryGetValue(terrain, out ChunkHandle handle))
            {
                if (handle.Collider)
                    handle.Collider.enabled = false;

                if (handle.WaterRenderer)
                    handle.WaterRenderer.enabled = false;
            }

            _pool.Enqueue(terrain);
        }

        public MeshRenderer GetWaterRenderer(Terrain terrain)
        {
            return _handles.TryGetValue(terrain, out ChunkHandle handle)
                ? handle.WaterRenderer
                : null;
        }

        public WaterState GetWaterState(Terrain terrain)
        {
            return _handles.TryGetValue(terrain, out ChunkHandle handle)
                ? handle.WaterState
                : null;
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
                terrainLayers = source.terrainLayers,
                treePrototypes = source.treePrototypes
            };
        }
        
        public void Dispose()
        {
            foreach (ChunkHandle handle in _handles.Values)
            {
                if (handle.WaterState?.Texture != null)
                {
                    Object.Destroy(handle.WaterState.Texture);
                    handle.WaterState.Texture = null;
                }
            }

            _handles.Clear();
            _pool.Clear();
        }
    }
}