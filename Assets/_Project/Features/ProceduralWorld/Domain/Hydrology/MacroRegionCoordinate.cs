using System;

namespace _Project.Features.ProceduralWorld.Domain.Hydrology
{
    public readonly struct MacroRegionCoordinate : IEquatable<MacroRegionCoordinate>
    {
        public readonly int X;
        public readonly int Y;

        public MacroRegionCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(MacroRegionCoordinate other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is MacroRegionCoordinate other && Equals(other);
        public override int GetHashCode() => (X * 397) ^ Y;
        public override string ToString() => $"MacroRegion({X},{Y})";
    }
}