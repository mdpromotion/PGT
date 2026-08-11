using System;
using _Project.Features.Player.Application;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace _Project.Features.Core.Application
{
    public class CoreGameLoop : IInitializable, IDisposable
    {
        private readonly IPlayerController _player;

        public CoreGameLoop(IPlayerController player)
        {
            _player = player;
        }
        
        public void Initialize()
        {
            InitializeAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            _player.Freeze(true);

            // temporary stub
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            
            while (!_player.Prepare())
            {
                Debug.Log("Player isn't prepared, I'll try again in 1 second");
                await UniTask.Delay(TimeSpan.FromSeconds(1));
            }
            
            _player.Ready();

            _player.Freeze(false);
        }

        public void Dispose()
        {
            
        }
    }
}
