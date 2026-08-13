using System;
using _Project.Features.Graphics.Infrastucture;
using UnityEngine;

namespace _Project.Features.Graphics.Domain
{
    public enum GraphicsType
    {
        Low = 0, 
        Medium = 1,
        High = 2
    }
    
    public enum AntiAliasingMode
    {
        None,
        Fxaa,
        Taa,
        Msaa4,
    }

    public enum WindowMode
    {
        Windowed,
        Borderless,
        Fullscreen,
    }
    
    public readonly struct GraphicsData
    {
        public readonly GraphicsType QualityMode;
        public readonly ShadowQualityMode ShadowQualityMode;
        public readonly AntiAliasingMode AntiAliasingMode;
        public readonly WindowMode WindowMode;
        public readonly bool VSync;
        public readonly int ViewDistance;

        public GraphicsData(
            GraphicsType category, 
            ShadowQualityMode shadowQuality, 
            AntiAliasingMode antiAliasingMode,
            WindowMode windowMode,
            bool vSync,
            int viewDistance)
        {
            QualityMode = category;
            ShadowQualityMode = shadowQuality;
            AntiAliasingMode = antiAliasingMode;
            WindowMode = windowMode;
            VSync = vSync;
            ViewDistance = viewDistance;
        }
    }

    [Serializable]
    public readonly struct ShadowQualityMode
    {
        public readonly ShadowQuality ShadowQuality;
        public readonly float ShadowDistance;

        public ShadowQualityMode(ShadowQuality shadowQuality, float shadowDistance)
        {
            ShadowQuality = shadowQuality;
            ShadowDistance = shadowDistance;
        }
    }
    
    public class GraphicsState
    {
        private readonly IGraphicsSettingsRepository _repository;
        
        public GraphicsType QualityMode { get; private set; }
        public ShadowQualityMode ShadowQualityMode { get; private set; }
        public AntiAliasingMode AntiAliasingMode { get; private set; }
        public WindowMode WindowMode { get; private set; }
        public bool VSync { get; private set; }
        public int ViewDistance { get; private set; }
        
        public event Action GraphicsChanged;

        public GraphicsState(IGraphicsSettingsRepository repository)
        {
            _repository = repository;
            SetGraphicsData(_repository.Load(), persist: false);
        }

        public void SetGraphicsData(GraphicsData data, bool persist = true)
        {
            QualityMode = data.QualityMode;
            ShadowQualityMode = data.ShadowQualityMode;
            AntiAliasingMode = data.AntiAliasingMode;
            WindowMode = data.WindowMode;
            VSync = data.VSync;
            ViewDistance = data.ViewDistance;
            
            if (persist)
                _repository.Save(data);
            
            GraphicsChanged?.Invoke();
        }
    }
}
