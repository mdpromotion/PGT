using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs
{
    [BurstCompile]
    public struct TerrainGenerationJob : IJobParallelFor
    {
        private NativeArray<float> _heights;
        
        private readonly int _resolution;
        
        private readonly float _chunkSizeX;
        private readonly float _chunkSizeZ;
        
        private readonly int2 _chunk;
        
        private readonly NoiseJobSettings _settings;



        public TerrainGenerationJob(
            NativeArray<float> heights,
            int resolution,
            float chunkSizeX,
            float chunkSizeZ,
            int2 chunk,
            NoiseJobSettings settings)
        {
            _heights = heights;

            _resolution = resolution;

            _chunkSizeX = chunkSizeX;
            _chunkSizeZ = chunkSizeZ;

            _chunk = chunk;

            _settings = settings;
        }



        public void Execute(int index)
        {
            int x = index % _resolution;
            int z = index / _resolution;


            float stepX =
                _chunkSizeX /
                (_resolution - 1);


            float stepZ =
                _chunkSizeZ /
                (_resolution - 1);



            float worldX =
                _chunk.x * _chunkSizeX +
                x * stepX +
                _settings.Offset.x;


            float worldZ =
                _chunk.y * _chunkSizeZ +
                z * stepZ +
                _settings.Offset.y;



            float amplitude = 1f;
            float frequency = 1f;

            float height = 0f;


            float maxAmplitude = 0f;



            for (int i = 0; i < _settings.Octaves; i++)
            {
                maxAmplitude += amplitude;


                float2 sample =
                    new float2(
                        worldX,
                        worldZ);


                sample +=
                    _settings.OctaveOffsets[i];


                sample *=
                    frequency /
                    math.max(
                        _settings.Scale,
                        0.0001f);



                float value =
                    noise.snoise(sample);



                height +=
                    value *
                    amplitude;



                amplitude *=
                    _settings.Persistence;


                frequency *=
                    _settings.Lacunarity;
            }



            height =
                height /
                maxAmplitude;



            height =
                (height + 1f) * 0.5f;



            height =
                math.clamp(
                    height,
                    0f,
                    1f);



            height =
                math.pow(
                    height,
                    _settings.RedistributionPower);



            if(height < _settings.SeaLevel)
            {
                height =
                    _settings.SeaLevel;
            }



            _heights[index] =
                height *
                _settings.HeightMultiplier;
        }
    }
}