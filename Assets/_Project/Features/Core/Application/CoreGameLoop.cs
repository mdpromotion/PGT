using System;
using _Project.Features.Core.Domain;
using _Project.Features.Cursor.Presentation;
using _Project.Features.Player.Application;
using _Project.Features.UI.Infrastructure;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace _Project.Features.Core.Application
{
    public class CoreGameLoop : IInitializable, IDisposable
    {
        private readonly IPlayerController _player;
        private readonly SceneTransitionService _sceneTransitionService;
        private readonly IGameState _gameState;
        private readonly ICursorService _cursorService;

        public CoreGameLoop(
            IPlayerController player, 
            SceneTransitionService sceneTransitionService, 
            IGameState gameState,
            ICursorService cursorService)
        {
            _player = player;
            _sceneTransitionService = sceneTransitionService;
            _gameState = gameState;
            _cursorService = cursorService;
        }

        public void Initialize()
        {
            InitializeAsync().Forget();
            _cursorService.LockCursor(true);

            _gameState.PausedChanged += OnPausedChanged;
        }

        private void OnPausedChanged(bool state)
        {
            _cursorService.LockCursor(!state);
        }

        private async UniTaskVoid InitializeAsync()
        {
            _player.Freeze(true);

            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));

            while (!_player.Prepare())
            {
                Debug.Log("Player isn't prepared, I'll try again in 1 second");
                await UniTask.Delay(TimeSpan.FromSeconds(1));
            }

            _player.Ready();
            _player.Freeze(false);
            
            Debug.Log("Player ready");

            await _sceneTransitionService.CompleteAsync();
        }

        public void Dispose()
        {
            _gameState.PausedChanged -= OnPausedChanged;
        }
    }
}