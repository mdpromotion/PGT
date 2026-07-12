namespace _Project.Features.ProceduralWorld.Infrastructure.World
{
    public readonly struct ClimateSample
    {
        public readonly float Temperature;
        public readonly float Moisture;


        public ClimateSample(
            float temperature,
            float moisture)
        {
            Temperature = temperature;
            Moisture = moisture;
        }
    }
}