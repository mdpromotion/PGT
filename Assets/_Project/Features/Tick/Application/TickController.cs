using System;
using _Project.Features.Shared.Application;
using _Project.Features.Tick.Domain;
using UnityEngine;
using VContainer.Unity;

namespace _Project.Features.Tick.Application
{
    public class TickController : IFixedTickable, ITick
    {
        private readonly float TickInterval;
        private float _elapsed;
        
        public event Action Tick;

        public TickController(TickData tickData)
        {
            TickInterval = tickData.TickInterval;
        }
        
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
