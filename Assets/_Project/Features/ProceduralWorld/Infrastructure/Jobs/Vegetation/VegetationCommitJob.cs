using _Project.Features.ProceduralWorld.Domain.Vegetation;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Vegetation
{
    public struct VegetationCommitJob : IJob
    {
        [ReadOnly] private readonly int _resolution;
        [ReadOnly] private readonly float _occupancyRadius;

        private NativeArray<byte> _occupancy;

        [ReadOnly] private NativeList<VegetationInstanceData> _candidates;

        [WriteOnly] private NativeList<VegetationInstanceData> _accepted;

        public VegetationCommitJob(
            int resolution,
            float occupancyRadius,
            NativeArray<byte> occupancy,
            NativeList<VegetationInstanceData> candidates,
            NativeList<VegetationInstanceData> accepted)
        {
            _resolution = resolution;
            _occupancyRadius = occupancyRadius;
            _occupancy = occupancy;
            _candidates = candidates;
            _accepted = accepted;
        }

        public void Execute()
        {
            for (int i = 0; i < _candidates.Length; i++)
            {
                VegetationInstanceData candidate = _candidates[i];

                int cellX = (int)candidate.Position.x;
                int cellZ = (int)candidate.Position.z;

                if (IsAreaFree(cellX, cellZ))
                {
                    MarkArea(cellX, cellZ);
                    _accepted.Add(candidate);
                }
            }
        }

        private bool IsAreaFree(int centerX, int centerZ)
        {
            int minX = math.max((int)math.floor(centerX - _occupancyRadius), 0);
            int maxX = math.min((int)math.ceil(centerX + _occupancyRadius), _resolution - 1);
            int minZ = math.max((int)math.floor(centerZ - _occupancyRadius), 0);
            int maxZ = math.min((int)math.ceil(centerZ + _occupancyRadius), _resolution - 1);

            for (int z = minZ; z <= maxZ; z++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    if (_occupancy[z * _resolution + x] != 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void MarkArea(int centerX, int centerZ)
        {
            int minX = math.max((int)math.floor(centerX - _occupancyRadius), 0);
            int maxX = math.min((int)math.ceil(centerX + _occupancyRadius), _resolution - 1);
            int minZ = math.max((int)math.floor(centerZ - _occupancyRadius), 0);
            int maxZ = math.min((int)math.ceil(centerZ + _occupancyRadius), _resolution - 1);

            for (int z = minZ; z <= maxZ; z++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    _occupancy[z * _resolution + x] = 1;
                }
            }
        }
    }
}