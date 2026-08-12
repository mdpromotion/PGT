using System;
using _Project.Features.Core.Domain;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Features.Core.Infrastructure
{
    public class CoreTimeService : IInitializable, IDisposable
    {
        private IGameState _gameState;

        [Inject]
        public void Construct(IGameState gameState)
        {
            _gameState = gameState;
        }

        public void Initialize()
        {
            _gameState.PausedChanged += OnPause;
        }

        private void OnPause(bool state)
        {
            Time.timeScale = state ? 0 : 1;
        }

        public void Dispose()
        {
            _gameState.PausedChanged -= OnPause;
        }
    }
}
