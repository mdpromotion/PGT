using _Project.Features.ProceduralWorld.Application.Chunks;
using _Project.Features.ProceduralWorld.Application.World;
using UnityEngine;
using VContainer;

namespace _Project.Features.ProceduralWorld.Presentation
{
    public class ProceduralWorldPresenter : MonoBehaviour
    {
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
            if (_chunkManager == null || _worldStreamer == null) return;
            
            _worldStreamer.Update();

            _chunkManager.Tick();
        }
    }
}