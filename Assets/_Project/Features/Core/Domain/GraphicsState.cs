using System;
using UnityEngine;

namespace _Project.Features.Core.Domain
{
    public readonly struct GraphicsData
    {
        public readonly GraphicsType Category;
        public readonly ShadowQualityMode ShadowQualityMode;
        public readonly AntiAliasingMode AntiAliasingMode;
        public readonly int ViewDistance;

        public GraphicsData(
            GraphicsType category, 
            ShadowQualityMode shadowQuality, 
            AntiAliasingMode antiAliasingMode,
            int viewDistance)
        {
            Category = category;
            ShadowQualityMode = shadowQuality;
            AntiAliasingMode = antiAliasingMode;
            ViewDistance = viewDistance;
        }
    }
    
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

    [Serializable]
    public struct ShadowQualityMode
    {
        [SerializeField] private ShadowQuality shadowQuality;
        [SerializeField] private float shadowDistance;
        
        public ShadowQuality ShadowQuality => shadowQuality;
        public float ShadowDistance => shadowDistance;

        public ShadowQualityMode(ShadowQuality shadowQuality, float shadowDistance)
        {
            this.shadowQuality = shadowQuality;
            this.shadowDistance = shadowDistance;
        }
    }
    
    public class GraphicsState
    {
        public GraphicsType Category { get; private set; }
        public ShadowQualityMode ShadowQualityMode { get; private set; }
        public AntiAliasingMode AntiAliasingMode { get; private set; }
        public int ViewDistance { get; private set; }
        
        public event Action GraphicsChanged;

        public GraphicsState(GraphicsData data)
        {
            SetGraphicsData(data);
        }

        public void SetGraphicsData(GraphicsData data)
        {
            Category = data.Category;
            ShadowQualityMode = data.ShadowQualityMode;
            AntiAliasingMode = data.AntiAliasingMode;
            ViewDistance = data.ViewDistance;
            
            GraphicsChanged?.Invoke();
        }
    }
}
