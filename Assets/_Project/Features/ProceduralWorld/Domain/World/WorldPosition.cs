namespace _Project.Features.ProceduralWorld.Domain.World
{
    public readonly struct WorldPosition
    {
        public readonly float X;
        public readonly float Z;


        public WorldPosition(
            float x,
            float z)
        {
            X = x;
            Z = z;
        }
    }
}