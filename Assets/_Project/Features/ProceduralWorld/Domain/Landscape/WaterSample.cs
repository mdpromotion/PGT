namespace _Project.Features.ProceduralWorld.Domain.Landscape
{
    public readonly struct WaterSample
    {
        public readonly float Mask;
        public readonly float SurfaceHeight;

        public WaterSample(float mask, float surfaceHeight)
        {
            Mask = mask;
            SurfaceHeight = surfaceHeight;
        }

        public bool IsSubmerged(float playerY, float maskThreshold)
        {
            return Mask >= maskThreshold && playerY <= SurfaceHeight;
        }
    }
}