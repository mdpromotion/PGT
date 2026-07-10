using UnityEngine;

namespace _Project.Features.TerrainGeneration.Presentation
{
    [DisallowMultipleComponent]
    public class TerrainChunkOffset : MonoBehaviour
    {
        [SerializeField]
        private Vector2 offset;

        public Vector2 Offset => offset;

        public void Initialize(Vector2 value)
        {
            offset = value;

            Debug.Log(
                $"[{name}] Initialized offset {offset}");
        }
    }
}