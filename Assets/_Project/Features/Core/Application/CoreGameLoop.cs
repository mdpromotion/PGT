using System;
using _Project.Features.Player.Application;
using Cysharp.Threading.Tasks;
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
            await UniTask.Delay(TimeSpan.FromSeconds(1));

            _player.Freeze(false);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
