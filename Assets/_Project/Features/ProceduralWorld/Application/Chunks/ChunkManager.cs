using System;
using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Application.Chunks.Generation;
using _Project.Features.ProceduralWorld.Application.Interfaces;
using _Project.Features.ProceduralWorld.Application.Landscape;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Infrastructure.Chunks;
using _Project.Features.ProceduralWorld.Infrastructure.Interfaces;

namespace _Project.Features.ProceduralWorld.Application.Chunks
{
    public class ChunkManager : IDisposable
    {
        private readonly ChunkGenerationScheduler _scheduler;
        private readonly ChunkRepository _repository;
        private readonly LandscapeApplier _applier;

        private readonly ILandscapeFactory _factory;
        private readonly IChunkNeighborConnector _neighborConnector;


        private readonly HashSet<ChunkCoordinate> _loading = new();



        private readonly Action<ChunkGenerationResult> _applyAction;
        private readonly Action<ChunkCoordinate> _completedAction;



        public ChunkManager(
            ChunkGenerationScheduler scheduler,
            ChunkRepository repository,
            LandscapeApplier applier,
            ILandscapeFactory factory,
            IChunkNeighborConnector neighborConnector)
        {
            _scheduler = scheduler;
            _repository = repository;
            _applier = applier;
            _factory = factory;
            _neighborConnector = neighborConnector;


            _applyAction =
                _applier.Apply;


            _completedAction =
                FinishLoading;
        }



        public void Tick()
        {
            _scheduler.Tick(
                _applyAction,
                _completedAction);
        }



        public void Dispose()
        {
            _scheduler.CompleteAll();

            _repository.Dispose();
        }



        public void QueueLoad(
            ChunkCoordinate coordinate)
        {
            if(_repository.Contains(coordinate))
                return;


            if(_loading.Contains(coordinate))
                return;



            _loading.Add(coordinate);



            _scheduler.Enqueue(
                new ChunkGenerationRequest(
                    coordinate,
                    257));
        }



        public void CancelLoad(
            ChunkCoordinate coordinate)
        {
            _loading.Remove(coordinate);


            _scheduler.Cancel(
                coordinate);
        }



        public void FinishLoading(
            ChunkCoordinate coordinate)
        {
            _loading.Remove(
                coordinate);
        }



        public void Unload(ChunkCoordinate coordinate)
        {
            if(!_repository.TryGet(coordinate, out ChunkInstance chunk))
                return;

            _neighborConnector.Disconnect(_repository, coordinate);

            _repository.Remove(coordinate);

            chunk.Landscape.Dispose();

            _factory.Release(chunk.Terrain);
        }
    }
}