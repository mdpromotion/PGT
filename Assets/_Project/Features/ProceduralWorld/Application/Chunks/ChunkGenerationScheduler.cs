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

        private readonly Comparison<GenerationTask> _comparison;


        private bool _needsSort;


        private const int MaxJobs = 10;
        private const int MaxApplyPerFrame = 1;



        public ChunkGenerationScheduler(
            IChunkGenerator generator)
        {
            _generator = generator;

            _comparison =
                (a, b) =>
                    _comparer.Compare(
                        a.Result.Coordinate,
                        b.Result.Coordinate);
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

            Complete(
                apply,
                completed);
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
                
                _needsSort = true;
            }


            if(_needsSort)
            {
                _running.Sort(
                    _comparison);

                _needsSort = false;
            }
        }



        private void Complete(
            Action<ChunkGenerationResult> apply,
            Action<ChunkCoordinate> completed)
        {
            int applied = 0;


            for(int i = 0; i < _running.Count;)
            {
                if(applied >= MaxApplyPerFrame)
                    break;


                GenerationTask task =
                    _running[i];


                if(!task.Handle.IsCompleted)
                {
                    i++;
                    continue;
                }
                
                task.Handle.Complete();

                if(task.Cancelled)
                {
                    task.Result.Dispose();

                    completed(task.Result.Coordinate);
                    RemoveTask(i);
                    continue;
                }


                apply(task.Result);

                task.Result.Dispose();

                completed(task.Result.Coordinate);
                RemoveTask(i);
                applied++;
            }
        }



        private void RemoveTask(
            int index)
        {
            int last =
                _running.Count - 1;


            _running[index] =
                _running[last];


            _running.RemoveAt(last);
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
                if(task.Result.Coordinate.Equals(coordinate))
                {
                    task.Cancelled = true;
                    return;
                }
            }
        }



        public void CompleteAll()
        {
            foreach(GenerationTask task in _running)
            {
                task.Handle.Complete();
                task.Result.Dispose();
            }
            _running.Clear();


            _running.Clear();
        }
    }
}