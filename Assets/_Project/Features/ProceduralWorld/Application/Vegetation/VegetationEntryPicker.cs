using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Infrastructure.Vegetation;

namespace _Project.Features.ProceduralWorld.Application.Vegetation
{
    public sealed class VegetationEntryPicker
    {
        private readonly List<CandidateWeight> _buffer = new();

        public VegetationCatalogEntry Pick(
            IReadOnlyList<VegetationCatalogEntry> entries,
            float height01,
            float slopeDegrees,
            uint seed)
        {
            _buffer.Clear();

            float totalWeight = 0f;
            for (int i = 0; i < entries.Count; i++)
            {
                VegetationCatalogEntry entry = entries[i];
                if (!entry.Matches(height01, slopeDegrees))
                    continue;

                totalWeight += entry.Weight;
                _buffer.Add(new CandidateWeight(entry, totalWeight));
            }

            if (totalWeight <= 0f)
                return null;

            var rng = new Unity.Mathematics.Random(seed == 0 ? 1u : seed);
            float roll = rng.NextFloat(0f, totalWeight);

            for (int i = 0; i < _buffer.Count; i++)
            {
                if (roll <= _buffer[i].AccumulatedWeight)
                    return _buffer[i].Entry;
            }

            return null;
        }

        private readonly struct CandidateWeight
        {
            public readonly VegetationCatalogEntry Entry;
            public readonly float AccumulatedWeight;

            public CandidateWeight(VegetationCatalogEntry entry, float accumulatedWeight)
            {
                Entry = entry;
                AccumulatedWeight = accumulatedWeight;
            }
        }
    }
}