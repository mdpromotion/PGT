using System;

namespace _Project.Features.Core.Domain
{
    public interface IGameState
    {
        bool Paused { get; }
        event Action<bool> PausedChanged;
    }

    public interface IGameStateController
    {
        void SetPaused(bool paused);
    }
    
    public class GameState : IGameStateController, IGameState
    {
        public bool Paused { get; private set; }
        public event Action<bool> PausedChanged;

        public void SetPaused(bool paused)
        {
            Paused = paused;
            PausedChanged?.Invoke(paused);
        }
    }
}
