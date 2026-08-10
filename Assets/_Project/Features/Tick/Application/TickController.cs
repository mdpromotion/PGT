using System;
using _Project.Features.Shared.Application;
using UnityEngine;
using VContainer.Unity;

namespace _Project.Features.Tick.Application
{
    public class TickController : IFixedTickable, ITick
    {
        private const float TickInterval = 1f / 20f;
        private float _elapsed;
        
        public event Action Tick;
        
        public void FixedTick()
        {
            _elapsed += Time.fixedDeltaTime;

            if (_elapsed < TickInterval)
                return;
            
            _elapsed -= TickInterval;
            
            Tick?.Invoke();
        }
    }
}   
