using System;
using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Application.Interfaces;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Biomes;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.World;
using _Project.Features.ProceduralWorld.Infrastructure;
using _Project.Features.ProceduralWorld.Infrastructure.World;

namespace _Project.Features.ProceduralWorld.Application.Chunks
{
    public class ChunkManager : IDisposable
    {
        private readonly ChunkGrid _grid;

        private readonly ChunkGenerationScheduler _scheduler;
        private readonly ChunkRepository _repository;
        private readonly ChunkApplier _applier;

        private readonly ITerrainFactory _factory;
        private readonly IChunkNeighborConnector _neighborConnector;

        private readonly WorldGenerator _generator;
        
        private readonly HashSet<ChunkCoordinate> _loading = new();



        public ChunkManager(
            ChunkGrid grid,
            ChunkGenerationScheduler scheduler,
            ChunkRepository repository,
            ChunkApplier applier,
            ITerrainFactory factory,
            IChunkNeighborConnector neighborConnector,
            WorldGenerator generator)
        {
            _grid = grid;

            _scheduler = scheduler;
            _repository = repository;
            _applier = applier;
            _factory = factory;
            _neighborConnector = neighborConnector;

            _generator = generator;
        }



        public void Tick()
        {
            _scheduler.Tick(
                _applier.Apply,
                FinishLoading);
        }
        
        public void Dispose()
        {
            _scheduler.CompleteAll();
        }

        public void QueueLoad(
            ChunkCoordinate coordinate)
        {
            if (_repository.Contains(coordinate))
                return;

            if (_loading.Contains(coordinate))
                return;

            WorldPosition position =
                new WorldPosition(
                    coordinate.X * _grid.ChunkSizeX,
                    coordinate.Y * _grid.ChunkSizeZ);

            BiomeDefinition biome =
                _generator.ResolveBiome(position);

            _loading.Add(coordinate);

            _scheduler.Enqueue(
                new ChunkGenerationRequest(
                    coordinate,
                    513,
                    biome));
        }

        public void CancelLoad(
            ChunkCoordinate coordinate)
        {
            _loading.Remove(coordinate);

            _scheduler.Cancel(coordinate);
        }
        
        public void FinishLoading(
            ChunkCoordinate coordinate)
        {
            _loading.Remove(coordinate);
        }

        public void Unload(
            ChunkCoordinate coordinate)
        {
            if (!_repository.TryGet(
                    coordinate,
                    out var terrain))
            {
                return;
            }


            _neighborConnector.Disconnect(
                _repository,
                coordinate);


            _repository.Remove(
                coordinate);


            _factory.Release(
                terrain);
        }
    }
}