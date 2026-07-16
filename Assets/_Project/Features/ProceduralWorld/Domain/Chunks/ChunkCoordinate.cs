using System;

namespace _Project.Features.ProceduralWorld.Domain.Chunks
{
    public readonly struct ChunkCoordinate : IEquatable<ChunkCoordinate>
    {
        public readonly int X;
        public readonly int Y;


        public ChunkCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }


        public bool Equals(ChunkCoordinate other)
        {
            return X == other.X &&
                   Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is ChunkCoordinate other &&
                   Equals(other);
        }


        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }


        public static ChunkCoordinate operator +
        (
            ChunkCoordinate a,
            ChunkCoordinate b
        )
        {
            return new ChunkCoordinate(
                a.X + b.X,
                a.Y + b.Y);
        }
    }
}