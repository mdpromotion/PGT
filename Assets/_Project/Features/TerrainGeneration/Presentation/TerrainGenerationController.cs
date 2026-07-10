using System.Collections.Generic;
using _Project.Features.TerrainGeneration.Application;
using _Project.Features.TerrainGeneration.Domain;
using _Project.Features.TerrainGeneration.Infrastructure;
using UnityEngine;

namespace _Project.Features.TerrainGeneration.Presentation
{
    public class TerrainGenerationController : MonoBehaviour
    {
        [SerializeField] private NoiseSettings noiseSettings;
        [SerializeField] private Vector2 worldOffset;
        [SerializeField] private Terrain chunkPrefab;

        private GenerateTerrainUseCase _generateTerrainUseCase;


        private void Awake()
        {
            Initialize();
        }


        [ContextMenu("Generate")]
        private void Generate()
        {
            EnsureInitialized();

            Terrain[] chunks =
                GetComponentsInChildren<Terrain>();

            foreach (Terrain chunk in chunks)
            {
                Vector2 offset =
                    GetChunkOffset(chunk);

                Debug.Log(
                    $"Generate {chunk.name} offset={offset}");

                GenerateChunk(
                    chunk,
                    offset);
            }
        }


        [ContextMenu("Generate Chunks Around")]
        private void GenerateChunksAround()
        {
            EnsureInitialized();


            // ВАЖНО:
            // фиксируем список до создания новых чанков
            Terrain[] existingChunks =
                GetComponentsInChildren<Terrain>();


            HashSet<Vector2> generatedOffsets =
                new HashSet<Vector2>();


            foreach (Terrain chunk in existingChunks)
            {
                Vector2 center =
                    GetChunkOffset(chunk);


                generatedOffsets.Add(center);


                TryCreateNeighbor(
                    center,
                    Vector2.right,
                    generatedOffsets);


                TryCreateNeighbor(
                    center,
                    Vector2.left,
                    generatedOffsets);


                TryCreateNeighbor(
                    center,
                    Vector2.up,
                    generatedOffsets);


                TryCreateNeighbor(
                    center,
                    Vector2.down,
                    generatedOffsets);
            }
        }



        private void TryCreateNeighbor(
            Vector2 current,
            Vector2 direction,
            HashSet<Vector2> generatedOffsets)
        {
            Vector2 neighborOffset =
                current +
                new Vector2(
                    direction.x * GetChunkSize().x,
                    direction.y * GetChunkSize().y);


            if (FindChunkByOffset(neighborOffset) != null)
                return;


            if (generatedOffsets.Contains(neighborOffset))
                return;


            Terrain chunk =
                InstantiateChunk(neighborOffset);


            GenerateChunk(
                chunk,
                neighborOffset);
        }



        private Terrain InstantiateChunk(Vector2 offset)
        {
            Vector3 position =
                new Vector3(
                    offset.x,
                    0,
                    offset.y);


            Terrain terrain =
                Instantiate(
                    chunkPrefab,
                    position,
                    Quaternion.identity,
                    transform);


            terrain.name =
                $"Chunk [{offset.x},{offset.y}]";


            terrain.terrainData =
                CreateTerrainData();


            TerrainChunkOffset holder =
                terrain.GetComponent<TerrainChunkOffset>();


            if (holder == null)
            {
                holder =
                    terrain.gameObject
                        .AddComponent<TerrainChunkOffset>();
            }


            holder.Initialize(offset);


            Debug.Log(
                $"Created {terrain.name}, " +
                $"data={terrain.terrainData.GetInstanceID()}");


            return terrain;
        }
        
        private TerrainData CreateTerrainData()
        {
            TerrainData source =
                chunkPrefab.terrainData;


            TerrainData data =
                new TerrainData();


            data.heightmapResolution =
                source.heightmapResolution;


            data.size =
                source.size;


            data.alphamapResolution =
                source.alphamapResolution;


            data.baseMapResolution =
                source.baseMapResolution;


            data.SetDetailResolution(
                source.detailResolution,
                source.detailResolutionPerPatch);


            data.terrainLayers =
                source.terrainLayers;


            return data;
        }


        private void GenerateChunk(
            Terrain terrain,
            Vector2 offset)
        {
            Debug.Log(
                $"Generating {terrain.name}, " +
                $"data={terrain.terrainData.GetInstanceID()}, " +
                $"offset={offset}");


            _generateTerrainUseCase.Execute(
                terrain.terrainData,
                noiseSettings,
                offset);
        }



        private Terrain FindChunkByOffset(Vector2 offset)
        {
            Terrain[] chunks =
                GetComponentsInChildren<Terrain>();


            foreach (Terrain chunk in chunks)
            {
                TerrainChunkOffset holder =
                    chunk.GetComponent<TerrainChunkOffset>();


                if (holder != null &&
                    holder.Offset == offset)
                {
                    return chunk;
                }
            }


            return null;
        }



        private Vector2 GetChunkOffset(Terrain terrain)
        {
            TerrainChunkOffset holder =
                terrain.GetComponent<TerrainChunkOffset>();


            if (holder == null)
            {
                holder =
                    terrain.gameObject
                    .AddComponent<TerrainChunkOffset>();


                Vector2 offset =
                    worldOffset +
                    new Vector2(
                        terrain.transform.position.x,
                        terrain.transform.position.z);


                holder.Initialize(offset);
            }


            return holder.Offset;
        }



        private Vector2 GetChunkSize()
        {
            return new Vector2(
                chunkPrefab.terrainData.size.x,
                chunkPrefab.terrainData.size.z);
        }



        private void EnsureInitialized()
        {
            if (_generateTerrainUseCase == null)
                Initialize();
        }



        private void Initialize()
        {
            IHeightmapGenerator generator =
                new PerlinHeightmapGenerator();


            UnityTerrainWriter writer =
                new UnityTerrainWriter();


            _generateTerrainUseCase =
                new GenerateTerrainUseCase(
                    generator,
                    writer);
        }
    }
}