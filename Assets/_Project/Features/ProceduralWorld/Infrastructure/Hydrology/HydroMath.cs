using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    [BurstCompile]
    public static class HydroMath
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Smoothstep(float t) => t * t * (3f - 2f * t);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SmoothstepSaturated(float t) => Smoothstep(math.saturate(t));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RiverWidth(float strength, float riverWidthScale, float maxStrength)
        {
            float clampedStrength = math.min(strength, maxStrength);
            return math.max(riverWidthScale * clampedStrength, 0.001f);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RiverWidth(float strength, float riverWidthScale, float maxStrength, out float clampedStrength)
        {
            clampedStrength = math.min(strength, maxStrength);
            return math.max(riverWidthScale * clampedStrength, 0.001f);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 WorldToCell(float2 world, float2 origin, float cellSize)
        {
            int cx = (int)math.floor((world.x - origin.x) / cellSize);
            int cz = (int)math.floor((world.y - origin.y) / cellSize);
            return new int2(cx, cz);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WorldToCellIndexClamped(
            float2 world, float2 origin, float cellSize, int gridWidth, int gridHeight)
        {
            int2 cell = WorldToCell(world, origin, cellSize);

            int cx = math.clamp(cell.x, 0, gridWidth - 1);
            int cz = math.clamp(cell.y, 0, gridHeight - 1);

            return cz * gridWidth + cx;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetCellIndex(int cx, int cz, int gridWidth, int gridHeight, out int cellIndex)
        {
            if (cx < 0 || cx >= gridWidth || cz < 0 || cz >= gridHeight)
            {
                cellIndex = -1;
                return false;
            }

            cellIndex = cz * gridWidth + cx;
            return true;
        }
    }
}