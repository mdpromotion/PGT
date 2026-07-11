using _Project.Features.Player.Presentation;
using UnityEngine;
using VContainer;

namespace _Project.Features.Camera.Presentation
{
    public sealed class FpsCameraController : MonoBehaviour
    {
        private Transform _cameraTransform;

        [SerializeField] private float _sensitivity = 0.08f;
        [SerializeField] private bool _invertY;

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

        private void Awake() => _cameraTransform = transform;

        private void Update()
        {
            if (_input == null || _motor == null)
                return;

            var look = _input.Look * _sensitivity;

            _motor.SetLookYaw(look.x);

            var y = _invertY ? look.y : -look.y;
            
            _pitch = Mathf.Clamp(
                _pitch + y,
                -89f,
                89f
            );
            
            _cameraTransform.localRotation = Quaternion.Euler(_pitch, 0, 0);
        }
    }
}