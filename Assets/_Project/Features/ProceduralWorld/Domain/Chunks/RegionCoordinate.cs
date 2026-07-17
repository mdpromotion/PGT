using System;

namespace _Project.Features.ProceduralWorld.Domain.Chunks
{
    public readonly struct RegionCoordinate : IEquatable<RegionCoordinate>
    {
        public readonly int X;
        public readonly int Y;

        public RegionCoordinate(
            int x,
            int y)
        {
            X = x;
            Y = y;
        }



        public static RegionCoordinate FromChunk(
            ChunkCoordinate chunk,
            int regionSizeInChunks)
        {
            int x = FloorDiv(
                chunk.X,
                regionSizeInChunks);

            int y = FloorDiv(
                chunk.Y,
                regionSizeInChunks);

            return new RegionCoordinate(
                x,
                y);
        }



        private static int FloorDiv(
            int value,
            int size)
        {
            return value >= 0
                ? value / size
                : (value - size + 1) / size;
        }



        public bool Equals(RegionCoordinate other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is RegionCoordinate other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}