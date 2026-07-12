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


        private readonly Queue<ChunkGenerationRequest> _queue = new();

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
            _queue.Enqueue(request);
        }



        public void Tick(
            Action<ChunkGenerationResult> apply)
        {
            Schedule();

            Complete(apply);
        }



        private void Schedule()
        {
            while(
                _running.Count < MaxJobs &&
                _queue.Count > 0)
            {
                var request =
                    _queue.Dequeue();


                _running.Add(
                    _generator.Schedule(request));
            }
        }



        private void Complete(
            Action<ChunkGenerationResult> apply)
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


                apply(task.Result);


                task.Result.Heights.Dispose();


                _running.RemoveAt(i);


                applied++;
            }
        }
    }
}