using _Project.Features.TerrainGeneration.Application;
using _Project.Features.TerrainGeneration.Domain;
using _Project.Features.TerrainGeneration.Infrastructure;
using UnityEngine;

namespace _Project.Features.TerrainGeneration.Presentation
{
    [RequireComponent(typeof(Terrain))]
    public class TerrainGenerationController : MonoBehaviour
    {
        [SerializeField] private NoiseSettings _noiseSettings;
        [SerializeField] private Vector2 _worldOffset;
        [SerializeField] private ChunkGridSettings _gridSettings;
        [SerializeField] private Transform _chunksParent;

        private GenerateTerrainUseCase _generateTerrainUseCase;
        private GenerateTerrainGridUseCase _generateGridUseCase;

        private void Awake() => Initialize();

        [ContextMenu("Generate")]
        private void Generate()
        {
            EnsureInitialized();
            var terrain = GetComponent<Terrain>();
            _generateTerrainUseCase.Execute(terrain.terrainData, _noiseSettings, _worldOffset);
        }

        [ContextMenu("Generate With Neighbours")]
        private void GenerateWithNeighbours()
        {
            EnsureInitialized();
            var terrain = GetComponent<Terrain>();
            Transform parent = _chunksParent != null ? _chunksParent : transform.parent;

            _generateGridUseCase.Execute(terrain, _noiseSettings, _worldOffset, _gridSettings, parent);
        }

        private void EnsureInitialized()
        {
            if (_generateTerrainUseCase == null)
                Initialize();
        }

        private void Initialize()
        {
            // I'll replace this DI to Zenject later.
            IHeightmapGenerator generator = new PerlinHeightmapGenerator();
            var writer = new UnityTerrainWriter();
            _generateTerrainUseCase = new GenerateTerrainUseCase(generator, writer);

            var chunkFactory = new TerrainChunkFactory();
            _generateGridUseCase = new GenerateTerrainGridUseCase(_generateTerrainUseCase, chunkFactory);
        }
    }
}