using _Project.Features.ProceduralWorld.Application;
using _Project.Features.ProceduralWorld.Application.Chunks;
using _Project.Features.ProceduralWorld.Application.Hydrology;
using _Project.Features.ProceduralWorld.Application.World;
using UnityEngine;
using VContainer;

namespace _Project.Features.ProceduralWorld.Presentation
{
    public class ProceduralWorldPresenter : MonoBehaviour
    {
        private ChunkManager _chunkManager;

        private WorldStreamer _worldStreamer;

        private HydrologyService _hydrology;



        [Inject]
        public void Construct(
            ChunkManager chunkManager,
            WorldStreamer worldStreamer,
            HydrologyService hydrology)
        {
            _chunkManager = chunkManager;

            _worldStreamer = worldStreamer;

            _hydrology = hydrology;
        }



        private void Update()
        {
            if (_chunkManager == null || _worldStreamer == null || _hydrology == null) return;
            
            _worldStreamer.Update();

            _chunkManager.Tick();

            _hydrology.Tick();
        }
    }
}