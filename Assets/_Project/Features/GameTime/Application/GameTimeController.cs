using System;
using _Project.Features.Shared.Application;
using VContainer.Unity;

namespace _Project.Features.GameTime.Application
{
    public class GameTimeController : IInitializable, IDisposable
    {
        private readonly ITick _tick;
        private readonly Domain.GameTime _gameTime;

        public GameTimeController(ITick tick, Domain.GameTime gameTime)
        {
            _tick = tick;
            _gameTime = gameTime;
        }

        public void Tick()
        {
            _gameTime.Advance();
        }

        public void Initialize()
        {
            _tick.Tick += Tick;
        }

        public void Dispose()
        {
            _tick.Tick -= Tick;
        }
    }
}
