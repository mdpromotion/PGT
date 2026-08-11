using System;
using _Project.Features.Camera.Infrastructure;
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

        [Header("Landing")]
        [SerializeField] private float _landingFallSpeedThreshold = -3f;

        private IPlayerInputReader _input;

        private Rigidbody _rb;
        private Collider _collider;
        
        public Vector3 CurrentVelocity => _rb.linearVelocity;
        
        [Inject]
        public void Construct(
            IPlayerInputReader input)
        {
            _input = input;
        }
        
        public void Freeze(bool state)
        {
            if (!_rb)
                return;
            
            if (state)
            {
                _rb.isKinematic = true;
                _rb.useGravity = false;
            }
            else
            {
                _rb.isKinematic = false;
                _rb.useGravity = true;
            }
        }


        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();

            _rb.interpolation =
                RigidbodyInterpolation.Interpolate;
        }

        public void SetVelocity(Vector3 velocity)
        {
            _rb.linearVelocity =
                velocity;
        }

        public void SetRotation(Quaternion rotation)
        {
            _rb.MoveRotation(rotation);
        }

        public bool IsGroundedCheck()
        {
            return Physics.CheckSphere(
                GetGroundCheckPosition(),
                _groundCheckRadius,
                _groundMask,
                QueryTriggerInteraction.Ignore);
        }


        private Vector3 GetGroundCheckPosition()
        {
            if (_collider)
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
    }
}