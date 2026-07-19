using System.Collections.Generic;
using _Project.Features.Sound.Domain;

namespace _Project.Features.Sound.Application
{
    public sealed class SoundPlaybackGuard
    {
        private readonly int _globalMaxVoices;
        private readonly Dictionary<SoundKey, float> _lastPlayTime = new();

        public SoundPlaybackGuard(int globalMaxVoices = 32)
        {
            _globalMaxVoices = globalMaxVoices;
        }

        public bool CanPlay(
            SoundDefinition definition,
            float currentTime,
            int activeInstancesForKey,
            int activeVoicesTotal,
            bool ignoreLimits)
        {
            if (definition == null)
                return false;

            if (ignoreLimits)
                return true;

            if (activeVoicesTotal >= _globalMaxVoices)
                return false;

            if (activeInstancesForKey >= definition.MaxConcurrentInstances)
                return false;

            if (_lastPlayTime.TryGetValue(definition.Key, out float lastTime) &&
                currentTime - lastTime < definition.Cooldown)
            {
                return false;
            }

            return true;
        }

        public void RegisterPlay(SoundDefinition definition, float currentTime)
        {
            _lastPlayTime[definition.Key] = currentTime;
        }
    }
}