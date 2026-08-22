using _Project.Features.Player.Domain;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.World;
using UnityEngine;

namespace _Project.Features.UI.Infrastructure
{
    public interface IPlayerPositionService
    {
        WorldPosition GetPlayerPosition();
        ChunkCoordinate GetCurrentChunkCoordinate();
    }
    
    public class PlayerPositionService : IPlayerPositionService
    {
        private readonly ChunkGrid  _chunkGrid;
        private readonly IPlayerReadOnly _player;

        public PlayerPositionService(ChunkGrid chunkGrid, IPlayerReadOnly player)
        {
            _chunkGrid = chunkGrid;
            _player = player;
        }

        public WorldPosition GetPlayerPosition()
        {
            ChunkCoordinate currentChunk = _chunkGrid.OriginCoordinate;;
            Vector3 playerPosition = _player.Position;
            
            var worldPosition = GenerationSpace.AbsoluteChunkOrigin(
                currentChunk, _chunkGrid.ChunkSizeX, _chunkGrid.ChunkSizeZ);;
            
            return new WorldPosition(worldPosition.x + _player.Position.x, playerPosition.y, worldPosition.y + _player.Position.z);
        }

        public ChunkCoordinate GetCurrentChunkCoordinate()
            => _chunkGrid.ToChunkCoordinate(_player.Position);
    }
}
