using _Project.Features.Core.Domain;
using UnityEngine;
using VContainer;

namespace _Project.Features.Core.Presentation
{
    public class CoreTimePresenter : MonoBehaviour
    {
        private IGameState _gameState;

        [Inject]
        public void Construct(IGameState gameState)
        {
            _gameState = gameState;
        }

        private void Awake()
        {
            print(_gameState.Paused);
            _gameState.PausedChanged += OnPause;
        }

        private void OnPause(bool state)
        {
            Time.timeScale = state ? 0 : 1;
        }

        private void OnDestroy()
        {
            _gameState.PausedChanged -= OnPause;
        }
    }
}
