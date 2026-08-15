using _Project.Features.Graphics.Infrastucture;
using _Project.Features.UI.Infrastructure;

namespace _Project.Features.Graphics.Domain
{
    public interface IFogSettings
    {
        float OriginalFogStartDistance { get; }
        float OriginalFogEndDistance { get; }
    }

    public class FogSettings : IFogSettings
    {
        public float OriginalFogStartDistance { get; }
        public float OriginalFogEndDistance { get; }

        public FogSettings(
            GraphicsState state,
            FogConfig fogConfig,
            GraphicsQualityConfig qualityConfig)
        {
            var viewType =
                qualityConfig
                    .GetViewDistanceEntry(state.ViewDistance)
                    .graphicsType;

            OriginalFogEndDistance =
                fogConfig.GetFogEndFromType(viewType);
            
            OriginalFogStartDistance = fogConfig.GetFogStartromType(viewType);
        }
    }
}