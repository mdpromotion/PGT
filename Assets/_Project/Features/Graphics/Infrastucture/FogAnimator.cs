using _Project.Features.Graphics.Domain;
using UnityEngine;

namespace _Project.Features.Graphics.Infrastucture
{
    public interface IFogAnimator
    {
        void SnapTo(FogState state);
        void SetTarget(FogState state);
        void Tick(float deltaTime);
    }

    public class FogAnimator : IFogAnimator
    {
        private readonly IFogApplier _applier;

        private FogState _targetState;
        
        private Color _currentColor;
        private float _currentStart;
        private float _currentEnd;

        private const float DistanceSpeed = 1000f;
        private const float ColorSpeed = 1.5f;

        public FogAnimator(IFogApplier applier)
        {
            _applier = applier;

            _currentColor = RenderSettings.fogColor;
            _currentStart = RenderSettings.fogStartDistance;
            _currentEnd = RenderSettings.fogEndDistance;
            
            _targetState = new FogState(_currentColor, _currentStart, _currentEnd);
        }

        public void SnapTo(FogState state)
        {
            _targetState = state;
            
            _currentColor = state.Color;
            _currentStart = state.StartDistance;
            _currentEnd = state.EndDistance;
            
            _applier.Apply(_currentColor, _currentStart, _currentEnd);
        }

        public void SetTarget(FogState state)
        {
            _targetState = state;
        }

        public void Tick(float deltaTime)
        {
            _currentColor.r = Mathf.MoveTowards(_currentColor.r, _targetState.Color.r, ColorSpeed * deltaTime);
            _currentColor.g = Mathf.MoveTowards(_currentColor.g, _targetState.Color.g, ColorSpeed * deltaTime);
            _currentColor.b = Mathf.MoveTowards(_currentColor.b, _targetState.Color.b, ColorSpeed * deltaTime);
            _currentColor.a = Mathf.MoveTowards(_currentColor.a, _targetState.Color.a, ColorSpeed * deltaTime);

            _currentStart = Mathf.MoveTowards(_currentStart, _targetState.StartDistance, DistanceSpeed * deltaTime);
            _currentEnd = Mathf.MoveTowards(_currentEnd, _targetState.EndDistance, DistanceSpeed * deltaTime);

            _applier.Apply(_currentColor, _currentStart, _currentEnd);
        }
    }

}