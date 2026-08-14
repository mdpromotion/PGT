using System;
using _Project.Features.Player.Presentation;
using _Project.Features.ProceduralWorld.Application.World;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Features.Player.Infrastructure
{
    public sealed class PlayerWorldRebaseSync : IInitializable, IDisposable
    {
        private IFpsPlayerMotor _motor;
        private WorldRebaseService _worldRebaseService;

        [Inject]
        public void Construct(
            IFpsPlayerMotor motor,
            WorldRebaseService worldRebaseService)
        {
            _motor = motor;
            _worldRebaseService = worldRebaseService;
        }
        
        public void Initialize()
        {
            _worldRebaseService.WorldRebased += HandleWorldRebased;
        }

        private void HandleWorldRebased(Vector3 delta)
        {
            _motor.ApplyOriginShift(delta);
        }

        public void Dispose()
        {
            _worldRebaseService.WorldRebased -= HandleWorldRebased;
        }
    }
}