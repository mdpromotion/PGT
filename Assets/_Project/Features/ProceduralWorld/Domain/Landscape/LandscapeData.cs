using Unity.Collections;
using _Project.Features.ProceduralWorld.Domain.Chunks;

namespace _Project.Features.ProceduralWorld.Domain.Landscape
{
    public sealed class LandscapeData
    {
        public ChunkCoordinate Coordinate { get; }

        public NativeArray<float> Heights { get; }

        public NativeArray<float> RiverMask { get; private set; }

        public NativeArray<float> WaterSurfaceHeight { get; private set; }

        public NativeArray<float> BankHeight { get; private set; }
        
        public NativeList<TreeInstanceRaw> Trees { get; private set; }

        public int Resolution { get; }

        public LandscapeData(
            ChunkCoordinate coordinate,
            NativeArray<float> heights,
            int resolution)
        {
            Coordinate = coordinate;

            Heights = heights;

            Resolution = resolution;
        }

        public void AttachRiverMask(
            NativeArray<float> riverMask)
        {
            RiverMask = riverMask;
        }

        public void AttachTrees(
            NativeList<TreeInstanceRaw> trees)
        {
            Trees = trees;
        }

        public void AttachWaterSurfaceHeight(
            NativeArray<float> waterSurfaceHeight)
        {
            WaterSurfaceHeight = waterSurfaceHeight;
        }

        public void AttachBankHeight(
            NativeArray<float> bankHeight)
        {
            BankHeight = bankHeight;
        }

        public void Dispose()
        {
            if (Heights.IsCreated)
            {
                Heights.Dispose();
            }

            if (RiverMask.IsCreated)
            {
                RiverMask.Dispose();
            }

            if (WaterSurfaceHeight.IsCreated)
            {
                WaterSurfaceHeight.Dispose();
            }

            if (BankHeight.IsCreated)
            {
                BankHeight.Dispose();
            }

            if (Trees.IsCreated)
            {
                Trees.Dispose();
            }
        }
    }
}