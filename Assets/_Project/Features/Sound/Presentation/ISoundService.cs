using UnityEngine;
using _Project.Features.Sound.Domain;
using _Project.Features.Sound.Application;

namespace _Project.Features.Sound.Presentation
{
    public interface ISoundService
    {
        void Play(SoundKey key);
        void Play(SoundKey key, SoundPlayOptions options);
        void PlayAt(SoundKey key, Vector3 worldPosition);
    }
}