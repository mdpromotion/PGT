using System;
using _Project.Features.Shared.Application;
using UnityEngine;
using VContainer.Unity;

namespace _Project.Features.Tick
{
    public class TickDebug : IInitializable, IDisposable
    {
        private readonly ITick _tick;

        private int _tickCount;
        private double _lastLogTime;

        public TickDebug(ITick tick)
        {
            _tick = tick;
        }

        public void Initialize()
        {
            _tick.Tick += Tick;
            _lastLogTime = Time.realtimeSinceStartupAsDouble;
        }

        
        public void Tick()
        {
            _tickCount++;

            double now = Time.realtimeSinceStartupAsDouble;
            double elapsed = now - _lastLogTime;

            if (elapsed >= 1.0)
            {
                _tickCount = 0;
                _lastLogTime = now;
            }
        }

        public void Dispose()
        {
            _tick.Tick -= Tick;
        }
    }
}