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
        [Header("Ground Check")]
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _groundCheckRadius = 0.35f;
        [SerializeField] private float _groundCheckOffset = 0.05f;
        [SerializeField] private float _groundCheckRate = 10f;

        [Header("Landing")]
        [SerializeField] private float _landingFallSpeedThreshold = -3f;

        private IPlayerInputReader _input;
        private IPlayerStanceState _stance;
        //private IWaterState _waterState;

        private IMovementMode _groundMovement;
        private IMovementMode _waterMovement;

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
            //IWaterState waterState,
            GroundMovementUseCase groundMovement,
            SwimmingMovementUseCase waterMovement)
        {
            _input = input;
            _stance = stance;
            //_waterState = waterState;

            _groundMovement = groundMovement;
            _waterMovement = waterMovement;
        }


        public void SetLookYaw(float yawDelta)
        {
            _pendingYawDelta += yawDelta;
        }


        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();

            _rb.interpolation =
                RigidbodyInterpolation.Interpolate;

            _groundCheckInterval =
                1f / _groundCheckRate;
        }


        private void FixedUpdate()
        {
            bool swimming = false;


            _rb.useGravity = !swimming;


            _yaw += _pendingYawDelta;
            _pendingYawDelta = 0f;


            Quaternion rotation =
                Quaternion.Euler(
                    0f,
                    _yaw,
                    0f);


            _rb.MoveRotation(rotation);


            Vector3 forward =
                rotation *
                Vector3.forward;

            Vector3 right =
                rotation *
                Vector3.right;


            Vector3 velocity =
                _rb.linearVelocity;


            IMovementMode movementMode =
                swimming
                    ? _waterMovement
                    : _groundMovement;


            Vector3 targetVelocity =
                movementMode.BuildVelocity(
                    _input.Move,
                    forward,
                    right,
                    velocity);


            UpdateGroundCheck();


            bool groundedNow =
                _groundedCached &&
                !swimming;


            if (swimming)
            {
                if (_input.JumpPressed)
                {
                    movementMode.TryJump(
                        ref targetVelocity);
                }

                if (_input.CrouchPressed)
                {
                    movementMode.TryCrouch(
                        ref targetVelocity);
                }
            }
            else
            {
                if (_input.JumpPressed &&
                    groundedNow &&
                    velocity.y <= 0f)
                {
                    if (movementMode.TryJump(
                            ref targetVelocity))
                    {
                        OnJumped?.Invoke();
                    }
                }
            }


            if (_input.CrouchPressed)
            {
                movementMode.TryCrouch(
                    ref targetVelocity);
            }


            if (groundedNow &&
                !_wasGrounded &&
                _lastVerticalVelocity <=
                _landingFallSpeedThreshold)
            {
                OnLanded?.Invoke();
            }


            _wasGrounded = groundedNow;
            IsGrounded = groundedNow;

            _lastVerticalVelocity = velocity.y;


            _rb.linearVelocity =
                targetVelocity;
        }


        private void UpdateGroundCheck()
        {
            _groundCheckTimer -=
                Time.fixedDeltaTime;


            if (_groundCheckTimer > 0f)
                return;


            _groundCheckTimer =
                _groundCheckInterval;


            _groundedCached =
                IsGroundedCheck();
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
                Bounds bounds =
                    _collider.bounds;


                return new Vector3(
                    bounds.center.x,
                    bounds.min.y +
                    _groundCheckOffset,
                    bounds.center.z);
            }


            return _rb.position +
                   Vector3.down *
                   _groundCheckOffset;
        }


        private void OnDrawGizmosSelected()
        {
            Collider col =
                GetComponent<Collider>();


            Vector3 position;


            if (col != null)
            {
                Bounds bounds =
                    col.bounds;


                position =
                    new Vector3(
                        bounds.center.x,
                        bounds.min.y +
                        _groundCheckOffset,
                        bounds.center.z);
            }
            else
            {
                position =
                    transform.position +
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