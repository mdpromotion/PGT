using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Noise
{
    public class ClimateNoiseGenerator
    {
        private readonly int _seed;
        
        public ClimateNoiseGenerator(
            int seed)
        {
            _seed = seed;
        }


        public float Generate(
            float x,
            float z,
            float scale,
            int offset)
        {
            float2 position =
                new float2(
                    x,
                    z);


            position += new float2(
                _seed * offset,
                _seed * offset * 2);


            float value =
                noise.snoise(
                    position / math.max(scale, 0.001f));


            return
                (value + 1f) * 0.5f;
        }
    }
}