using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    internal static class D8Directions
    {
        public const int Count = 8;

        public static int2 GetOffset(int direction)
        {
            switch (direction)
            {
                case 0: return new int2(1, 0);
                case 1: return new int2(1, 1);
                case 2: return new int2(0, 1);
                case 3: return new int2(-1, 1);
                case 4: return new int2(-1, 0);
                case 5: return new int2(-1, -1);
                case 6: return new int2(0, -1);
                default: return new int2(1, -1);
            }
        }

        public static float GetDistance(int direction)
        {
            return (direction & 1) == 0 ? 1f : 1.41421356f;
        }
    }
}