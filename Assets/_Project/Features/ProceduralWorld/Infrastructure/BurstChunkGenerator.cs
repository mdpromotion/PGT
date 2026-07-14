using System;
using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Application.Interfaces;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Biomes;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.World;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace _Project.Features.ProceduralWorld.Infrastructure
{
    public class BurstChunkGenerator : IChunkGenerator, IDisposable
    {
        private readonly ChunkGrid _grid;
        private readonly WorldSettings _worldSettings;
        
        private readonly Dictionary<int, NativeArray<float2>> _octaveOffsetsCache =
            new();

        public BurstChunkGenerator(
            ChunkGrid grid,
            WorldSettings worldSettings)
        {
            _grid = grid;
            _worldSettings = worldSettings;
        }

        public GenerationTask Schedule(
            ChunkGenerationRequest request)
        {
            int count =
                request.Resolution *
                request.Resolution;

            NativeArray<float> heights =
                new NativeArray<float>(
                    count,
                    Allocator.Persistent);
            
            NativeArray<float2> octaveOffsets =
                GetOctaveOffsets(
                    request.Biome.Generation.Octaves);

            TerrainGenerationJob job =
                new TerrainGenerationJob(
                    heights,
                    request.Resolution,
                    _grid.ChunkSizeX,
                    _grid.ChunkSizeZ,
                    new int2(
                        request.Coordinate.X,
                        request.Coordinate.Y),
                    CreateSettings(
                        request.Biome.Generation),
                    octaveOffsets);

            JobHandle handle =
                job.Schedule(
                    count,
                    64);

            return new GenerationTask(
                handle,
                new ChunkGenerationResult(
                    request.Coordinate,
                    heights,
                    request.Resolution));
        }

        private NativeArray<float2> CreateOctaveOffsets(int octaves)
        {
            NativeArray<float2> offsets =
                new NativeArray<float2>(
                    octaves,
                    Allocator.Persistent);

            Random random =
                new Random(
                    (uint)math.max(
                        _worldSettings.Seed,
                        1));

            for (int i = 0; i < octaves; i++)
            {
                offsets[i] =
                    new float2(
                        random.NextFloat(-100000f, 100000f),
                        random.NextFloat(-100000f, 100000f));
            }

            return offsets;
        }

        private TerrainNoiseSettings CreateSettings(
            BiomeGenerationSettings settings)
        {
            TerrainNoiseSettings result = new TerrainNoiseSettings();

            result.Scale = settings.Scale;
            result.Octaves = settings.Octaves;
            result.Persistence = settings.Persistence;
            result.Lacunarity = settings.Lacunarity;
            result.RedistributionPower = settings.RedistributionPower;
            result.HeightMultiplier = settings.HeightMultiplier;
            result.Offset = float2.zero;
            
            result.RiverOffset = float2.zero;

            return result;
        }
        
        private NativeArray<float2> GetOctaveOffsets(
            int octaves)
        {
            if (_octaveOffsetsCache.TryGetValue(
                    octaves,
                    out var offsets))
            {
                return offsets;
            }

            offsets = new NativeArray<float2>(
                octaves,
                Allocator.Persistent);

            Random random =
                new Random(
                    (uint)math.max(
                        _worldSettings.Seed,
                        1));

            for (int i = 0; i < octaves; i++)
            {
                offsets[i] =
                    new float2(
                        random.NextFloat(-100000f, 100000f),
                        random.NextFloat(-100000f, 100000f));
            }

            _octaveOffsetsCache.Add(
                octaves,
                offsets);

            return offsets;
        }
        
        public void Dispose()
        {
            foreach (NativeArray<float2> offsets in _octaveOffsetsCache.Values)
            {
                if (offsets.IsCreated)
                    offsets.Dispose();
            }

            _octaveOffsetsCache.Clear();
        }
    }
}