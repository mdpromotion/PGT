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

            List<ChunkCoordinate> ordered = new();

            for (int x = -_viewDistance; x <= _viewDistance; x++)
            {
                for (int y = -_viewDistance; y <= _viewDistance; y++)
                {
                    ChunkCoordinate coordinate =
                        new(center.X + x, center.Y + y);

                    _requiredChunks.Add(coordinate);
                    ordered.Add(coordinate);
                }
            }

            ordered.Sort((a, b) =>
            {
                int da =
                    (a.X - center.X) * (a.X - center.X) +
                    (a.Y - center.Y) * (a.Y - center.Y);

                int db =
                    (b.X - center.X) * (b.X - center.X) +
                    (b.Y - center.Y) * (b.Y - center.Y);

                return da.CompareTo(db);
            });

            foreach (var coordinate in ordered)
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