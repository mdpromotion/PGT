using UnityEngine;

namespace _Project.Features.TerrainGeneration.Infrastructure
{
    public class TerrainChunkFactory
    {
        public Terrain CreateChunk(Terrain source, int chunkX, int chunkZ, Transform parent)
        {
            var terrainData = new TerrainData
            {
                heightmapResolution = source.terrainData.heightmapResolution,
                size = source.terrainData.size
            };

            var go = new GameObject($"Terrain_Chunk_{chunkX}_{chunkZ}");
            go.transform.SetParent(parent, worldPositionStays: false);

            var terrain = go.AddComponent<Terrain>();
            terrain.terrainData = terrainData;

            var collider = go.AddComponent<TerrainCollider>();
            collider.terrainData = terrainData;

            Vector3 sourcePos = source.transform.position;
            Vector3 size = terrainData.size;
            go.transform.position = new Vector3(
                sourcePos.x + chunkX * size.x,
                sourcePos.y,
                sourcePos.z + chunkZ * size.z
            );

            return terrain;
        }
    }
}