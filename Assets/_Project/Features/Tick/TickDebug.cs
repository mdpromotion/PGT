using System;
using _Project.Features.Shared.Application;
using UnityEngine;
using VContainer.Unity;

namespace _Project.Features.Tick
{
    public class TickDebug : IInitializable, IDisposable
    {
        private readonly ITick _tick;
        
        public TickDebug(ITick tick)
        {
            _tick = tick;
        }

        public void Initialize()
        {
            _tick.Tick += Tick;
            Debug.Log("Tick Debug Started");
        }

        public void Tick()
        {
            Debug.Log("tick");
        }
        
        public void Dispose()
        {
            _tick.Tick -= Tick;
        }
    }
}
