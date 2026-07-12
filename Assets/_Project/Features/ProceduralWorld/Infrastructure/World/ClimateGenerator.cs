using _Project.Features.ProceduralWorld.Domain.World;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.World
{
    public class ClimateGenerator
    {
        private readonly WorldSettings _settings;


        public ClimateGenerator(
            WorldSettings settings)
        {
            _settings = settings;
        }


        public ClimateSample Sample(
            float x,
            float z)
        {
            float temperature =
                GenerateNoise(
                    x,
                    z,
                    _settings.Noise.TemperatureScale,
                    100);


            float moisture =
                GenerateNoise(
                    x,
                    z,
                    _settings.Noise.MoistureScale,
                    200);


            return new ClimateSample(
                temperature,
                moisture);
        }



        private float GenerateNoise(
            float x,
            float z,
            float scale,
            int offset)
        {
            float2 sample =
                new float2(
                    x,
                    z);


            sample +=
                new float2(
                    _settings.Seed * offset,
                    _settings.Seed * offset * 2);



            float value =
                noise.snoise(
                    sample / scale);



            return
                (value + 1f) * 0.5f;
        }
    }
}