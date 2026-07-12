using _Project.Features.Camera.Presentation;
using _Project.Features.Player.Application;
using UnityEngine;
using VContainer;

namespace _Project.Features.Player.Presentation
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class FpsPlayerMotor : MonoBehaviour, IFpsPlayerMotor
    {
        private Transform _viewRoot;
        private Rigidbody _rb;

        [SerializeField] private float _moveSpeed = 6f;
        [SerializeField] private float _jumpVelocity = 5f;

        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _groundCheckDistance = 0.15f;

        private IPlayerInputReader _input;
        private FpsMovementUseCase _useCase;

        private bool _jumpQueued;
        private float _yaw;
        
        
        [Inject]
        public void Construct(IPlayerInputReader input, FpsMovementUseCase useCase)
        {
            _input = input;
            _useCase = useCase;
        }
        
        public void SetLookYaw(float yaw)
        {
            _yaw += yaw;

            transform.rotation = Quaternion.Euler(
                0f,
                _yaw,
                0f
            );
        }

        private void Awake()
        {
            _viewRoot = transform;
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (_input.JumpPressedThisFrame)
                _jumpQueued = true;
        }

        private void FixedUpdate()
        {
            var currentVelocity = _rb.linearVelocity;

            var targetVelocity = _useCase.BuildVelocity(
                _input.Move,
                _viewRoot.forward,
                _viewRoot.right,
                currentVelocity,
                _moveSpeed
            );

            /*if (_jumpQueued)
            {
                if (IsGrounded())
                    targetVelocity.y = _jumpVelocity;
            }*/

            _rb.linearVelocity = targetVelocity;

            _jumpQueued = false;
        }
        
    }
}