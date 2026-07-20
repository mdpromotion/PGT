using _Project.Features.Player.Domain;
using _Project.Features.Player.Presentation;
using UnityEngine;
using VContainer;

namespace _Project.Features.Player.Infrastructure
{
    [RequireComponent(typeof(CapsuleCollider))]
    public sealed class PlayerStanceController : MonoBehaviour, IPlayerStanceState
    {
        [Header("Crouch")]
        [SerializeField] private float _crouchHeightMultiplier = 0.65f;
        [SerializeField] private Vector3 _crouchCenterOffset = new(0f, -0.35f, 0f);
        [SerializeField] private float _stanceChangeCooldown = 0.15f;

        [Header("Stand Check")]
        [SerializeField] private LayerMask _obstacleMask;
        [SerializeField] private float _standCheckPadding = 0.02f;

        private IPlayerInputReader _input;

        private CapsuleCollider _capsule;

        private float _standingHeight;
        private Vector3 _standingCenter;

        private float _crouchingHeight;
        private Vector3 _crouchingCenter;

        private bool _isCrouching;

        private float _changeTimer;

        private readonly Collider[] _overlapBuffer = new Collider[8];

        public bool IsCrouching => _isCrouching;

        public float CrouchBlend => _isCrouching ? 1f : 0f;

        [Inject]
        public void Construct(
            IPlayerInputReader input)
        {
            _input = input;
        }

        private void Awake()
        {
            _capsule = GetComponent<CapsuleCollider>();

            CacheDimensions();
        }

        private void Update()
        {
            _changeTimer -= Time.deltaTime;

            if (_changeTimer > 0f)
                return;

            bool wantsCrouch = _input.CrouchPressed;

            if (wantsCrouch && !_isCrouching)
            {
                EnterCrouch();
            }
            else if (!wantsCrouch && _isCrouching)
            {
                TryStand();
            }
        }

        private void EnterCrouch()
        {
            _isCrouching = true;
            _changeTimer = _stanceChangeCooldown;

            _capsule.height = _crouchingHeight;
            _capsule.center = _crouchingCenter;
        }

        private void TryStand()
        {
            if (!CanStand())
                return;

            _isCrouching = false;
            _changeTimer = _stanceChangeCooldown;

            _capsule.height = _standingHeight;
            _capsule.center = _standingCenter;
        }

        private bool CanStand()
        {
            Vector3 center =
                transform.TransformPoint(_standingCenter);

            float radius = _capsule.radius;

            float offset =
                Mathf.Max(
                    0f,
                    _standingHeight * 0.5f - radius);

            Vector3 up = transform.up;

            int count = Physics.OverlapCapsuleNonAlloc(
                center + up * offset,
                center - up * offset,
                radius + _standCheckPadding,
                _overlapBuffer,
                _obstacleMask,
                QueryTriggerInteraction.Ignore);

            for (int i = 0; i < count; i++)
            {
                if (_overlapBuffer[i] != null &&
                    _overlapBuffer[i] != _capsule)
                {
                    return false;
                }
            }

            return true;
        }

        private void CacheDimensions()
        {
            _standingHeight = _capsule.height;
            _standingCenter = _capsule.center;

            _crouchingHeight =
                _standingHeight *
                _crouchHeightMultiplier;

            _crouchingCenter =
                _standingCenter +
                _crouchCenterOffset;
        }
    }
}