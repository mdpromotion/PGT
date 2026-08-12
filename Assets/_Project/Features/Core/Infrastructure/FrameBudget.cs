using System.Diagnostics;
using UnityEngine;
using VContainer.Unity;

namespace _Project.Features.Core.Infrastructure
{
    public sealed class FrameBudget : IFrameBudget, ITickable
    {
        private readonly FrameBudgetConfig _config;

        private float _smoothedFrameTime;
        private float _budgetMilliseconds;
        private float _spentMilliseconds;

        public FrameBudget(
            FrameBudgetConfig config)
        {
            _config = config;

            _smoothedFrameTime =
                1f / _config.TargetFrameRate;
        }


        public void Tick()
        {
            float frameTime =
                Time.unscaledDeltaTime;

            _smoothedFrameTime =
                Mathf.Lerp(
                    _smoothedFrameTime,
                    frameTime,
                    _config.Smoothing);

            _spentMilliseconds = 0f;

            _budgetMilliseconds =
                CalculateBudget();
        }


        public bool TryBeginOperation(
            out IFrameBudgetOperation operation)
        {
            if (_spentMilliseconds >= _budgetMilliseconds)
            {
                operation = null;

                return false;
            }

            operation =
                new Operation(this);

            return true;
        }


        private float CalculateBudget()
        {
            float targetFrameTime =
                1f / _config.TargetFrameRate;

            float budget =
                targetFrameTime *
                _config.BudgetRatio *
                1000f;
            
            float overload =
                Mathf.Max(
                    0f,
                    _smoothedFrameTime -
                    targetFrameTime);

            if (overload > 0f)
            {
                float factor =
                    targetFrameTime /
                    _smoothedFrameTime;

                budget *= factor;
            }

            return Mathf.Clamp(
                budget,
                _config.MinBudgetMilliseconds,
                _config.MaxBudgetMilliseconds);
        }


        private void Record(
            double milliseconds)
        {
            _spentMilliseconds +=
                (float)milliseconds;
        }


        private sealed class Operation :
            IFrameBudgetOperation
        {
            private readonly FrameBudget _owner;
            private readonly long _startTimestamp;

            private bool _disposed;


            public Operation(
                FrameBudget owner)
            {
                _owner = owner;

                _startTimestamp =
                    Stopwatch.GetTimestamp();
            }


            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;

                long elapsed =
                    Stopwatch.GetTimestamp() -
                    _startTimestamp;

                double milliseconds =
                    elapsed * 1000.0 /
                    Stopwatch.Frequency;

                _owner.Record(milliseconds);
            }
        }
    }
}