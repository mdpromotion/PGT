using UnityEngine;

namespace _Project.Features.Player.Infrastructure
{
    [CreateAssetMenu(
        menuName = "Project/Player/Movement Config",
        fileName = "PlayerMovementConfig")]
    public sealed class PlayerMovementConfig : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField] private float baseSpeed = 6f;
        [SerializeField] private float sprintSpeedMultiplier = 1.66f;
        [SerializeField] private float crouchSpeedMultiplier = 0.55f;

        [Header("Jump")]
        [SerializeField] private float jumpVelocity = 5f;

        [Header("Water")]
        [SerializeField] private float _waterGravity = -3f;
        [SerializeField] private float _swimVerticalSpeed = 5f;
        [SerializeField] private float _waterAcceleration = 4f;
        [SerializeField] private float _waterDrag = 3f;
        [SerializeField] private float _waterVerticalDrag = 2f;

        public float SwimVerticalSpeed => _swimVerticalSpeed;
        public float WaterGravity => _waterGravity;
        public float WaterAcceleration => _waterAcceleration;
        public float WaterDrag => _waterDrag;
        public float WaterVerticalDrag => _waterVerticalDrag;

        public float BaseSpeed => baseSpeed;

        public float SprintSpeedMultiplier => sprintSpeedMultiplier;

        public float CrouchSpeedMultiplier => crouchSpeedMultiplier;

        public float JumpVelocity => jumpVelocity;
    }
}