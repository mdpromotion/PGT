using _Project.Features.Player.Presentation;
using UnityEngine;

namespace _Project.Features.Player.Infrastructure
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public sealed class FpsPlayerMotor : MonoBehaviour, IFpsPlayerMotor
    {
        [Header("Ground Check")]
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float groundCheckRadius = 0.35f;
        [SerializeField] private float groundCheckOffset = 0.05f;

        private Rigidbody _rb;
        private Collider _collider;

        public Vector3 CurrentVelocity =>
            _rb.linearVelocity;

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
                groundCheckRadius,
                groundMask,
                QueryTriggerInteraction.Ignore);
        }

        public bool TryGetSafeGroundPosition(
            out Vector3 position)
        {
            Bounds bounds =
                _collider.bounds;

            Vector3 origin =
                new Vector3(
                    bounds.center.x,
                    bounds.min.y +
                    groundCheckOffset,
                    bounds.center.z);

            if (Physics.Raycast(
                    origin,
                    Vector3.down,
                    out RaycastHit hit,
                    Mathf.Infinity,
                    groundMask,
                    QueryTriggerInteraction.Ignore))
            {
                float bottomOffset =
                    _rb.position.y -
                    bounds.min.y;

                position =
                    new Vector3(
                        _rb.position.x,
                        hit.point.y +
                        bottomOffset,
                        _rb.position.z);

                return true;
            }

            position = default;

            return false;
        }

        public void TeleportToPosition(
            Vector3 position)
        {
            _rb.position =
                position;

            _rb.linearVelocity =
                Vector3.zero;
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
                    groundCheckOffset,
                    bounds.center.z);
            }

            return _rb.position +
                   Vector3.down *
                   groundCheckOffset;
        }
    }
}