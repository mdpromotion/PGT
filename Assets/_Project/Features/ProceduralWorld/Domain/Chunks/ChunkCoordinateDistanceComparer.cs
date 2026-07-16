using System.Collections.Generic;

namespace _Project.Features.ProceduralWorld.Domain.Chunks
{
    public sealed class ChunkCoordinateDistanceComparer : IComparer<ChunkCoordinate>
    {
        public ChunkCoordinate Center;

        public int Compare(ChunkCoordinate a, ChunkCoordinate b)
        {
            int da = (a.X - Center.X) * (a.X - Center.X) +
                     (a.Y - Center.Y) * (a.Y - Center.Y);

            int db = (b.X - Center.X) * (b.X - Center.X) +
                     (b.Y - Center.Y) * (b.Y - Center.Y);

            return da.CompareTo(db);
        }
    }
}