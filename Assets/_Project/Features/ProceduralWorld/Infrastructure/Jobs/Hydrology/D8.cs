using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    public static class D8
    {
        public static readonly int2[] Offsets =
        {
            new int2(-1, -1), new int2(0, -1), new int2(1, -1),
            new int2(-1,  0),                  new int2(1,  0),
            new int2(-1,  1), new int2(0,  1), new int2(1,  1)
        };

        public const int None = -1;
    }
}