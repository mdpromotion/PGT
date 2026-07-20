using UnityEngine;
using _Project.Features.Sound.Domain;

namespace _Project.Features.Player.Infrastructure
{
    [CreateAssetMenu(menuName = "Project/Player/Footstep Sound Set", fileName = "FootstepSoundSet")]
    public sealed class FootstepSoundSet : ScriptableObject
    {
        [SerializeField] private SoundKey[] _walkFootsteps;
        [SerializeField] private SoundKey[] _runFootsteps;
        [SerializeField] private SoundKey[] _jumpStart;
        [SerializeField] private SoundKey[] _jumpLand;
        
        public SoundKey[] WalkFootsteps => _walkFootsteps;
        public SoundKey[] RunFootsteps => _runFootsteps;
        public SoundKey[] JumpStart => _jumpStart;
        public SoundKey[] JumpLand => _jumpLand;
    }
}