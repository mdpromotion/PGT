using System;
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

        public CoreGameLoop(IPlayerController player, SceneTransitionService sceneTransitionService)
        {
            _player = player;
            _sceneTransitionService = sceneTransitionService;
        }

        public void Initialize()
        {
            InitializeAsync().Forget();
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

            await _sceneTransitionService.CompleteAsync();
        }

        public void Dispose()
        {
        }
    }
}