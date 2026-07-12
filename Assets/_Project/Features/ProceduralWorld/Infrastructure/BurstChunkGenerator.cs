using _Project.Features.ProceduralWorld.Application;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure
{
    public class BurstChunkGenerator : IChunkGenerator
    {
        
        private readonly ChunkGrid _grid;

        public BurstChunkGenerator(
            ChunkGrid grid)
        {
            _grid = grid;
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


            TerrainGenerationJob job =
                new TerrainGenerationJob(
                    heights,
                    request.Resolution,
                    _grid.ChunkSizeX,
                    _grid.ChunkSizeZ,
                    new int2(
                        request.Coordinate.X,
                        request.Coordinate.Y),
                    CreateSettings(request.Settings));


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
        private NoiseJobSettings CreateSettings(
            NoiseSettings settings)
        {
            Unity.Mathematics.Random random =
                new Unity.Mathematics.Random(
                    (uint)math.max(settings.Seed,1));


            NoiseJobSettings result =
                new NoiseJobSettings();


            result.Scale =
                settings.Scale;

            result.Octaves =
                settings.Octaves;

            result.Persistence =
                settings.Persistence;

            result.Lacunarity =
                settings.Lacunarity;

            result.RedistributionPower =
                settings.RedistributionPower;

            result.SeaLevel =
                settings.SeaLevel;

            result.HeightMultiplier =
                settings.HeightMultiplier;


            result.Offset =
                settings.Offset;



            for(int i = 0; i < settings.Octaves; i++)
            {
                result.OctaveOffsets.Add(
                    new float2(
                        random.NextFloat(-100000,100000),
                        random.NextFloat(-100000,100000)));
            }


            return result;
        }
    }
}