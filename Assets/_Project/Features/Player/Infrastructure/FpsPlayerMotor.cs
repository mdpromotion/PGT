using System;
using _Project.Features.Player.Application;
using _Project.Features.Player.Domain;
using _Project.Features.Player.Presentation;
using UnityEngine;
using VContainer;

namespace _Project.Features.Player.Infrastructure
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public sealed class FpsPlayerMotor : MonoBehaviour, IFpsPlayerMotor
    {
        [Header("Movement")]
        [SerializeField] private float _baseSpeed = 6f;
        [SerializeField] private float _sprintSpeedMultiplier = 1.66f;
        [SerializeField] private float _crouchSpeedMultiplier = 0.55f;

        [Header("Jump")]
        [SerializeField] private float _jumpVelocity = 5f;

        [Header("Ground Check")]
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _groundCheckRadius = 0.35f;
        [SerializeField] private float _groundCheckOffset = 0.05f;
        [SerializeField] private float _groundCheckRate = 10f;

        [Header("Landing")]
        [SerializeField] private float _landingFallSpeedThreshold = -3f;

        private IPlayerInputReader _input;
        private IPlayerStanceState _stance;
        private FpsMovementUseCase _useCase;

        private Rigidbody _rb;
        private Collider _collider;

        private float _yaw;
        private float _pendingYawDelta;

        private bool _wasGrounded;
        private bool _groundedCached;

        private float _groundCheckTimer;
        private float _groundCheckInterval;

        private float _lastVerticalVelocity;

        public bool IsGrounded { get; private set; }

        public event Action OnJumped;
        public event Action OnLanded;

        [Inject]
        public void Construct(
            IPlayerInputReader input,
            IPlayerStanceState stance,
            FpsMovementUseCase useCase)
        {
            _input = input;
            _stance = stance;
            _useCase = useCase;
        }

        public void SetLookYaw(float yawDelta)
        {
            _pendingYawDelta += yawDelta;
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();

            _rb.interpolation = RigidbodyInterpolation.Interpolate;

            _groundCheckInterval = 1f / _groundCheckRate;
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

            Vector3 targetVelocity = _useCase.BuildVelocity(
                _input.Move,
                forward,
                right,
                velocity,
                _baseSpeed,
                GetSpeedMultiplier());

            UpdateGroundCheck();

            bool groundedNow = _groundedCached;

            if (_input.JumpPressed &&
                groundedNow &&
                velocity.y <= 0f)
            {
                targetVelocity.y = _jumpVelocity;

                OnJumped?.Invoke();

                groundedNow = false;
                _groundedCached = false;
            }

            if (groundedNow &&
                !_wasGrounded &&
                _lastVerticalVelocity <= _landingFallSpeedThreshold)
            {
                OnLanded?.Invoke();
            }

            _wasGrounded = groundedNow;
            IsGrounded = groundedNow;

            _lastVerticalVelocity = velocity.y;

            _rb.linearVelocity = targetVelocity;
        }

        private float GetSpeedMultiplier()
        {
            if (_stance != null && _stance.IsCrouching)
                return _crouchSpeedMultiplier;

            if (_input.SprintPressed)
                return _sprintSpeedMultiplier;

            return 1f;
        }

        private void UpdateGroundCheck()
        {
            _groundCheckTimer -= Time.fixedDeltaTime;

            if (_groundCheckTimer > 0f)
                return;

            _groundCheckTimer = _groundCheckInterval;

            _groundedCached = IsGroundedCheck();
        }

        private bool IsGroundedCheck()
        {
            return Physics.CheckSphere(
                GetGroundCheckPosition(),
                _groundCheckRadius,
                _groundMask,
                QueryTriggerInteraction.Ignore);
        }

        private Vector3 GetGroundCheckPosition()
        {
            if (_collider != null)
            {
                Bounds bounds = _collider.bounds;

                return new Vector3(
                    bounds.center.x,
                    bounds.min.y + _groundCheckOffset,
                    bounds.center.z);
            }

            return _rb.position +
                   Vector3.down *
                   _groundCheckOffset;
        }

        private void OnDrawGizmosSelected()
        {
            Collider col = GetComponent<Collider>();

            Vector3 position;

            if (col != null)
            {
                Bounds bounds = col.bounds;

                position = new Vector3(
                    bounds.center.x,
                    bounds.min.y + _groundCheckOffset,
                    bounds.center.z);
            }
            else
            {
                position = transform.position +
                           Vector3.down *
                           _groundCheckOffset;
            }

            Gizmos.color =
                IsGrounded
                    ? Color.green
                    : Color.red;

            Gizmos.DrawWireSphere(
                position,
                _groundCheckRadius);
        }
    }
}