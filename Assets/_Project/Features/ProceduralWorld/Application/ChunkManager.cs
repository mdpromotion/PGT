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


        private readonly Dictionary<ChunkCoordinate,Terrain> _chunks = new();


        public ChunkManager(
            TerrainChunkFactory factory,
            GenerateTerrainUseCase generator)
        {
            _factory = factory;
            _generator = generator;
        }



        public void Generate(
            ChunkCoordinate coordinate,
            NoiseSettings settings,
            Transform parent)
        {
            if (_chunks.ContainsKey(coordinate))
                return;


            Terrain terrain =
                _factory.Create(
                    coordinate,
                    parent);


            Vector2 worldOffset =
                new Vector2(
                    coordinate.X *
                    terrain.terrainData.size.x,

                    coordinate.Y *
                    terrain.terrainData.size.z);


            _generator.Execute(
                terrain.terrainData,
                settings,
                worldOffset);


            _chunks.Add(
                coordinate,
                terrain);
        }
    }
}