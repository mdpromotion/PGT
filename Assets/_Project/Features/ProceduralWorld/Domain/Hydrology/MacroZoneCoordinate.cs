using System;

namespace _Project.Features.ProceduralWorld.Domain.Hydrology
{
    public readonly struct MacroZoneCoordinate : IEquatable<MacroZoneCoordinate>
    {
        public int X { get; }
        public int Y { get; }

        public MacroZoneCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(MacroZoneCoordinate other) =>
            X == other.X && Y == other.Y;

        public override bool Equals(object obj) =>
            obj is MacroZoneCoordinate other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine(X, Y);

        public override string ToString() =>
            $"MacroZone({X}, {Y})";
    }
}
