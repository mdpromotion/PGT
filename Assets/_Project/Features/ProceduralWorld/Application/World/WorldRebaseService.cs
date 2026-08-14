using System;
using _Project.Features.ProceduralWorld.Application.Chunks;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.World;
using _Project.Features.ProceduralWorld.Infrastructure.Chunks;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Application.World
{
    public sealed class WorldRebaseService
    {
        public event Action<Vector3> WorldRebased;

        private readonly ChunkGrid _grid;
        private readonly ChunkRepository _repository;
        private readonly WorldRebaseSettings _settings;

        public WorldRebaseService(
            ChunkGrid grid,
            ChunkRepository repository,
            WorldRebaseSettings settings)
        {
            _grid = grid;
            _repository = repository;
            _settings = settings;
        }

        public void TryRebase(ChunkCoordinate center)
        {
            ChunkCoordinate origin = _grid.OriginCoordinate;

            int dx = Mathf.Abs(center.X - origin.X);
            int dy = Mathf.Abs(center.Y - origin.Y);
            
            if (dx < _settings.ThresholdChunks &&
                dy < _settings.ThresholdChunks)
            {
                return;
            }

            Vector2 oldOffset = _grid.ToWorldOffset(center);
            Vector3 delta = new Vector3(-oldOffset.x, 0f, -oldOffset.y);

            foreach (ChunkInstance chunk in _repository.All)
            {
                if (chunk.Terrain)
                {
                    chunk.Terrain.transform.position += delta;
                }
            }

            _grid.SetOriginCoordinate(center);

            WorldRebased?.Invoke(delta);
        }
    }
}