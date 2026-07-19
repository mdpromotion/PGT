using _Project.Features.Player.Presentation;
using UnityEngine;
using VContainer;

namespace _Project.Features.Camera.Infrastructure
{
    public sealed class FpsCameraController : MonoBehaviour
    {
        private Transform _cameraTransform;

        [SerializeField] private float sensitivity = 0.08f;
        [SerializeField] private bool invertY;

        private IPlayerInputReader _input;
        private IFpsPlayerMotor _motor;

        private float _pitch;

        [Inject]
        public void Construct(
            IPlayerInputReader input,
            IFpsPlayerMotor motor)
        {
            _input = input;
            _motor = motor;
        }

        private void Awake()
        {
            _cameraTransform = transform;
        }

        private void Update()
        {
            if (_input == null || _motor == null)
                return;

            Vector2 look = _input.Look * sensitivity;

            _motor.SetLookYaw(look.x);

            float y = invertY ? look.y : -look.y;

            _pitch = Mathf.Clamp(
                _pitch + y,
                -89f,
                89f);

            _cameraTransform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }
    }
}