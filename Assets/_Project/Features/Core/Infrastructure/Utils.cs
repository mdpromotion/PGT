using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;

namespace _Project.Features.Core.Infrastructure
{
    public static class Utils
    {
        public static void SortByDistance(
            List<ChunkCoordinate> list,
            ChunkCoordinate center)
        {
            for (int i = 1; i < list.Count; i++)
            {
                ChunkCoordinate value = list[i];

                int valueDistance =
                    (value.X - center.X) * (value.X - center.X) +
                    (value.Y - center.Y) * (value.Y - center.Y);

                int j = i - 1;

                while (j >= 0)
                {
                    ChunkCoordinate current = list[j];

                    int currentDistance =
                        (current.X - center.X) * (current.X - center.X) +
                        (current.Y - center.Y) * (current.Y - center.Y);

                    if (currentDistance <= valueDistance)
                        break;

                    list[j + 1] = current;
                    j--;
                }

                list[j + 1] = value;
            }
        }
    }
}
