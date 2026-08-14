using UnityEngine;

namespace _Project.Features.ProceduralWorld.Domain.World
{
    [CreateAssetMenu(menuName = "Procedural World/World Rebase Settings")]
    public class WorldRebaseSettings : ScriptableObject
    {
        [Min(1)]
        public int ThresholdChunks = 64;
    }
}