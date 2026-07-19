using UnityEngine;

namespace _Project.Features.Sound.Application
{
    public readonly struct SoundPlayOptions
    {
        public readonly Vector3? Position;
        public readonly float VolumeScale;
        public readonly bool IgnoreLimits;

        public static readonly SoundPlayOptions Default = new SoundPlayOptions(null, 1f, false);

        public SoundPlayOptions(Vector3? position, float volumeScale, bool ignoreLimits)
        {
            Position = position;
            VolumeScale = volumeScale;
            IgnoreLimits = ignoreLimits;
        }

        public static SoundPlayOptions At(Vector3 position) => new SoundPlayOptions(position, 1f, false);
    }
}