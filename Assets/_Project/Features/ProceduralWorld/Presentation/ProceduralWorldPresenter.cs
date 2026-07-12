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
        public void Construct(
            ChunkManager chunkManager,
            WorldStreamer worldStreamer)
        {
            _chunkManager = chunkManager;
            _worldStreamer = worldStreamer;
        }

        private void Update()
        {
            _worldStreamer.Update();

            _chunkManager.Tick();
        }

        [ContextMenu("Generate Center")]
        private void Generate()
        {
            _chunkManager.QueueLoad(
                new ChunkCoordinate(0, 0),
                noiseSettings);

            _chunkManager.Tick();
        }

        [ContextMenu("Generate Neighbors")]
        private void GenerateAround()
        {
            ChunkCoordinate center = new(0, 0);

            _chunkManager.QueueLoad(center + new ChunkCoordinate(1, 0), noiseSettings);
            _chunkManager.QueueLoad(center + new ChunkCoordinate(-1, 0), noiseSettings);
            _chunkManager.QueueLoad(center + new ChunkCoordinate(0, 1), noiseSettings);
            _chunkManager.QueueLoad(center + new ChunkCoordinate(0, -1), noiseSettings);

            //_chunkManager.FlushQueue();
        }

        [ContextMenu("Unload Neighbors")]
        private void UnloadAround()
        {
            ChunkCoordinate center = new(0, 0);

            _chunkManager.Unload(center + new ChunkCoordinate(1, 0));
            _chunkManager.Unload(center + new ChunkCoordinate(-1, 0));
            _chunkManager.Unload(center + new ChunkCoordinate(0, 1));
            _chunkManager.Unload(center + new ChunkCoordinate(0, -1));
        }
    }
}