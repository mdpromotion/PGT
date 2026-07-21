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
        private NativeArray<float> _waterSurfaceHeight;
        private NativeArray<float> _bankHeight;

        private readonly int _resolution;

        private readonly float _stepX;
        private readonly float _stepZ;

        private readonly int2 _chunk;
        private readonly float _chunkSizeX;
        private readonly float _chunkSizeZ;

        [ReadOnly] private NativeArray<RiverSegment> _riverSegments;

        [ReadOnly] private NativeArray<int> _cellStart;

        [ReadOnly] private NativeArray<int> _cellCount;

        [ReadOnly] private NativeArray<int> _pointIndices;

        private readonly float2 _hashOrigin;
        private readonly float _cellSize;
        private readonly int _gridWidth;
        private readonly int _gridHeight;

        private readonly HydrologySettings _settings;
        
        private readonly TerrainNoiseSettings _noiseSettings;

        [ReadOnly] private NativeArray<float2> _noiseOffsets;


        public HydrologyCarveJob(
            NativeArray<float> heights,
            NativeArray<float> riverMask,
            NativeArray<float> waterSurfaceHeight,
            NativeArray<float> bankHeight,
            int resolution,
            float chunkSizeX,
            float chunkSizeZ,
            int2 chunk,
            NativeArray<RiverSegment> riverSegments,
            SpatialHashData hash,
            HydrologySettings settings,
            TerrainNoiseSettings noiseSettings,
            NativeArray<float2> noiseOffsets)
        {
            _heights = heights;
            _riverMask = riverMask;
            _waterSurfaceHeight = waterSurfaceHeight;
            _bankHeight = bankHeight;
            _resolution = resolution;
            _chunkSizeX = chunkSizeX;
            _chunkSizeZ = chunkSizeZ;
            _chunk = chunk;
            _riverSegments = riverSegments;
            _settings = settings;
            _noiseSettings = noiseSettings;
            _noiseOffsets = noiseOffsets;

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

            float originalHeight = _heights[index];

            float combinedMask = 0f;
            float weightedHeight = 0f;
            float weightedSurfaceHeight = 0f;
            float totalWeight = 0f;
            float strongestDepth = 0f;

            float maxRiverStrength = math.max(_settings.MaxRiverStrength, 0.0001f);
            float invMaxRiverStrength = 1f / maxRiverStrength;

            float minCarveDepth = _settings.CarveDepth * _settings.InitialCarveDepthFactor;
            float maxCarveDepth = _settings.CarveDepth;

            float riverWidthScale = _settings.RiverWidth;

            float overlapMultiplier = 1f + math.max(_settings.EdgeOverlapFactor, 0f);

            int2 centerCell = HydroMath.WorldToCell(world, _hashOrigin, _cellSize);

            for (int dz = -1; dz <= 1; dz++)
            {
                int cz = centerCell.y + dz;

                for (int dx = -1; dx <= 1; dx++)
                {
                    int cx = centerCell.x + dx;

                    if (!HydroMath.TryGetCellIndex(cx, cz, _gridWidth, _gridHeight, out int cell))
                        continue;

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

                        float riverProfileHeight = math.lerp(segment.HeightA, segment.HeightB, t);

                        float nominalWidth = HydroMath.RiverWidth(
                            strength,
                            riverWidthScale,
                            maxRiverStrength,
                            out float clampedStrength);

                        float width = nominalWidth * overlapMultiplier;
                        float widthSq = width * width;

                        if (distanceSq >= widthSq)
                            continue;

                        float distance = math.sqrt(distanceSq);
                        float normalized = math.saturate(1f - distance / width);

                        float influence = HydroMath.Smoothstep(normalized);

                        float depth = math.lerp(
                            minCarveDepth,
                            maxCarveDepth,
                            clampedStrength * invMaxRiverStrength);

                        float2 tangent = math.normalize(ab);
                        float2 perpendicular = new float2(-tangent.y, tangent.x);
                        
                        float2 toPoint = world - closest;
                        float signedOffset = math.dot(toPoint, perpendicular);

                        float sampleDist = width * 0.5f;

                        float heightLeft = HeightSampler.Sample(
                            closest - perpendicular * sampleDist,
                            _noiseSettings,
                            _noiseOffsets);

                        float heightRight = HeightSampler.Sample(
                            closest + perpendicular * sampleDist,
                            _noiseSettings,
                            _noiseOffsets);
                        
                        float t01 = math.clamp(
                            signedOffset / sampleDist,
                            -1f,
                            1f);
                        
                        float bankHeight = math.lerp(
                            heightLeft,
                            heightRight,
                            (t01 + 1f) * 0.5f);
                        
                        float centerBankHeight = math.lerp(
                            heightLeft,
                            heightRight,
                            0.5f);
                        
                        float bankTilt = bankHeight - centerBankHeight;

                        float waterHeight = riverProfileHeight + bankTilt;
                            

                        float riverBottom = waterHeight - depth;

                        float localDepth = depth * influence;

                        strongestDepth = math.max(
                            strongestDepth,
                            localDepth);

                        float blendWeight = influence * influence;

                        weightedHeight += riverBottom * blendWeight;

                        weightedSurfaceHeight += waterHeight * blendWeight;

                        totalWeight += blendWeight;

                        combinedMask = 1f - 
                            (1f - combinedMask) * 
                            (1f - influence);
                    }
                }
            }

            _riverMask[index] = combinedMask;

            if (combinedMask <= 0f || totalWeight <= 0f)
            {
                _waterSurfaceHeight[index] = originalHeight;
                return;
            }

            float blendedHeight = weightedHeight / totalWeight;
            float blendedSurfaceHeight = weightedSurfaceHeight / totalWeight;

            _waterSurfaceHeight[index] = blendedSurfaceHeight;

            float targetRiverHeight = math.min(blendedHeight, originalHeight - strongestDepth);
            float targetHeight = math.min(originalHeight, math.lerp(originalHeight, targetRiverHeight, combinedMask));

            _heights[index] = targetHeight;
            _bankHeight[index] = originalHeight;
        }
    }
}