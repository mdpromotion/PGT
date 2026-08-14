using _Project.Features.Player.Presentation;
using _Project.Features.ProceduralWorld.Application.World;
using UnityEngine;
using VContainer;

namespace _Project.Features.Player.Infrastructure
{
    public sealed class PlayerWorldRebaseSync : IWorldRebaseParticipant
    {
        private readonly IFpsPlayerMotor _motor;

        public int Order => 100;

        [Inject]
        public PlayerWorldRebaseSync(IFpsPlayerMotor motor)
        {
            _motor = motor;
        }

        public void OnWorldRebased(Vector3 delta)
        {
            _motor.ApplyOriginShift(delta);
        }
    }
}