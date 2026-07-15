using System;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Domain.Hydrology
{
    public readonly struct HydrologyRegionCoordinate :
        IEquatable<HydrologyRegionCoordinate>
    {
        public readonly int X;

        public readonly int Y;



        public HydrologyRegionCoordinate(
            int x,
            int y)
        {
            X = x;
            Y = y;
        }



        public static HydrologyRegionCoordinate FromWorldPosition(
            float2 worldPosition,
            float regionSize)
        {
            return new HydrologyRegionCoordinate(
                (int)math.floor(
                    worldPosition.x / regionSize),

                (int)math.floor(
                    worldPosition.y / regionSize));
        }



        public bool Equals(
            HydrologyRegionCoordinate other)
        {
            return X == other.X &&
                   Y == other.Y;
        }



        public override bool Equals(
            object obj)
        {
            return obj is HydrologyRegionCoordinate other &&
                   Equals(other);
        }



        public override int GetHashCode()
        {
            return HashCode.Combine(
                X,
                Y);
        }
    }
}