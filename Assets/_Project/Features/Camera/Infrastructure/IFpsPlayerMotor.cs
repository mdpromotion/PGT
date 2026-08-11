using System;
using UnityEngine;

namespace _Project.Features.Camera.Infrastructure
{
    public interface IFpsPlayerMotor
    {
        void Freeze(bool state);
        bool IsGroundedCheck();
        Vector3 CurrentVelocity { get; }
        void SetVelocity(Vector3 velocity);
        void SetRotation(Quaternion rotation);
    }
}