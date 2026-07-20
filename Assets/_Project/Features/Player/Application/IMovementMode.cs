using UnityEngine;

namespace _Project.Features.Player.Application
{
    public interface IMovementMode
    {
        Vector3 BuildVelocity(
            Vector2 moveInput,
            Vector3 forward,
            Vector3 right,
            Vector3 currentVelocity);

        bool TryJump(ref Vector3 velocity);

        bool TryCrouch(ref Vector3 velocity);
    }
}