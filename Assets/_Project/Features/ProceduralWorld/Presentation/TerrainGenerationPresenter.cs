using _Project.Features.ProceduralWorld.Application;
using _Project.Features.ProceduralWorld.Domain;
using UnityEngine;
using VContainer;


namespace _Project.Features.TerrainGeneration.Presentation
{
    public class TerrainGenerationPresenter : MonoBehaviour
    {
        [SerializeField]
        private NoiseSettings noiseSettings;


        private ChunkManager _chunkManager;


        [Inject]
        public void Construct(
            ChunkManager world)
        {
            _chunkManager = world;
        }



        [ContextMenu("Generate Center")]
        private void Generate()
        {
            _chunkManager.Generate(
                new ChunkCoordinate(0,0),
                noiseSettings,
                transform);
        }



        [ContextMenu("Generate Neighbors")]
        private void GenerateAround()
        {
            ChunkCoordinate center =
                new ChunkCoordinate(0,0);


            _chunkManager.Generate(
                center + new ChunkCoordinate(1,0),
                noiseSettings,
                transform);


            _chunkManager.Generate(
                center + new ChunkCoordinate(-1,0),
                noiseSettings,
                transform);


            _chunkManager.Generate(
                center + new ChunkCoordinate(0,1),
                noiseSettings,
                transform);


            _chunkManager.Generate(
                center + new ChunkCoordinate(0,-1),
                noiseSettings,
                transform);
        }
    }
}