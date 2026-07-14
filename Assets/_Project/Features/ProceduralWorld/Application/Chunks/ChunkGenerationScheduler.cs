using System;
using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Application.Interfaces;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;

namespace _Project.Features.ProceduralWorld.Application.Chunks
{
    public class ChunkGenerationScheduler
    {
        private readonly IChunkGenerator _generator;


        private readonly LinkedList<ChunkGenerationRequest> _queue = new();

        private readonly Dictionary<
            ChunkCoordinate,
            LinkedListNode<ChunkGenerationRequest>> _queued = new();

        private readonly List<GenerationTask> _running = new();


        private readonly ChunkCoordinateDistanceComparer _comparer = new();


        private const int MaxJobs = 10;
        private const int MaxApplyPerFrame = 1;



        public ChunkGenerationScheduler(
            IChunkGenerator generator)
        {
            _generator = generator;
        }



        public void Enqueue(
            ChunkGenerationRequest request)
        {
            if (_queued.ContainsKey(request.Coordinate))
                return;

            LinkedListNode<ChunkGenerationRequest> node =
                _queue.AddLast(request);

            _queued.Add(
                request.Coordinate,
                node);
        }



        public void Tick(
            Action<ChunkGenerationResult> apply,
            Action<ChunkCoordinate> completed)
        {
            Schedule();

            Complete(apply, completed);
        }



        private void Schedule()
        {
            while (_running.Count < MaxJobs &&
                   _queue.First != null)
            {
                LinkedListNode<ChunkGenerationRequest> node =
                    _queue.First;

                _queue.RemoveFirst();

                _queued.Remove(
                    node.Value.Coordinate);

                _running.Add(
                    _generator.Schedule(
                        node.Value));
            }
        }



        private void Complete(
            Action<ChunkGenerationResult> apply,
            Action<ChunkCoordinate> completed)
        {
            int applied = 0;


            _running.Sort(
                (a,b)=>
                    _comparer.Compare(
                        a.Result.Coordinate,
                        b.Result.Coordinate));


            for(int i = 0; i < _running.Count;)
            {
                if(applied >= MaxApplyPerFrame)
                    break;


                var task = _running[i];


                if(!task.Handle.IsCompleted)
                {
                    i++;
                    continue;
                }


                task.Handle.Complete();

                if(task.Cancelled)
                {
                    task.Result.Heights.Dispose();

                    completed(task.Result.Coordinate);

                    _running.RemoveAt(i);

                    continue;
                }

                apply(task.Result);

                task.Result.Heights.Dispose();

                completed(task.Result.Coordinate);

                _running.RemoveAt(i);

                applied++;
            }
        }
        
        public void Cancel(
            ChunkCoordinate coordinate)
        {
            if (_queued.TryGetValue(
                    coordinate,
                    out LinkedListNode<ChunkGenerationRequest> node))
            {
                _queue.Remove(node);

                _queued.Remove(coordinate);

                return;
            }

            foreach (GenerationTask task in _running)
            {
                if (task.Result.Coordinate.Equals(coordinate))
                {
                    task.Cancelled = true;
                    return;
                }
            }
        }   
        public void CompleteAll()
        {
            foreach (GenerationTask task in _running)
            {
                task.Handle.Complete();

                task.Result.Heights.Dispose();
            }

            _running.Clear();
        }
    }
}