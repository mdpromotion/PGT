using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Settings;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    public struct HydrologyCarveJob : IJobParallelFor
    {
        private NativeArray<float> _heights;
        private NativeArray<float> _riverMask;

        private readonly int _resolution;

        private readonly float _stepX;
        private readonly float _stepZ;

        private readonly int2 _chunk;
        private readonly float _chunkSizeX;
        private readonly float _chunkSizeZ;

        [ReadOnly]
        private NativeArray<RiverSegment> _riverSegments;

        [ReadOnly]
        private NativeArray<int> _cellStart;

        [ReadOnly]
        private NativeArray<int> _cellCount;

        [ReadOnly]
        private NativeArray<int> _pointIndices;

        private readonly float2 _hashOrigin;
        private readonly float _cellSize;
        private readonly int _gridWidth;
        private readonly int _gridHeight;

        private readonly HydrologySettings _settings;

        public HydrologyCarveJob(
            NativeArray<float> heights,
            NativeArray<float> riverMask,
            int resolution,
            float chunkSizeX,
            float chunkSizeZ,
            int2 chunk,
            NativeArray<RiverSegment> riverSegments,
            SpatialHashData hash,
            HydrologySettings settings)
        {
            _heights = heights;
            _riverMask = riverMask;
            _resolution = resolution;
            _chunkSizeX = chunkSizeX;
            _chunkSizeZ = chunkSizeZ;
            _chunk = chunk;
            _riverSegments = riverSegments;
            _settings = settings;

            _stepX = chunkSizeX / (resolution - 1);
            _stepZ = chunkSizeZ / (resolution - 1);

            _cellStart = hash.CellStart;
            _cellCount = hash.CellCount;
            _pointIndices = hash.PointIndices.AsDeferredJobArray();
            _hashOrigin = hash.Origin;
            _cellSize = hash.CellSize;
            _gridWidth = hash.GridWidth;
            _gridHeight = hash.GridHeight;
        }

        public void Execute(int index)
        {
            int x = index % _resolution;
            int z = index / _resolution;

            float worldX = _chunk.x * _chunkSizeX + x * _stepX;
            float worldZ = _chunk.y * _chunkSizeZ + z * _stepZ;

            float2 world = new float2(worldX, worldZ);

            float combinedMask = 0f;
            float weightedHeight = 0f;
            float totalWeight = 0f;
            float strongestDepth = 0f;

            float maxRiverStrength = math.max(_settings.MaxRiverStrength, 0.0001f);
            float invMaxRiverStrength = 1f / maxRiverStrength;

            float minCarveDepth = _settings.CarveDepth * _settings.InitialCarveDepthFactor;
            float maxCarveDepth = _settings.CarveDepth;

            float riverWidthScale = _settings.RiverWidth;

            int centerCx = (int)math.floor((world.x - _hashOrigin.x) / _cellSize);
            int centerCz = (int)math.floor((world.y - _hashOrigin.y) / _cellSize);

            for (int dz = -1; dz <= 1; dz++)
            {
                int cz = centerCz + dz;

                if (cz < 0 || cz >= _gridHeight)
                    continue;

                for (int dx = -1; dx <= 1; dx++)
                {
                    int cx = centerCx + dx;

                    if (cx < 0 || cx >= _gridWidth)
                        continue;

                    int cell = cz * _gridWidth + cx;

                    int start = _cellStart[cell];
                    int cellPointCount = _cellCount[cell];

                    for (int k = 0; k < cellPointCount; k++)
                    {
                        int i = _pointIndices[start + k];

                        RiverSegment segment = _riverSegments[i];

                        float2 a = segment.A;
                        float2 b = segment.B;
                        float2 ab = b - a;

                        float abLenSq = math.max(math.lengthsq(ab), 0.0001f);
                        float t = math.saturate(math.dot(world - a, ab) / abLenSq);

                        float2 closest = a + ab * t;
                        float2 delta = world - closest;

                        float distanceSq = math.lengthsq(delta);
                        float strength = math.lerp(segment.StrengthA, segment.StrengthB, t);
                        float riverHeight = math.lerp(segment.HeightA, segment.HeightB, t);

                        float clampedStrength = math.min(strength, maxRiverStrength);
                        float width = math.max(riverWidthScale * clampedStrength, 0.001f);
                        float widthSq = width * width;

                        if (distanceSq >= widthSq)
                            continue;

                        float distance = math.sqrt(distanceSq);
                        float normalized = math.saturate(1f - distance / width);

                        float influence = normalized * normalized * (3f - 2f * normalized);

                        float depth = math.lerp(
                            minCarveDepth,
                            maxCarveDepth,
                            clampedStrength * invMaxRiverStrength);

                        float riverBottom = riverHeight - depth;

                        float localDepth = depth * influence;
                        strongestDepth = math.max(strongestDepth, localDepth);

                        float blendWeight = influence * influence;
                        weightedHeight += riverBottom * blendWeight;
                        totalWeight += blendWeight;

                        combinedMask = 1f - (1f - combinedMask) * (1f - influence);
                    }
                }
            }

            _riverMask[index] = combinedMask;

            if (combinedMask <= 0f || totalWeight <= 0f)
                return;

            float blendedHeight = weightedHeight / totalWeight;

            float targetRiverHeight = math.min(
                blendedHeight,
                _heights[index] - strongestDepth);

            float targetHeight = math.min(
                _heights[index],
                math.lerp(
                    _heights[index],
                    targetRiverHeight,
                    combinedMask));

            _heights[index] = targetHeight;
        }
    }
}