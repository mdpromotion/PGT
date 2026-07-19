using UnityEngine;
using _Project.Features.Sound.Domain;
using _Project.Features.Sound.Application;
using _Project.Features.Sound.Presentation;

namespace _Project.Features.Sound.Infrastructure
{
    public sealed class SoundService : ISoundService
    {
        private readonly SoundDatabase _database;
        private readonly SoundVoicePool _voicePool;
        private readonly SoundPlaybackGuard _guard;

        public SoundService(SoundDatabase database, SoundVoicePool voicePool, SoundPlaybackGuard guard)
        {
            _database = database;
            _voicePool = voicePool;
            _guard = guard;
        }

        public void Play(SoundKey key) => Play(key, SoundPlayOptions.Default);

        public void PlayAt(SoundKey key, Vector3 worldPosition) => Play(key, SoundPlayOptions.At(worldPosition));

        public void Play(SoundKey key, SoundPlayOptions options)
        {
            SoundDefinition definition = _database.Get(key);
            if (definition == null) return;

            float now = Time.time;
            int activeForKey = _voicePool.CountActiveForKey(key);
            int activeTotal = _voicePool.CountActiveTotal();

            if (!_guard.CanPlay(definition, now, activeForKey, activeTotal, options.IgnoreLimits))
                return;

            AudioClip clip = definition.GetRandomClip();
            if (clip == null)
            {
                return;
            }

            _voicePool.Play(definition, clip, options.Position, options.VolumeScale);
            _guard.RegisterPlay(definition, now);
        }
    }
}