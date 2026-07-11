using _Project.Features.ProceduralWorld.Application;
using _Project.Features.ProceduralWorld.Domain;
using UnityEngine;
using VContainer;

namespace _Project.Features.ProceduralWorld.Presentation
{
    public class ProceduralWorldPresenter : MonoBehaviour
    {
        [SerializeField]
        private NoiseSettings noiseSettings;

        private ChunkManager _chunkManager;
        private WorldStreamer _worldStreamer;

        [Inject]
        public void Construct(ChunkManager chunkManager,  WorldStreamer worldStreamer)
        {
            _chunkManager = chunkManager;
            _worldStreamer = worldStreamer;
            Debug.Log(_worldStreamer);
        }
        
        private void Update()
        {
            _worldStreamer.Update();
        }
        
        [ContextMenu("Generate Center")]
        private void Generate()
        {
            _chunkManager.Load(
                new ChunkCoordinate(0, 0),
                noiseSettings);
        }

        [ContextMenu("Generate Neighbors")]
        private void GenerateAround()
        {
            ChunkCoordinate center = new ChunkCoordinate(0, 0);

            _chunkManager.Load(center + new ChunkCoordinate(1, 0), noiseSettings);
            _chunkManager.Load(center + new ChunkCoordinate(-1, 0), noiseSettings);
            _chunkManager.Load(center + new ChunkCoordinate(0, 1), noiseSettings);
            _chunkManager.Load(center + new ChunkCoordinate(0, -1), noiseSettings);
        }

        [ContextMenu("Unload Neighbors")]
        private void UnloadAround()
        {
            ChunkCoordinate center = new ChunkCoordinate(0, 0);

            _chunkManager.Unload(center + new ChunkCoordinate(1, 0));
            _chunkManager.Unload(center + new ChunkCoordinate(-1, 0));
            _chunkManager.Unload(center + new ChunkCoordinate(0, 1));
            _chunkManager.Unload(center + new ChunkCoordinate(0, -1));
        }
    }
}