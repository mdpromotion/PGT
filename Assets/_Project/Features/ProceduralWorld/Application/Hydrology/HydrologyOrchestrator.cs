using System;
using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Domain.Hydrology;
using _Project.Features.ProceduralWorld.Domain.World;
using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Settings;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;


namespace _Project.Features.ProceduralWorld.Application.Hydrology
{
    public sealed class HydrologyOrchestrator :
        IDisposable
    {
        private readonly WorldSettings _worldSettings;

        private readonly HydrologySettings _hydrologySettings;


        private readonly NativeArray<float2> _octaveOffsets;



        private readonly Dictionary<
            HydrologyRegionCoordinate,
            JobHandle> _generationJobs = new();



        public HydrologyOrchestrator(
            WorldSettings worldSettings,
            HydrologySettings hydrologySettings)
        {
            _worldSettings = worldSettings;

            _hydrologySettings = hydrologySettings;


            _octaveOffsets =
                CreateOctaveOffsets();
        }



        public void ProcessRegion(
            HydrologyRegion region)
        {
            if (region == null)
                return;


            if (region.State != HydrologyRegionState.Created)
                return;



            region.SetGeneratingSources();



            HydrologySourceGenerationJob job =
                new HydrologySourceGenerationJob(
                    region.Sources,
                    new int2(
                        region.Coordinate.X,
                        region.Coordinate.Y),
                    _hydrologySettings.RegionSize,
                    _worldSettings.Seed,
                    _hydrologySettings.SourceAttempts,
                    0.8f,
                    CreateTerrainSettings(),
                    _octaveOffsets);



            JobHandle handle =
                job.Schedule();



            _generationJobs.Add(
                region.Coordinate,
                handle);
        }



        public void Tick(
            HydrologyRegion region)
        {
            if (region == null)
                return;


            if (!_generationJobs.TryGetValue(
                    region.Coordinate,
                    out JobHandle handle))
            {
                return;
            }


            if (!handle.IsCompleted)
                return;



            handle.Complete();


            region.SetReady();


            _generationJobs.Remove(
                region.Coordinate);
        }



        private NativeArray<float2> CreateOctaveOffsets()
        {
            int octaves =
                _worldSettings.Noise.TemperatureOctaves;


            NativeArray<float2> offsets =
                new NativeArray<float2>(
                    octaves,
                    Allocator.Persistent);



            Unity.Mathematics.Random random =
                new Unity.Mathematics.Random(
                    (uint)math.max(
                        _worldSettings.Seed,
                        1));



            for(int i = 0; i < octaves; i++)
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


            return offsets;
        }



        private TerrainNoiseSettings CreateTerrainSettings()
        {
            return new TerrainNoiseSettings
            {
                Scale =
                    _worldSettings.Noise.TemperatureScale,

                Octaves =
                    _worldSettings.Noise.TemperatureOctaves,

                Persistence =
                    _worldSettings.Noise.TemperaturePersistence,

                Lacunarity = 2f,

                RedistributionPower = 1f,

                Offset = float2.zero
            };
        }



        public void Dispose()
        {
            foreach(JobHandle handle in _generationJobs.Values)
            {
                handle.Complete();
            }


            _generationJobs.Clear();


            if (_octaveOffsets.IsCreated)
                _octaveOffsets.Dispose();
        }
    }
}