using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    [CreateAssetMenu(menuName = "Procedural World/Hydrology Settings")]
    public class HydrologySettings : ScriptableObject
    {
        public int ZoneSizeInChunks = 16;
        
        public int CellsPerChunk = 8;
        
        public int HaloCells = 24;
        
        public float MinSlopeEpsilon = 1e-5f;
    }
}