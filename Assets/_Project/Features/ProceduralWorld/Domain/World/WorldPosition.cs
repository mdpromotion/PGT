namespace _Project.Features.ProceduralWorld.Domain.World
{
    public readonly struct WorldPosition
    {
        public readonly double X;
        public readonly float Y;
        public readonly double Z;

        public WorldPosition(double x, float y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}