using _Project.Features.Player.Domain;
using _Project.Features.Player.Infrastructure;
using _Project.Features.Sound.Domain;
using _Project.Features.Sound.Presentation;
using UnityEngine;
using VContainer;

namespace _Project.Features.Player.Presentation
{
    public sealed class PlayerWaterSoundController : MonoBehaviour
    {
        private PlayerSoundSet _playerSoundSet;
        private IWaterState _waterState;
        private ISoundService _soundService;
        private IPlayerReadOnly _player;

        [Inject]
        public void Construct(
            PlayerSoundSet playerSoundSet,
            IWaterState waterState,
            ISoundService soundService,
            IPlayerReadOnly player)
        {
            _playerSoundSet = playerSoundSet;
            _waterState = waterState;
            _soundService = soundService;
            _player = player;
        }

        private void OnEnable()
        {
            _waterState.OnEnterWater += HandleEnterWater;
            _waterState.OnExitWater += HandleExitWater;
        }

        private void OnDisable()
        {
            _waterState.OnEnterWater -= HandleEnterWater;
            _waterState.OnExitWater -= HandleExitWater;
        }

        private void HandleEnterWater(WaterEnterInfo info)
        {
            SoundKey key = _playerSoundSet.WaterEnter;
            
            _soundService.PlayAt(
                key,
                _player.Position);
        }
        
        private void HandleExitWater()
        {
            SoundKey key = _playerSoundSet.WaterExit;
            
            _soundService.PlayAt(
                key,
                _player.Position);
        }
    }
}