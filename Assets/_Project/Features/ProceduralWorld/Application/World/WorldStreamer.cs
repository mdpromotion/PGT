    using System.Collections.Generic;
    using _Project.Features.Core.Infrastructure;
    using _Project.Features.Player.Domain;
    using _Project.Features.ProceduralWorld.Application.Chunks;
    using _Project.Features.ProceduralWorld.Domain;
    using UnityEngine;

    namespace _Project.Features.ProceduralWorld.Application.World
    {
        public class WorldStreamer
        {
            private readonly ChunkManager _chunkManager;
            private readonly ChunkGrid _chunkGrid;
            private readonly IPlayerReadOnly _player;
            
            private readonly int _viewDistance;

            private readonly HashSet<ChunkCoordinate> _activeChunks = new();
            private readonly HashSet<ChunkCoordinate> _requiredChunks = new();

            private readonly List<ChunkCoordinate> _ordered = new();

            private ChunkCoordinate _currentCenter;

            private bool _initialized;



            public WorldStreamer(
                ChunkManager chunkManager,
                ChunkGrid chunkGrid,
                IPlayerReadOnly player,
                int viewDistance)
            {
                _chunkManager = chunkManager;
                _chunkGrid = chunkGrid;
                _player = player;
                _viewDistance = viewDistance;
            }



            public void Update()
            {
                
                ChunkCoordinate center =
                    _chunkGrid.ToChunkCoordinate(
                        _player.Position);

                if (_initialized &&
                    center.Equals(_currentCenter))
                {
                    return;
                }

                _initialized = true;
                _currentCenter = center;

                Refresh(center);
            }



            private void Refresh(
                ChunkCoordinate center)
            {
                _requiredChunks.Clear();
                _ordered.Clear();

                for (int x = -_viewDistance; x <= _viewDistance; x++)
                {
                    for (int y = -_viewDistance; y <= _viewDistance; y++)
                    {
                        ChunkCoordinate coordinate =
                            new ChunkCoordinate(
                                center.X + x,
                                center.Y + y);

                        _requiredChunks.Add(coordinate);
                        _ordered.Add(coordinate);
                    }
                }

                Utils.SortByDistance(
                    _ordered,
                    center);

                foreach (ChunkCoordinate coordinate in _ordered)
                {
                    if (_activeChunks.Contains(coordinate))
                        continue;

                    _chunkManager.QueueLoad(
                        coordinate);
                }

                foreach (ChunkCoordinate coordinate in _activeChunks)
                {
                    if (_requiredChunks.Contains(coordinate))
                        continue;


                    _chunkManager.CancelLoad(
                        coordinate);


                    _chunkManager.Unload(
                        coordinate);
                }

                _activeChunks.Clear();

                foreach (ChunkCoordinate coordinate in _requiredChunks)
                {
                    _activeChunks.Add(coordinate);
                }
            }
        }
    }