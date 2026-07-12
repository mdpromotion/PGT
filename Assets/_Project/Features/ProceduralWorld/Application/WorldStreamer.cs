using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Features.Player.Domain;
using _Project.Features.ProceduralWorld.Domain;

namespace _Project.Features.ProceduralWorld.Application
{
    public class WorldStreamer
    {
        private readonly ChunkManager _chunkManager;
        private readonly ChunkGrid _chunkGrid;
        private readonly NoiseSettings _noiseSettings;
        private readonly IPlayerReadOnly _player;
        private readonly int _viewDistance;

        private readonly HashSet<ChunkCoordinate> _activeChunks = new();
        private readonly HashSet<ChunkCoordinate> _requiredChunks = new();
        
        private readonly List<ChunkCoordinate> _ordered = new();
        private ChunkCoordinateDistanceComparer _distanceComparer = new();

        private ChunkCoordinate _currentCenter;
        private bool _initialized;

        public WorldStreamer(
            ChunkManager chunkManager,
            ChunkGrid chunkGrid,
            NoiseSettings noiseSettings,
            IPlayerReadOnly player,
            int viewDistance)
        {
            _chunkManager = chunkManager;
            _chunkGrid = chunkGrid;
            _noiseSettings = noiseSettings;
            _player = player;
            _viewDistance = viewDistance;
        }

        public void Update()
        {
            _chunkManager.Tick();

            ChunkCoordinate center =
                _chunkGrid.ToChunkCoordinate(_player.Position);

            if (_initialized && center.Equals(_currentCenter))
                return;

            _initialized = true;
            _currentCenter = center;

            Refresh(center);
        }

        private void Refresh(ChunkCoordinate center)
        {
            _requiredChunks.Clear();

            _ordered.Clear();

            for (int x = -_viewDistance; x <= _viewDistance; x++)
            {
                for (int y = -_viewDistance; y <= _viewDistance; y++)
                {
                    ChunkCoordinate coordinate = new(center.X + x, center.Y + y);
                    _requiredChunks.Add(coordinate);
                    _ordered.Add(coordinate);
                }
            }

            _distanceComparer.Center = center;
            _ordered.Sort(_distanceComparer);

            foreach (var coordinate in _ordered)
            {
                if (!_activeChunks.Contains(coordinate))
                    _chunkManager.QueueLoad(coordinate, _noiseSettings);
            }

            foreach (var coordinate in _ordered)
            {
                if (!_activeChunks.Contains(coordinate))
                {
                    _chunkManager.QueueLoad(
                        coordinate,
                        _noiseSettings);
                }
            }

            foreach (var coordinate in _activeChunks)
            {
                if (!_requiredChunks.Contains(coordinate))
                    _chunkManager.Unload(coordinate);
            }

            _activeChunks.Clear();

            foreach (var coordinate in _requiredChunks)
                _activeChunks.Add(coordinate);
        }
    }
}