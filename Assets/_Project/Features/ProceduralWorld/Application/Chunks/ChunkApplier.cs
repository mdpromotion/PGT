using _Project.Features.ProceduralWorld.Application.Interfaces;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Infrastructure;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Application.Chunks
{
    public class ChunkApplier
    {
        private readonly ITerrainFactory _factory;
        private readonly ITerrainWriter _writer;
        private readonly IChunkNeighborConnector _neighborConnector;
        private readonly ChunkRepository _repository;
        private readonly Transform _parent;



        public ChunkApplier(
            ITerrainFactory factory,
            ITerrainWriter writer,
            IChunkNeighborConnector neighborConnector,
            ChunkRepository repository,
            Transform parent)
        {
            _factory = factory;
            _writer = writer;
            _neighborConnector = neighborConnector;
            _repository = repository;
            _parent = parent;
        }



        public void Apply(
            ChunkGenerationResult result)
        {
            Terrain terrain =
                _factory.Create(
                    result.Coordinate,
                    _parent);


            _writer.Write(
                terrain,
                result);


            terrain.terrainData.SyncHeightmap();


            _repository.Add(
                result.Coordinate,
                terrain);


            _neighborConnector.Connect(
                _repository,
                result.Coordinate);
        }
    }
}