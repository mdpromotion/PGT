using System;
using UnityEngine;

namespace _Project.Features.Graphics.Domain
{
    public struct FogState : IEquatable<FogState>
    {
        public readonly Color Color;
        public readonly float StartDistance;
        public readonly float EndDistance;

        public FogState(Color color, float startDistance, float endDistance)
        {
            Color = color;
            StartDistance = startDistance;
            EndDistance = endDistance;
        }

        public bool Equals(FogState other)
        {
            return Color.Equals(other.Color) &&
                   Mathf.Approximately(StartDistance, other.StartDistance) &&
                   Mathf.Approximately(EndDistance, other.EndDistance);
        }

        public static FogState Lerp(FogState from, FogState to, float t)
        {
            return new FogState(
                Color.Lerp(from.Color, to.Color, t),
                Mathf.Lerp(from.StartDistance, to.StartDistance, t),
                Mathf.Lerp(from.EndDistance, to.EndDistance, t));
        }
    }
}