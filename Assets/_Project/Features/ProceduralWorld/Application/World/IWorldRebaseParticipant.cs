using UnityEngine;

namespace _Project.Features.ProceduralWorld.Application.World
{
    public interface IWorldRebaseParticipant
    {
        int Order { get; }

        void OnWorldRebased(Vector3 delta);
    }
}