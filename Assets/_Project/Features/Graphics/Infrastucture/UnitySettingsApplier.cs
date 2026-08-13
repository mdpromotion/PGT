using System;
using _Project.Features.Graphics.Domain;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VContainer.Unity;

namespace _Project.Features.Graphics.Infrastucture
{
    public class UnitySettingsApplier : IInitializable, IDisposable
    {
        private readonly GraphicsState _graphicsState;

        public UnitySettingsApplier(GraphicsState graphicsState)
        {
            _graphicsState = graphicsState;
        }
        
        public void Initialize()
        {
            _graphicsState.GraphicsChanged += ApplyGraphicsSettings;
            ApplyGraphicsSettings();
        }

        private void ApplyGraphicsSettings()
        {
            ApplyShadowSettings();
            ApplyAntiAliasing();
            ApplyWindowMode();
            ApplyVSync();
        }

        private void ApplyShadowSettings()
        {
            if (GraphicsSettings.currentRenderPipeline is not UniversalRenderPipelineAsset urpAsset) return;

            urpAsset.shadowDistance = _graphicsState.ShadowQualityMode.ShadowDistance;

            QualitySettings.shadows = _graphicsState.ShadowQualityMode.ShadowQuality;
        }

        private void ApplyAntiAliasing()
        {
            if (GraphicsSettings.currentRenderPipeline is not UniversalRenderPipelineAsset urpAsset) return;
            
            int msaaSampleCount = _graphicsState.AntiAliasingMode switch
            {
                AntiAliasingMode.Msaa4 => 4,
                _ => 1
            };

            if (urpAsset.msaaSampleCount == msaaSampleCount)
                return;
            
            urpAsset.msaaSampleCount = msaaSampleCount;
        }
        
        private void ApplyWindowMode()
        {
            FullScreenMode screenMode = _graphicsState.WindowMode switch
            {
                WindowMode.Windowed => FullScreenMode.Windowed,
                WindowMode.Borderless => FullScreenMode.FullScreenWindow,
                WindowMode.Fullscreen => FullScreenMode.ExclusiveFullScreen,
                _ => FullScreenMode.FullScreenWindow
            };

            if (Screen.fullScreenMode == screenMode)
                return;
            
            Screen.fullScreenMode = screenMode;
        }

        private void ApplyVSync()
        {
            int vSyncCount = _graphicsState.VSync ? 1 : 0;
            
            if (QualitySettings.vSyncCount == vSyncCount)
                return;

            QualitySettings.vSyncCount = vSyncCount;
        }

        public void Dispose()
        {
            _graphicsState.GraphicsChanged -= ApplyGraphicsSettings;
        }
    }
}
