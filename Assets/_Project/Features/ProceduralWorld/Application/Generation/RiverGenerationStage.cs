using _Project.Features.ProceduralWorld.Domain.Chunks;
using Unity.Collections;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;


namespace _Project.Features.ProceduralWorld.Application.Chunks.Modifiers
{
    public sealed class RiverGenerationStage :
        IChunkGenerationStage
    {
        public void Execute(
            ChunkGenerationState state)
        {
            NativeArray<float> heights =
                state.Landscape.Heights;



            int size =
                state.Landscape.Resolution;



            Random random =
                new Random(
                    (uint)(
                        state.Context.Coordinate.X *
                        73856093 ^
                        state.Context.Coordinate.Y *
                        19349663));



            int riverCount = 2;



            for(int i = 0; i < riverCount; i++)
            {
                int start =
                    random.NextInt(
                        0,
                        heights.Length);



                CarveRiver(
                    heights,
                    size,
                    start,
                    ref random);
            }
        }



        private void CarveRiver(
            NativeArray<float> heights,
            int size,
            int start,
            ref Random random)
        {
            int index = start;



            for(int i = 0; i < size; i++)
            {
                if(index < 0 ||
                   index >= heights.Length)
                {
                    break;
                }



                heights[index] -= 0.0025f;



                int x =
                    index % size;


                int y =
                    index / size;



                x += random.NextInt(-1,2);
                y += random.NextInt(-1,2);



                x =
                    math.clamp(
                        x,
                        0,
                        size-1);


                y =
                    math.clamp(
                        y,
                        0,
                        size-1);



                index =
                    y * size + x;
            }
        }
    }
}