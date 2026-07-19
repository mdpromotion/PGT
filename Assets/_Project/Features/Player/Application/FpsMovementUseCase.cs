using UnityEngine;

namespace _Project.Features.Player.Application
{
    public sealed class FpsMovementUseCase
    {
        public Vector3 BuildVelocity(
            Vector2 moveInput,
            Vector3 forward,
            Vector3 right,
            Vector3 currentVelocity,
            float moveSpeed)
        {
            Vector3 planarForward = Vector3.ProjectOnPlane(forward, Vector3.up).normalized;
            Vector3 planarRight = Vector3.ProjectOnPlane(right, Vector3.up).normalized;

            Vector3 move = planarForward * moveInput.y + planarRight * moveInput.x;

            if (move.sqrMagnitude > 1f)
                move.Normalize();

            Vector3 desiredVelocity = move * moveSpeed;

            return new Vector3(
                desiredVelocity.x,
                currentVelocity.y,
                desiredVelocity.z);
        }
    }
}