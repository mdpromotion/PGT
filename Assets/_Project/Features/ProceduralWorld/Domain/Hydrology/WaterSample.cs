namespace _Project.Features.ProceduralWorld.Domain.Hydrology
{
    public readonly struct WaterSample
    {
        public readonly float Mask;
        public readonly float WorldSurfaceHeight;

        public WaterSample(float mask, float worldSurfaceHeight)
        {
            Mask = mask;
            WorldSurfaceHeight = worldSurfaceHeight;
        }
    }
}