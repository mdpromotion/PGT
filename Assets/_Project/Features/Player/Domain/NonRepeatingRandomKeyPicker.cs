using UnityEngine;
using _Project.Features.Sound.Domain;

namespace _Project.Features.Player.Domain
{
    public sealed class NonRepeatingRandomKeyPicker
    {
        private SoundKey _lastPicked;
        private bool _hasPicked;

        public SoundKey Pick(SoundKey[] pool)
        {
            int length = pool.Length;

            if (length == 1)
            {
                _lastPicked = pool[0];
                _hasPicked = true;
                return _lastPicked;
            }

            SoundKey candidate;

            do
            {
                candidate = pool[Random.Range(0, length)];
            }
            while (_hasPicked && Equals(candidate, _lastPicked));

            _lastPicked = candidate;
            _hasPicked = true;

            return candidate;
        }
    }
}