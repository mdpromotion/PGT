using System;

namespace _Project.Features.GameTime.Domain
{
    public interface IGameTime
    {
        float CurrentTime { get; }
        float TicksPerDay { get; }

        event Action<float> TimeChanged;

        float HoursToTicks(float hours);
    }
}