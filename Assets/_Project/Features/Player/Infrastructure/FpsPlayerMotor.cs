using _Project.Features.Camera.Infrastructure;
using _Project.Features.Player.Application;
using _Project.Features.Player.Presentation;
using UnityEngine;
using VContainer;

namespace _Project.Features.Player.Infrastructure
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class FpsPlayerMotor : MonoBehaviour, IFpsPlayerMotor
    {
        [SerializeField] private float _moveSpeed = 6f;
        [SerializeField] private float _jumpVelocity = 5f;
        [SerializeField] private float _sprintSpeed = 10f;

        [Header("Ground Check")]
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _groundCheckDistance = 0.25f;
        [SerializeField] private float _groundCheckRadius = 0.3f;

        private IPlayerInputReader _input;
        private FpsMovementUseCase _useCase;

        private Rigidbody _rb;
        private float _yaw;
        private float _pendingYawDelta;

        [Inject]
        public void Construct(
            IPlayerInputReader input,
            FpsMovementUseCase useCase)
        {
            _input = input;
            _useCase = useCase;
        }

        public void SetLookYaw(float yawDelta)
        {
            _pendingYawDelta += yawDelta;
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        private void FixedUpdate()
        {
            _yaw += _pendingYawDelta;
            _pendingYawDelta = 0f;

            Quaternion rotation = Quaternion.Euler(0f, _yaw, 0f);
            _rb.MoveRotation(rotation);

            Vector3 forward = rotation * Vector3.forward;
            Vector3 right = rotation * Vector3.right;

            Vector3 velocity = _rb.linearVelocity;

            float speed = _input.SprintPressed
                ? _sprintSpeed
                : _moveSpeed;

            Vector3 targetVelocity = _useCase.BuildVelocity(
                _input.Move,
                forward,
                right,
                velocity,
                speed);

            if (_input.JumpPressed && IsGrounded() && velocity.y <= 0f)
            {
                targetVelocity.y = _jumpVelocity;
            }

            _rb.linearVelocity = targetVelocity;
        }

        private bool IsGrounded()
        {
            Vector3 checkPosition = transform.position + Vector3.down * 0.9f;

            return Physics.CheckSphere(
                checkPosition,
                _groundCheckRadius,
                _groundMask,
                QueryTriggerInteraction.Ignore);
        }
    }
}