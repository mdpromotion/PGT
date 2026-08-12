using _Project.Features.Camera.Infrastructure;
using _Project.Features.Core.Domain;
using _Project.Features.Core.Presentation;
using _Project.Features.Player.Application;
using _Project.Features.Player.Domain;
using UnityEngine;
using VContainer.Unity;

namespace _Project.Features.Camera.Application
{
    public class CameraController : ILateTickable
    {
        private readonly ICameraMotor _cameraMotor;
        private readonly PlayerCameraConfig _cameraConfig;
        private readonly IPlayerInputReader _input;
        private readonly IPlayerController _controller;
        private readonly IPlayerStanceState _stance;
        private readonly IGameState _gameState;
        
        private float _pitch;
        private float _currentHeight;
        
        public CameraController(
            ICameraMotor cameraMotor,  
            PlayerCameraConfig cameraConfig,
            IPlayerInputReader input,  
            IPlayerController controller, 
            IPlayerStanceState stance, 
            IGameState gameState )
        {
            _cameraMotor = cameraMotor;
            _cameraConfig = cameraConfig;
            _input = input;
            _controller = controller;
            _stance = stance;
            _gameState = gameState;
            
            _currentHeight = _cameraMotor.GetCurrentHeight();
        }
        
        public void LateTick()
        {
            if (_gameState.Paused)
                return;
            
            UpdateLook();

            UpdateCameraHeight();
        }
        
        private void UpdateLook()
        {
            Vector2 look = _input.Look * _cameraConfig.sensitivity;
            
            _controller.SetLookYaw(look.x);
            
            float y = _cameraConfig.invertY ? look.y : -look.y;
            
            _pitch = Mathf.Clamp(_pitch + y, -89f, 89f);

            _cameraMotor.SetRotation(Quaternion.Euler(_pitch, 0f, 0f));
        }
        
        private void UpdateCameraHeight()
        {
            float targetHeight = Mathf.Lerp(_cameraConfig.standingHeight, _cameraConfig.crouchingHeight, _stance.CrouchBlend);

            _currentHeight = Mathf.Lerp(_currentHeight, targetHeight, _cameraConfig.heightSmoothSpeed * Time.deltaTime);
            
            Vector3 position = _cameraMotor.Position;
            
            position.y = _currentHeight;
            
            _cameraMotor.SetPosition(position);
        }
    }
}