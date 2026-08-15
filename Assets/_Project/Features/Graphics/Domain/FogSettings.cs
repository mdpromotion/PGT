using System;
using _Project.Features.Graphics.Infrastucture;
using _Project.Features.UI.Infrastructure;
using UnityEngine;
using VContainer.Unity;

namespace _Project.Features.Graphics.Domain
{
    public interface IFogSettings
    {
        float OriginalFogStartDistance { get; }
        float OriginalFogEndDistance { get; }
    }

    public class FogSettings : IFogSettings, IInitializable, IDisposable
    {
        
        private readonly GraphicsState _graphicsState;
        private readonly FogConfig _config;
        private readonly GraphicsQualityConfig _qualityConfig;
        
        public float OriginalFogStartDistance { get; private set; }
        public float OriginalFogEndDistance { get; private set; }

        public FogSettings(
            GraphicsState state,
            FogConfig fogConfig,
            GraphicsQualityConfig qualityConfig)
        {
            _graphicsState = state;
            _config = fogConfig;
            _qualityConfig = qualityConfig;

            SetNewFogDistance();
        }

        public void Initialize()
        {
            _graphicsState.GraphicsChanged += SetNewFogDistance;
        }

        private void SetNewFogDistance()
        {
            var viewType = _qualityConfig.GetViewDistanceEntry(_graphicsState.ViewDistance).graphicsType;

            OriginalFogEndDistance = _config.GetFogEndFromType(viewType);
            
            OriginalFogStartDistance = _config.GetFogStartromType(viewType);
        }

        public void Dispose()
        {
            _graphicsState.GraphicsChanged -= SetNewFogDistance;
        }
    }
}