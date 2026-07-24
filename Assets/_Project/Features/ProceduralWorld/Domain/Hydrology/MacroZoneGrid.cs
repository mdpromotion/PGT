using _Project.Features.ProceduralWorld.Domain.Chunks;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Domain.Hydrology
{
    public sealed class MacroZoneGrid
    {
        public int ZoneSizeInChunks { get; }

        private readonly ChunkGrid _chunkGrid;

        public MacroZoneGrid(
            ChunkGrid chunkGrid,
            int zoneSizeInChunks)
        {
            _chunkGrid = chunkGrid;
            ZoneSizeInChunks = Mathf.Max(1, zoneSizeInChunks);
        }

        public MacroZoneCoordinate ToMacroZoneCoordinate(ChunkCoordinate chunk)
        {
            return new MacroZoneCoordinate(
                FloorDiv(chunk.X, ZoneSizeInChunks),
                FloorDiv(chunk.Y, ZoneSizeInChunks));
        }
        
        public Vector2Int ToLocalChunkOffset(ChunkCoordinate chunk)
        {
            MacroZoneCoordinate zone = ToMacroZoneCoordinate(chunk);
            return new Vector2Int(
                chunk.X - zone.X * ZoneSizeInChunks,
                chunk.Y - zone.Y * ZoneSizeInChunks);
        }

        public ChunkCoordinate ToZoneOriginChunk(MacroZoneCoordinate zone)
        {
            return new ChunkCoordinate(
                zone.X * ZoneSizeInChunks,
                zone.Y * ZoneSizeInChunks);
        }
        
        public Vector2 ToZoneWorldOrigin(MacroZoneCoordinate zone)
        {
            return _chunkGrid.ToWorldOffset(ToZoneOriginChunk(zone));
        }

        public float ZoneWorldSizeX => ZoneSizeInChunks * _chunkGrid.ChunkSizeX;
        public float ZoneWorldSizeZ => ZoneSizeInChunks * _chunkGrid.ChunkSizeZ;
        
        private static int FloorDiv(int a, int b)
        {
            int q = a / b;
            int r = a % b;
            if (r != 0 && (r < 0) != (b < 0))
                q--;
            return q;
        }
    }
}
