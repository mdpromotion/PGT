using _Project.Features.ProceduralWorld.Application;
using _Project.Features.ProceduralWorld.Domain;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure
{
    public class UnityTerrainWriter : ITerrainWriter
    {
        private float[,] _buffer;


        public void Write(
            Terrain terrain,
            ChunkGenerationResult result)
        {
            EnsureBuffer(result.Resolution);


            FillBuffer(result);


            terrain.terrainData
                .SetHeightsDelayLOD(
                    0,
                    0,
                    _buffer);
        }



        private void EnsureBuffer(
            int resolution)
        {
            if (_buffer != null &&
                _buffer.GetLength(0) == resolution)
                return;


            _buffer =
                new float[
                    resolution,
                    resolution];
        }



        private void FillBuffer(
            ChunkGenerationResult result)
        {
            int resolution =
                result.Resolution;


            for(int y = 0; y < resolution; y++)
            {
                for(int x = 0; x < resolution; x++)
                {
                    _buffer[y,x] =
                        result.Heights[
                            y * result.Resolution + x];
                }
            }
        }
    }
}