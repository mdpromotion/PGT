using System;

namespace _Project.Features.Player.Application
{
    public interface IPlayerController
    {
        void SetLookYaw(float yawDelta);
        void Freeze();
        
        bool IsGrounded { get; }

        event Action OnJumped;
        event Action OnLanded;
    }
}