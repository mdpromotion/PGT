using UnityEngine;

namespace _Project.Features.Player.Presentation
{
    public interface IFpsPlayerMotor
    {
        Vector3 CurrentVelocity { get; }

        void SetVelocity(Vector3 velocity);
        void SetRotation(Quaternion rotation);
        void Freeze(bool state);

        bool IsGroundedCheck();

        bool TryGetSafeGroundPosition(
            out Vector3 position);

        void TeleportToPosition(
            Vector3 position);
    }
}