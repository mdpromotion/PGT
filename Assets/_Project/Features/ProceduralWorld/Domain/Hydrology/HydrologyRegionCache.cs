using System;
using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;
using Unity.Collections;

namespace _Project.Features.ProceduralWorld.Domain.Hydrology
{
    public sealed class HydrologyRegionCache : IDisposable
    {
        private readonly Dictionary<
            HydrologyRegionCoordinate,
            HydrologyRegion> _regions = new();


        private readonly Allocator _allocator;



        public IEnumerable<HydrologyRegion> Regions =>
            _regions.Values;



        public HydrologyRegionCache(
            Allocator allocator = Allocator.Persistent)
        {
            _allocator = allocator;
        }



        public HydrologyRegion GetOrCreate(
            HydrologyRegionCoordinate coordinate)
        {
            if (_regions.TryGetValue(
                    coordinate,
                    out HydrologyRegion region))
            {
                return region;
            }


            region =
                new HydrologyRegion(
                    coordinate,
                    _allocator);


            _regions.Add(
                coordinate,
                region);


            return region;
        }



        public bool TryGet(
            HydrologyRegionCoordinate coordinate,
            out HydrologyRegion region)
        {
            return _regions.TryGetValue(
                coordinate,
                out region);
        }



        public void Remove(
            HydrologyRegionCoordinate coordinate)
        {
            if (!_regions.TryGetValue(
                    coordinate,
                    out HydrologyRegion region))
            {
                return;
            }


            region.Dispose();


            _regions.Remove(
                coordinate);
        }



        public void Dispose()
        {
            foreach (HydrologyRegion region in _regions.Values)
            {
                region.Dispose();
            }


            _regions.Clear();
        }
    }
}