using System;
using _Project.Features.Camera.Infrastructure;
using _Project.Features.Player.Domain;
using _Project.Features.Player.Presentation;
using UnityEngine;
using VContainer.Unity;

namespace _Project.Features.Player.Application
{
    public class PlayerController : IFixedTickable, IPlayerController
    {
        private float _yaw;
        private float _pendingYawDelta;
        
        private float _lastVerticalVelocity;
        
        private bool _wasGrounded;
        private bool _groundedCached;
        private bool _isFrozen = false;
        
        public bool IsGrounded => _groundedCached;
        
        private float _groundCheckTimer;
        private float _groundCheckInterval;
        
        private IMovementMode _groundMovement;
        private IMovementMode _waterMovement;
        
        private readonly IPlayerInputReader _input;
        private readonly IFpsPlayerMotor _playerMotor;
        private readonly IWaterState _waterState;
        
        
        private const float LandingFallSpeedThreshold = -3f;
        private const float GroundCheckRate = 10f;
        
        public event Action OnJumped;
        public event Action OnLanded;


        public PlayerController(IFpsPlayerMotor playerMotor, 
            GroundMovementUseCase groundMovement, 
            SwimmingMovementUseCase waterMovement,
            IPlayerInputReader input,
            IWaterState waterState)
        {
            _playerMotor = playerMotor;
            _groundMovement = groundMovement;
            _waterMovement = waterMovement;
            _input = input;
            _waterState = waterState;
            
            _groundCheckInterval = 1 / GroundCheckRate;
        }
        
        
        public void FixedTick()
        {
            UpdateGroundCheck();
            
            bool swimming = _waterState.IsInWater;

            bool groundedNow =
                _groundedCached &&
                !swimming;
            
            _yaw += _pendingYawDelta;
            _pendingYawDelta = 0f;
            
            Quaternion rotation =
                Quaternion.Euler(
                    0f,
                    _yaw,
                    0f);
            
            _playerMotor.SetRotation(rotation);
            
            Vector3 forward =
                rotation *
                Vector3.forward;

            Vector3 right =
                rotation *
                Vector3.right;
            
            Vector3 velocity =
                _playerMotor.CurrentVelocity;
            
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
                LandingFallSpeedThreshold)
            {
                OnLanded?.Invoke();
            }


            _wasGrounded = groundedNow;

            _lastVerticalVelocity = velocity.y;
        }
        
        public void SetLookYaw(float yawDelta)
        {
            _pendingYawDelta += yawDelta;
        }

        public void Freeze()
        {
            _isFrozen = true;
            _playerMotor.Freeze(_isFrozen);
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
                _playerMotor.IsGroundedCheck();
        }
    }
}
