using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Application;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Infrastructure;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Application
{
    public class ChunkManager
    {
        private readonly TerrainChunkFactory _factory;
        private readonly IChunkGenerator _generator;
        private readonly Transform _parent;


        private readonly Dictionary<ChunkCoordinate, Terrain> _chunks = new();
        private readonly Queue<ChunkGenerationRequest> _queue = new();
        private readonly List<GenerationTask> _running = new();
        private readonly ITerrainWriter _writer;

        private readonly int _maxJobs = 2;
        private const int MaxApplyPerFrame = 1;
        
        private readonly HashSet<ChunkCoordinate> _loading =
            new();



        public ChunkManager(
            TerrainChunkFactory factory,
            IChunkGenerator generator,
            ITerrainWriter writer,
            Transform parent)
        {
            _factory = factory;
            _generator = generator;
            _writer = writer;
            _parent = parent;
        }



        public void Tick()
        {
            ScheduleJobs();

            CompleteJobs();
        }



        public void QueueLoad(
            ChunkCoordinate coordinate,
            NoiseSettings settings)
        {
            if (_chunks.ContainsKey(coordinate))
                return;


            if (_loading.Contains(coordinate))
                return;


            _loading.Add(coordinate);


            _queue.Enqueue(
                new ChunkGenerationRequest(
                    coordinate,
                    settings,
                    513));
        }



        private void ScheduleJobs()
        {
            while (
                _running.Count < _maxJobs &&
                _queue.Count > 0)
            {
                ChunkGenerationRequest request =
                    _queue.Dequeue();


                GenerationTask task =
                    _generator.Schedule(request);


                _running.Add(task);
            }
        }



        private void CompleteJobs()
        {
            int applied = 0;


            for(
                int i = _running.Count - 1;
                i >= 0;
                i--)
            {
                if(applied >= MaxApplyPerFrame)
                    break;


                GenerationTask task =
                    _running[i];


                if(!task.Handle.IsCompleted)
                    continue;


                task.Handle.Complete();


                ApplyResult(task.Result);


                task.Result.Heights.Dispose();


                _running.RemoveAt(i);


                applied++;
            }
        }



        private void ApplyResult(
            ChunkGenerationResult result)
        {
            Terrain terrain =
                _factory.Create(
                    result.Coordinate,
                    _parent);


            _writer.Write(
                terrain,
                result);
            
            terrain.Flush();

            terrain.terrainData.SyncHeightmap();


            _chunks.Add(
                result.Coordinate,
                terrain);


            _loading.Remove(
                result.Coordinate);
            
            Debug.Log(
                $"CREATE CHUNK {result.Coordinate}");
        }

        public void Unload(
            ChunkCoordinate coordinate)
        {
            if(_chunks.TryGetValue(
                coordinate,
                out Terrain terrain))
            {
                terrain.gameObject.SetActive(false);
            }
        }
    }
}