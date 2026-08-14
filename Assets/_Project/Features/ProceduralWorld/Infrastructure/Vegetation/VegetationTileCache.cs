using System;
using System.Collections.Concurrent;
using _Project.Features.ProceduralWorld.Application.Interfaces;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.World;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Vegetation;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Vegetation
{
    public sealed class VegetationTileCache : IDisposable, IGenerationCacheEvictor
    {
        private readonly VegetationGridSettings _settings;
        private readonly int _worldSeed;
        private readonly float2 _patchNoiseOffset;

        private readonly int _chunksPerTile;

        private readonly double _tileWorldSizeX;
        private readonly double _tileWorldSizeZ;

        private readonly int _cellsPerSide;

        private readonly ConcurrentDictionary<
            VegetationTileCoordinate,
            Lazy<VegetationTileData>> _tiles = new();

        public VegetationTileCache(
            VegetationGridSettings settings,
            WorldSettings worldSettings,
            ChunkGrid chunkGrid)
        {
            _settings = settings;
            _worldSeed = worldSettings.Seed;

            var rng = new Unity.Mathematics.Random(
                (uint)_worldSeed == 0 ? 1u : (uint)_worldSeed);

            _patchNoiseOffset = rng.NextFloat2(-100000f, 100000f);

            _chunksPerTile = settings.ChunksPerTile;

            _tileWorldSizeX =
                (double)chunkGrid.ChunkSizeX * _chunksPerTile;

            _tileWorldSizeZ =
                (double)chunkGrid.ChunkSizeZ * _chunksPerTile;

            _cellsPerSide = math.max(
                1,
                (int)math.round(
                    (float)(_tileWorldSizeX / settings.PointSpacing)));
        }

        public VegetationTileCoordinate ToTileCoordinate(
            ChunkCoordinate chunkCoordinate)
        {
            int tx = FloorDivide(chunkCoordinate.X, _chunksPerTile);
            int tz = FloorDivide(chunkCoordinate.Y, _chunksPerTile);

            return new VegetationTileCoordinate(tx, tz);
        }
        
        public double2 GetTileWorldOrigin(
            VegetationTileCoordinate coordinate)
        {
            return new double2(
                coordinate.X * _tileWorldSizeX,
                coordinate.Y * _tileWorldSizeZ);
        }

        public VegetationTileData GetOrBuild(
            VegetationTileCoordinate coordinate)
        {
            Lazy<VegetationTileData> lazy = _tiles.GetOrAdd(
                coordinate,
                key => new Lazy<VegetationTileData>(
                    () => Build(key)));

            return lazy.Value;
        }

        private VegetationTileData Build(
            VegetationTileCoordinate coordinate)
        {
            double2 tileOrigin = GetTileWorldOrigin(coordinate);

            float cellSize =
                (float)(_tileWorldSizeX / _cellsPerSide);

            int cellCount =
                _cellsPerSide * _cellsPerSide;

            var points = new NativeList<float2>(
                cellCount,
                Allocator.Persistent);

            points.SetCapacity(cellCount);

            uint tileSeed =
                (uint)_worldSeed
                ^ (uint)(coordinate.X * 486187739)
                ^ (uint)(coordinate.Y * 1000003);
            
            float2 tileNoiseOrigin = new float2(
                (float)tileOrigin.x,
                (float)tileOrigin.y);

            new GenerateTileCandidatePointsJob
            {
                CellsPerSide = _cellsPerSide,
                CellSize = cellSize,

                TileNoiseOrigin = tileNoiseOrigin,

                JitterStrength = _settings.JitterStrength,
                TileSeed = tileSeed,

                PatchNoiseOffset = _patchNoiseOffset,
                PatchScale = _settings.PatchScale,
                PatchOctaves = _settings.PatchOctaves,
                PatchThreshold = _settings.PatchThreshold,

                Output = points.AsParallelWriter(),
            }
            .Schedule(cellCount, 64)
            .Complete();

            return new VegetationTileData(
                coordinate,
                points);
        }

        public void EvictOutside(
            ChunkCoordinate center,
            int viewDistance)
        {
            int tileRadius =
                (viewDistance / _chunksPerTile) + 1;

            VegetationTileCoordinate centerTile =
                ToTileCoordinate(center);

            foreach (var coord in _tiles.Keys)
            {
                int dx = math.abs(
                    coord.X - centerTile.X);

                int dz = math.abs(
                    coord.Y - centerTile.Y);

                if (dx <= tileRadius && dz <= tileRadius)
                    continue;

                if (_tiles.TryRemove(
                        coord,
                        out var lazy)
                    && lazy.IsValueCreated)
                {
                    lazy.Value.Dispose();
                }
            }
        }

        public void Dispose()
        {
            foreach (var lazy in _tiles.Values)
            {
                if (lazy.IsValueCreated)
                    lazy.Value.Dispose();
            }

            _tiles.Clear();
        }

        private static int FloorDivide(
            int value,
            int divisor)
        {
            long v = value;
            long d = divisor;

            if (v >= 0)
                return (int)(v / d);

            return (int)(-((-v + d - 1) / d));
        }
    }
}