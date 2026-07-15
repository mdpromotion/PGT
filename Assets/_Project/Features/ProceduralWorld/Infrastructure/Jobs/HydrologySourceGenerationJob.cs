using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Settings;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs
{
    [BurstCompile]
    public struct HydrologySourceGenerationJob : IJob
    {
        private NativeList<RiverSource> _sources;

        private readonly int2 _region;

        private readonly float _regionSize;

        private readonly int _seed;

        private readonly int _sourceAttempts;

        private readonly float _minHeight;


        private readonly TerrainNoiseSettings _terrainSettings;

        [ReadOnly]
        private NativeArray<float2> _octaveOffsets;



        public HydrologySourceGenerationJob(
            NativeList<RiverSource> sources,
            int2 region,
            float regionSize,
            int seed,
            int sourceAttempts,
            float minHeight,
            TerrainNoiseSettings terrainSettings,
            NativeArray<float2> octaveOffsets)
        {
            _sources = sources;

            _region = region;

            _regionSize = regionSize;

            _seed = seed;

            _sourceAttempts = sourceAttempts;

            _minHeight = minHeight;

            _terrainSettings = terrainSettings;

            _octaveOffsets = octaveOffsets;
        }



        public void Execute()
        {
            Random random =
                CreateRandom();


            float2 regionOrigin =
                new float2(
                    _region.x * _regionSize,
                    _region.y * _regionSize);



            for (int i = 0; i < _sourceAttempts; i++)
            {
                float2 position =
                    regionOrigin +
                    random.NextFloat2(
                        0f,
                        _regionSize);


                float height =
                    HeightSampler.Sample(
                        position,
                        _terrainSettings,
                        _octaveOffsets);



                if (height < _minHeight)
                    continue;



                _sources.Add(
                    new RiverSource
                    {
                        Position = position,
                        Height = height
                    });
            }
        }



        private Random CreateRandom()
        {
            uint hash =
                math.hash(
                    new int3(
                        _region.x,
                        _region.y,
                        _seed));


            if (hash == 0)
                hash = 1;


            return new Random(hash);
        }
    }
}