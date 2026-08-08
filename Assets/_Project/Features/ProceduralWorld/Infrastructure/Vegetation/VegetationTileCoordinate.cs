using System;

namespace _Project.Features.ProceduralWorld.Infrastructure.Vegetation
{
    public readonly struct VegetationTileCoordinate : IEquatable<VegetationTileCoordinate>
    {
        public readonly int X;
        public readonly int Y;

        public VegetationTileCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(VegetationTileCoordinate other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is VegetationTileCoordinate o && Equals(o);
        public override int GetHashCode() => HashCode.Combine(X, Y);
    }
}