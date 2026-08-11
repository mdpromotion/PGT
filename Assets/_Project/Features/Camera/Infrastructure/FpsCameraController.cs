using _Project.Features.Player.Application;
using _Project.Features.Player.Domain;
using _Project.Features.Player.Presentation;
using UnityEngine;
using VContainer;

namespace _Project.Features.Camera.Infrastructure
{
    public sealed class FpsCameraController : MonoBehaviour
    {
        private Transform _cameraTransform;


        [Header("Look")]
        [SerializeField]
        private float sensitivity = 0.08f;


        [SerializeField]
        private bool invertY;


        [Header("Crouch Camera")]
        [SerializeField]
        private float _standingHeight = 1.7f;


        [SerializeField]
        private float _crouchingHeight = 1.15f;


        [SerializeField]
        private float _heightSmoothSpeed = 12f;



        private IPlayerInputReader _input;

        private IPlayerController _controller;

        private IPlayerStanceState _stance;



        private float _pitch;

        private float _currentHeight;


        [Inject]
        public void Construct(
            IPlayerInputReader input,
            IPlayerController controller,
            IPlayerStanceState stance)
        {
            _input = input;

            _controller = controller;

            _stance = stance;
        }



        private void Awake()
        {
            _cameraTransform = transform;

            _currentHeight =
                _cameraTransform.localPosition.y;
        }



        private void Update()
        {
            if (_input == null ||
                _controller == null ||
                _stance == null)
            {
                return;
            }


            UpdateLook();

            UpdateCameraHeight();
        }



        private void UpdateLook()
        {
            Vector2 look =
                _input.Look *
                sensitivity;


            _controller.SetLookYaw(
                look.x);


            float y =
                invertY
                    ? look.y
                    : -look.y;


            _pitch =
                Mathf.Clamp(
                    _pitch + y,
                    -89f,
                    89f);


            _cameraTransform.localRotation =
                Quaternion.Euler(
                    _pitch,
                    0f,
                    0f);
        }



        private void UpdateCameraHeight()
        {
            float targetHeight =
                Mathf.Lerp(
                    _standingHeight,
                    _crouchingHeight,
                    _stance.CrouchBlend);



            _currentHeight =
                Mathf.Lerp(
                    _currentHeight,
                    targetHeight,
                    _heightSmoothSpeed *
                    Time.deltaTime);



            Vector3 position =
                _cameraTransform.localPosition;


            position.y =
                _currentHeight;


            _cameraTransform.localPosition =
                position;
        }
    }
}