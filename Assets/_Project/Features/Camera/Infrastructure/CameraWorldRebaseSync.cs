using _Project.Features.ProceduralWorld.Application.World;
using UnityEngine;
using VContainer;

namespace _Project.Features.Camera.Infrastructure
{
    public sealed class CameraWorldRebaseSync : IWorldRebaseParticipant
    {
        private readonly ICameraMotor _motor;

        public int Order => 300;

        [Inject]
        public CameraWorldRebaseSync(ICameraMotor motor)
        {
            _motor = motor;
        }

        public void OnWorldRebased(Vector3 delta)
        {
            _motor.ApplyOriginShift(delta);
        }
    }
}