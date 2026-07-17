using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Settings;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs
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
        private NativeArray<float2Point> _riverPoints;

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
            NativeArray<float2Point> riverPoints,
            SpatialHashData hash,
            HydrologySettings settings)
        {
            _heights = heights;
            _riverMask = riverMask;
            _resolution = resolution;
            _chunkSizeX = chunkSizeX;
            _chunkSizeZ = chunkSizeZ;
            _chunk = chunk;
            _riverPoints = riverPoints;
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

            float closestRiverDist = float.MaxValue;
            float closestStrength = 0f;
            float closestRiverHeight = 0f;

            float closestLakeDist = float.MaxValue;
            float closestLakeRadius = 0f;
            float closestLakeHeight = 0f;

            int centerCx = (int)math.floor((world.x - _hashOrigin.x) / _cellSize);
            int centerCz = (int)math.floor((world.y - _hashOrigin.y) / _cellSize);

            int count = _riverPoints.Length;

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

                        float2Point point = _riverPoints[i];
                        float2 a = new float2(point.X, point.Z);

                        if (point.Kind == HydrologyPointKind.Lake)
                        {
                            float lakeDist = math.distance(world, a);

                            if (lakeDist < closestLakeDist)
                            {
                                closestLakeDist = lakeDist;
                                closestLakeRadius = math.max(point.Strength, 0.001f);
                                closestLakeHeight = point.Height;
                            }

                            continue;
                        }

                        bool hasNext =
                            i + 1 < count &&
                            _riverPoints[i + 1].SegmentId == point.SegmentId &&
                            _riverPoints[i + 1].Kind == HydrologyPointKind.River;

                        float distance;
                        float strength;
                        float riverHeight;

                        if (hasNext)
                        {
                            float2Point next = _riverPoints[i + 1];
                            float2 b = new float2(next.X, next.Z);
                            float2 ab = b - a;

                            float length = math.max(math.lengthsq(ab), 0.0001f);
                            float t = math.saturate(math.dot(world - a, ab) / length);
                            float2 closest = a + ab * t;

                            distance = math.distance(world, closest);
                            strength = math.lerp(point.Strength, next.Strength, t);
                            riverHeight = math.lerp(point.Height, next.Height, t);
                        }
                        else
                        {
                            distance = math.distance(world, a);
                            strength = point.Strength;
                            riverHeight = point.Height;
                        }

                        if (distance < closestRiverDist)
                        {
                            closestRiverDist = distance;
                            closestStrength = strength;
                            closestRiverHeight = riverHeight;
                        }
                    }
                }
            }

            float lakeMask = 0f;

            if (closestLakeDist < float.MaxValue)
            {
                float t = math.saturate(1f - closestLakeDist / closestLakeRadius);
                lakeMask = t * t * (3f - 2f * t);
            }

            float riverShape = 0f;

            if (closestRiverDist < float.MaxValue)
            {
                float width = math.max(_settings.RiverWidth * closestStrength, 0.001f);
                float normalized = math.saturate(1f - closestRiverDist / width);
                riverShape = math.pow(normalized, 2.2f);
            }

            float mask = math.max(riverShape, lakeMask);
            _riverMask[index] = mask;

            if (mask <= 0f)
            {
                return;
            }

            float targetHeight = _heights[index];

            if (riverShape > 0f)
            {
                float depth = math.lerp(
                    _settings.CarveDepth * 0.35f,
                    _settings.CarveDepth,
                    math.saturate(closestStrength / 3f));

                float riverBottom = closestRiverHeight - depth;
                targetHeight = math.min(targetHeight, math.lerp(_heights[index], riverBottom, riverShape));
            }

            if (lakeMask > 0f)
            {
                float lakeBottom = closestLakeHeight - _settings.LakeDepth;
                float lakeTarget = math.lerp(_heights[index], lakeBottom, lakeMask);
                targetHeight = math.min(targetHeight, lakeTarget);
            }

            _heights[index] = targetHeight;
        }
    }
}