using _Project.Features.ProceduralWorld.Application.Chunks;
using _Project.Features.ProceduralWorld.Application.Interfaces;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.Landscape;
using _Project.Features.ProceduralWorld.Infrastructure.Chunks;
using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;
using _Project.Features.ProceduralWorld.Infrastructure.Interfaces;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Application.Landscape
{
    public class LandscapeApplier
    {
        private readonly ILandscapeFactory _factory;
        private readonly ITerrainWriter _writer;
        private readonly IChunkNeighborConnector _neighborConnector;
        private readonly ChunkRepository _repository;
        private readonly WaterSurfaceApplier _waterSurfaceApplier;
        private readonly Transform _parent;

        public LandscapeApplier(
            ILandscapeFactory factory,
            ITerrainWriter writer,
            IChunkNeighborConnector neighborConnector,
            ChunkRepository repository,
            WaterSurfaceApplier waterSurfaceApplier,
            Transform parent)
        {
            _factory = factory;
            _writer = writer;
            _neighborConnector = neighborConnector;
            _repository = repository;
            _waterSurfaceApplier = waterSurfaceApplier;
            _parent = parent;
        }

        public void Apply(ChunkGenerationResult result)
        {
            LandscapeData data = result.State.Landscape;

            Terrain terrain = _factory.Create(data.Coordinate, _parent);

            _writer.Write(terrain, data);
            terrain.terrainData.SyncHeightmap();

            _waterSurfaceApplier.Apply(terrain, result.State);

            ChunkInstance chunk = new ChunkInstance(
                data.Coordinate,
                data,
                result.State.Hydrology,
                terrain);

            _repository.Add(chunk);
            _neighborConnector.Connect(_repository, data.Coordinate);
        }
    }
}