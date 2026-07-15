using System;
using _Project.Features.ProceduralWorld.Domain.Hydrology;
using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Application.Hydrology
{
    public sealed class HydrologyService :
        IHydrologyProvider,
        IDisposable
    {
        private readonly HydrologySettings _settings;

        private readonly HydrologyRegionCache _cache;

        private readonly HydrologyOrchestrator _orchestrator;



        public bool Enabled =>
            _settings.Enabled;



        public HydrologyService(
            HydrologySettings settings,
            HydrologyOrchestrator orchestrator)
        {
            _settings = settings;

            _orchestrator = orchestrator;

            _cache =
                new HydrologyRegionCache();
        }



        public HydrologyRegion GetOrCreateRegion(
            float2 worldPosition)
        {
            if (!_settings.Enabled)
                return null;



            HydrologyRegionCoordinate coordinate =
                HydrologyRegionCoordinate.FromWorldPosition(
                    worldPosition,
                    _settings.RegionSize);



            HydrologyRegion region =
                _cache.GetOrCreate(
                    coordinate);



            _orchestrator.ProcessRegion(
                region);



            return region;
        }



        public bool TryGetRegion(
            float2 worldPosition,
            out HydrologyRegion region)
        {
            region = null;


            if (!_settings.Enabled)
                return false;



            HydrologyRegionCoordinate coordinate =
                HydrologyRegionCoordinate.FromWorldPosition(
                    worldPosition,
                    _settings.RegionSize);



            return _cache.TryGet(
                coordinate,
                out region);
        }



        public void Tick()
        {
            foreach(HydrologyRegion region in _cache.Regions)
            {
                _orchestrator.Tick(
                    region);
            }
        }



        public void Dispose()
        {
            _cache.Dispose();
        }
    }
}