using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Infrastructure;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Application
{
    public class ChunkManager
    {
        private readonly TerrainChunkFactory _factory;
        private readonly GenerateTerrainUseCase _generator;
        private readonly Transform _parent;

        private readonly Dictionary<ChunkCoordinate, Terrain> _chunks = new();

        public ChunkManager(
            TerrainChunkFactory factory,
            GenerateTerrainUseCase generator,
            Transform parent)
        {
            _factory = factory;
            _generator = generator;
            _parent = parent;
        }

        public bool IsLoaded(ChunkCoordinate coordinate)
        {
            return _chunks.TryGetValue(coordinate, out var terrain)
                   && terrain.gameObject.activeSelf;
        }

        public void Load(ChunkCoordinate coordinate, NoiseSettings settings)
        {
            if (_chunks.TryGetValue(coordinate, out Terrain existing))
            {
                if (!existing.gameObject.activeSelf)
                    existing.gameObject.SetActive(true);
                return;
            }

            Terrain terrain = _factory.Create(coordinate, _parent);

            _generator.Execute(terrain.terrainData, settings, coordinate);
            terrain.Flush();

            _chunks.Add(coordinate, terrain);
        }

        public void Unload(ChunkCoordinate coordinate)
        {
            if (_chunks.TryGetValue(coordinate, out Terrain terrain))
                terrain.gameObject.SetActive(false);
        }
    }
}