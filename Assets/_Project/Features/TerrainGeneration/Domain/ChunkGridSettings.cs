using UnityEngine;

namespace _Project.Features.TerrainGeneration.Domain
{
    [System.Serializable]
    public class ChunkGridSettings
    {
        [Min(0)] public int Radius = 1;
    }
}