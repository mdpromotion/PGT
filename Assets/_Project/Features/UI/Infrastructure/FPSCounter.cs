using UnityEngine;
using VContainer.Unity;

namespace _Project.Features.UI.Infrastructure
{
    public interface IFPSCounter
    {
        float CurrentFps { get; }
    }
    
    public class FPSCounter : ITickable, IFPSCounter
    {
        public float CurrentFps { get; private set; }
        
        private const float UpdateInterval = 1f;
        
        private int _frameCount;
        private float _timeAccumulator;
        
        public void Tick()
        {
            _frameCount++;
            _timeAccumulator += Time.unscaledDeltaTime;
            
            if (_timeAccumulator >= UpdateInterval)
            {
                CurrentFps = _frameCount / _timeAccumulator;
                _frameCount = 0;
                _timeAccumulator = 0f;
            }
        }
    }
}
