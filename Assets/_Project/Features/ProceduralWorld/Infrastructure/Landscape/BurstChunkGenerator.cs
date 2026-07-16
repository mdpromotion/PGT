using System;
using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Application.Chunks;
using _Project.Features.ProceduralWorld.Application.Chunks.Modifiers;
using _Project.Features.ProceduralWorld.Application.Interfaces;
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

            _modifierPipeline = modifierPipeline;
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
                    _worldSettings.Octaves);



            TerrainNoiseSettings settings =
                CreateTerrainSettings();



            TerrainGenerationJob job =
                new TerrainGenerationJob(
                    heights,
                    request.Resolution,
                    _grid.ChunkSizeX,
                    _grid.ChunkSizeZ,
                    new int2(
                        request.Coordinate.X,
                        request.Coordinate.Y),
                    settings,
                    octaveOffsets);



            JobHandle handle =
                job.Schedule(
                    count,
                    64);
            
            LandscapeData landscape =
                new LandscapeData(
                    request.Coordinate,
                    heights,
                    request.Resolution);



            ChunkGenerationContext context =
                new ChunkGenerationContext(
                    request.Coordinate);
            
            handle =
                _modifierPipeline.Schedule(
                    context,
                    heights,
                    handle);

            ChunkGenerationState state =
                new ChunkGenerationState(
                    context,
                    landscape);



            return new GenerationTask(
                handle,
                state);
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
                if(offsets.IsCreated)
                    offsets.Dispose();
            }


            _octaveOffsetsCache.Clear();
        }
    }
}