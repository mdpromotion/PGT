using System;
using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Application.Chunks.Generation;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.Landscape;
using _Project.Features.ProceduralWorld.Domain.World;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Settings;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace _Project.Features.ProceduralWorld.Infrastructure.Landscape
{
    public sealed class LandscapeGenerator :
        IGenerationStage,
        IDisposable
    {
        private readonly ChunkGrid _grid;

        private readonly WorldSettings _worldSettings;

        private readonly Dictionary<int, NativeArray<float2>>
            _octaveOffsetsCache = new();



        public LandscapeGenerator(
            ChunkGrid grid,
            WorldSettings worldSettings)
        {
            _grid = grid;
            _worldSettings = worldSettings;
        }



        public JobHandle Schedule(
            ChunkGenerationState state,
            JobHandle dependency)
        {
            ChunkGenerationContext context =
                state.Context;

            int resolution =
                context.Resolution;

            NativeArray<float> heights =
                new NativeArray<float>(
                    resolution * resolution,
                    Allocator.Persistent);

            LandscapeData landscape =
                new LandscapeData(
                    context.Coordinate,
                    heights,
                    resolution);

            state.Landscape =
                landscape;

            TerrainGenerationJob job =
                new TerrainGenerationJob(
                    heights,
                    resolution,
                    _grid.ChunkSizeX,
                    _grid.ChunkSizeZ,
                    new int2(
                        context.Coordinate.X,
                        context.Coordinate.Y),
                    CreateTerrainSettings(),
                    GetOctaveOffsets(
                        _worldSettings.Octaves));

            return job.Schedule(
                heights.Length,
                64,
                dependency);
        }



        private TerrainNoiseSettings CreateTerrainSettings()
        {
            return new TerrainNoiseSettings
            {
                Scale =
                    _worldSettings.Scale,

                Octaves =
                    _worldSettings.Octaves,

                Persistence =
                    _worldSettings.Persistence,

                Lacunarity =
                    _worldSettings.Lacunarity,

                RedistributionPower =
                    _worldSettings.RedistributionPower,

                Offset =
                    float2.zero
            };
        }



        private NativeArray<float2> GetOctaveOffsets(
            int octaves)
        {
            if (_octaveOffsetsCache.TryGetValue(
                    octaves,
                    out NativeArray<float2> offsets))
            {
                return offsets;
            }



            offsets =
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
                        random.NextFloat(
                            -100000f,
                            100000f),
                        random.NextFloat(
                            -100000f,
                            100000f));
            }

            _octaveOffsetsCache.Add(
                octaves,
                offsets);

            return offsets;
        }



        public void Dispose()
        {
            foreach (NativeArray<float2> offsets
                     in _octaveOffsetsCache.Values)
            {
                if (offsets.IsCreated)
                    offsets.Dispose();
            }

            _octaveOffsetsCache.Clear();
        }
    }
}