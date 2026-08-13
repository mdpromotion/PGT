using System;
using _Project.Features.Core.Domain;
using UnityEngine;

namespace _Project.Features.Core.Infrastructure
{
    [CreateAssetMenu(menuName = "Settings/Graphics Preset")]
    public class GraphicsPreset  : ScriptableObject
    {
        public GraphicsType Category;
        public ShadowQualityEntry ShadowQuality;
        public AntiAliasingMode AntiAliasingMode;
        public GraphicsType ViewDistance;
    }
    
    [Serializable]
    public struct ShadowQualityEntry
    {
        public ShadowQuality quality;
        public GraphicsType shadowDistance;
    }
}
