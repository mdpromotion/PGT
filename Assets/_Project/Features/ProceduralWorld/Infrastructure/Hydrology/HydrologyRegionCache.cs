using System;
using System.Collections.Generic;
using Unity.Jobs;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public sealed class HydrologyRegionCache : IDisposable
    {
        private readonly struct CacheEntry
        {
            public readonly HydrologyRegionData Data;
            public readonly JobHandle Handle;

            public CacheEntry(HydrologyRegionData data, JobHandle handle)
            {
                Data = data;
                Handle = handle;
            }
        }

        private readonly HydrologyRegionBuilder _builder;

        private readonly Dictionary<RegionCoordinate, CacheEntry> _regions = new();

        private readonly List<RegionCoordinate> _evictionBuffer = new();

        public HydrologyRegionCache(HydrologyRegionBuilder builder)
        {
            _builder = builder;
        }
        
        public RegionFetchResult Get(RegionCoordinate region)
        {
            if (_regions.TryGetValue(region, out CacheEntry entry))
            {
                return new RegionFetchResult(entry.Data, entry.Handle);
            }

            RegionFetchResult result = _builder.Schedule(region);

            _regions.Add(
                region,
                new CacheEntry(result.Data, result.Handle));

            return result;
        }



        public void EvictOutside(
            RegionCoordinate center,
            int keepRadius)
        {
            _evictionBuffer.Clear();

            foreach (KeyValuePair<RegionCoordinate, CacheEntry> pair in _regions)
            {
                int dx = pair.Key.X - center.X;
                int dy = pair.Key.Y - center.Y;

                int chebyshev = System.Math.Max(
                    System.Math.Abs(dx),
                    System.Math.Abs(dy));

                if (chebyshev > keepRadius)
                {
                    _evictionBuffer.Add(pair.Key);
                }
            }

            foreach (RegionCoordinate key in _evictionBuffer)
            {
                CacheEntry entry = _regions[key];

                entry.Handle.Complete();
                entry.Data.Dispose();

                _regions.Remove(key);
            }
        }



        public void Dispose()
        {
            foreach (CacheEntry entry in _regions.Values)
            {
                entry.Handle.Complete();
                entry.Data.Dispose();
            }

            _regions.Clear();
        }
    }
}