using System;
using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Application.Chunks.Modifiers;
using _Project.Features.ProceduralWorld.Application.Interfaces;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Biomes.Settings;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.World;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Settings;
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
        private readonly HeightModifierPipeline _modifierPipeline;

        private readonly Dictionary<int, NativeArray<float2>>
            _octaveOffsetsCache = new();



        public BurstChunkGenerator(
            ChunkGrid grid,
            WorldSettings worldSettings,
            HeightModifierPipeline modifierPipeline)
        {
            _grid = grid;

            _worldSettings = worldSettings;

            _modifierPipeline =
                modifierPipeline;
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



            TerrainNoiseSettings terrainSettings =
                CreateTerrainSettings(
                    request.Biome.Generation);



            TerrainGenerationJob terrainJob =
                new TerrainGenerationJob(
                    heights,
                    request.Resolution,
                    _grid.ChunkSizeX,
                    _grid.ChunkSizeZ,
                    new int2(
                        request.Coordinate.X,
                        request.Coordinate.Y),
                    terrainSettings,
                    octaveOffsets);



            JobHandle handle =
                terrainJob.Schedule(
                    count,
                    64);


            ChunkGenerationContext context =
                new ChunkGenerationContext(
                    request.Coordinate,
                    new Unity.Mathematics.float2(
                        request.Coordinate.X * _grid.ChunkSizeX,
                        request.Coordinate.Y * _grid.ChunkSizeZ),
                    _grid.ChunkSizeX,
                    _grid.ChunkSizeZ,
                    request.Resolution);



            handle =
                _modifierPipeline.Schedule(
                    context,
                    heights,
                    handle);



            return new GenerationTask(
                handle,
                new ChunkGenerationResult(
                    request.Coordinate,
                    heights,
                    request.Resolution));
        }





        private TerrainNoiseSettings CreateTerrainSettings(
            BiomeGenerationSettings settings)
        {
            return new TerrainNoiseSettings
            {
                Scale =
                    settings.Scale,

                Octaves =
                    settings.Octaves,

                Persistence =
                    settings.Persistence,

                Lacunarity =
                    settings.Lacunarity,

                RedistributionPower =
                    settings.RedistributionPower,

                Offset =
                    float2.zero
            };
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
            foreach (var offsets in _octaveOffsetsCache.Values)
            {
                if (offsets.IsCreated)
                    offsets.Dispose();
            }


            _octaveOffsetsCache.Clear();
        }
    }
}