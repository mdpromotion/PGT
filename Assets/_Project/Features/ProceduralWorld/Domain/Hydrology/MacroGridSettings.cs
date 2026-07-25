using UnityEngine;

namespace _Project.Features.ProceduralWorld.Domain.Hydrology
{
    [CreateAssetMenu(menuName = "Procedural World/Macro Grid Settings")]
    public sealed class MacroGridSettings : ScriptableObject
    {
        [Min(1f)]
        public float CellSize = 512f;

        [Min(1)]
        public int TileCells = 16;

        [Min(0)]
        public int PaddingCells = 24;

        public int PaddedSize => TileCells + 2 * PaddingCells;

        public float TileWorldSize => TileCells * CellSize;
    }
}