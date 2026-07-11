using _Project.Features.Player.Domain;
using UnityEngine;

namespace _Project.Features.Player.Infrastructure
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class RigidbodyPlayerState : MonoBehaviour, IPlayerReadOnly
    {
        [SerializeField] private Rigidbody _rb;

        public Vector3 Position => _rb.position;
        public Vector3 Velocity => _rb.linearVelocity;


        private void Awake()
        {
            if (_rb == null)
                _rb = GetComponent<Rigidbody>();

            Debug.Log($"[RigidbodyPlayerState] Awake on '{gameObject.name}', parent scope should find me here.", this);
        }


        private void Update()
        {
            if (Time.frameCount % 60 == 0)
            {
                Debug.Log(
                    $"[PlayerState] Position={Position}, Velocity={Velocity}",
                    this
                );
            }
        }
    }
}