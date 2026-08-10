using System;

namespace _Project.Features.Camera.Infrastructure
{
    public interface IFpsPlayerMotor
    {
        void SetLookYaw(float yawDelta);

        bool IsGrounded { get; }

        event Action OnJumped;
        event Action OnLanded;
    }
}