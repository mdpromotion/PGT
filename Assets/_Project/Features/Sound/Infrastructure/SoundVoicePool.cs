using System.Collections.Generic;
using UnityEngine;
using _Project.Features.Sound.Domain;

namespace _Project.Features.Sound.Infrastructure
{
    public sealed class SoundVoicePool : MonoBehaviour
    {
        [SerializeField] private int _initialPoolSize = 16;

        private readonly List<Voice> _voices = new();

        private sealed class Voice
        {
            public AudioSource Source;
            public SoundKey Key;
            public bool InUse;
        }

        private void Awake()
        {
            for (int i = 0; i < _initialPoolSize; i++)
                _voices.Add(CreateVoice());
        }

        private Voice CreateVoice()
        {
            var go = new GameObject("Voice");
            go.transform.SetParent(transform);

            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;

            return new Voice { Source = source, InUse = false };
        }

        public int CountActiveTotal()
        {
            int count = 0;
            foreach (Voice v in _voices)
                if (v.InUse && v.Source.isPlaying) count++;
            return count;
        }

        public int CountActiveForKey(SoundKey key)
        {
            int count = 0;
            foreach (Voice v in _voices)
                if (v.InUse && v.Source.isPlaying && v.Key == key) count++;
            return count;
        }

        public void Play(SoundDefinition definition, AudioClip clip, Vector3? position, float volumeScale)
        {
            Voice voice = FindFreeVoice();
            voice.InUse = true;
            voice.Key = definition.Key;

            AudioSource source = voice.Source;
            source.clip = clip;
            source.volume = definition.GetRandomVolume() * volumeScale;
            source.pitch = definition.GetRandomPitch();
            source.spatialBlend = definition.Spatial ? 1f : 0f;

            if (position.HasValue)
                source.transform.position = position.Value;

            source.Play();
        }

        private Voice FindFreeVoice()
        {
            foreach (Voice v in _voices)
                if (!v.InUse || !v.Source.isPlaying)
                    return v;
            
            var extra = CreateVoice();
            _voices.Add(extra);
            return extra;
        }
    }
}