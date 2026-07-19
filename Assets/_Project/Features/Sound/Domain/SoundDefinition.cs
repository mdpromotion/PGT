using UnityEngine;

namespace _Project.Features.Sound.Domain
{
    [System.Serializable]
    public sealed class SoundDefinition
    {
        [SerializeField] private SoundKey _key;
        [SerializeField] private SoundCategory _category = SoundCategory.Footstep;
        [SerializeField] private AudioClip[] _clips;

        [Header("Anti-deafening")]
        [SerializeField, Min(0f)] private float _cooldown = 0.05f;
        [SerializeField, Min(1)] private int _maxConcurrentInstances = 3;

        [Header("Variation")]
        [SerializeField, Range(0f, 1f)] private float _volume = 1f;
        [SerializeField, Range(0f, 0.5f)] private float _volumeVariance = 0.05f;
        [SerializeField, Range(0f, 0.5f)] private float _pitchVariance = 0.05f;
        [SerializeField] private bool _spatial = true;

        public SoundKey Key => _key;
        public SoundCategory Category => _category;
        public float Cooldown => _cooldown;
        public int MaxConcurrentInstances => _maxConcurrentInstances;

        public AudioClip GetRandomClip()
        {
            if (_clips == null || _clips.Length == 0)
                return null;

            return _clips[Random.Range(0, _clips.Length)];
        }

        public float GetRandomVolume() =>
            Mathf.Clamp01(_volume + Random.Range(-_volumeVariance, _volumeVariance));

        public float GetRandomPitch() =>
            1f + Random.Range(-_pitchVariance, _pitchVariance);

        public bool Spatial => _spatial;
    }
}