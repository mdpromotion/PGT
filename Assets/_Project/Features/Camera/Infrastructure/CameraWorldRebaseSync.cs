using System;
using _Project.Features.ProceduralWorld.Application.World;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Features.Camera.Infrastructure
{
    public sealed class CameraWorldRebaseSync : IInitializable, IDisposable
    {
        private ICameraMotor _motor;
        private WorldRebaseService _worldRebaseService;

        [Inject]
        public void Construct(
            ICameraMotor motor,
            WorldRebaseService worldRebaseService)
        {
            _motor = motor;
            _worldRebaseService = worldRebaseService;
        }

        public void Initialize()
        {
            _worldRebaseService.WorldRebased += HandleWorldRebased;
        }

        public void Dispose()
        {
            _worldRebaseService.WorldRebased -= HandleWorldRebased;
        }

        private void HandleWorldRebased(Vector3 delta)
        {
            _motor.ApplyOriginShift(delta);
        }
    }
}