using System;
using _Project.Features.Graphics.Domain;
using UnityEngine;

namespace _Project.Features.Graphics.Infrastucture
{
    [CreateAssetMenu(menuName = "Settings/Graphics Preset")]
    public class GraphicsPreset : ScriptableObject
    {
        public GraphicsType Category;
        public ShadowQualityEntry ShadowQuality;
        public AntiAliasingMode AntiAliasingMode;
        public WindowMode WindowMode;
        public bool VSync;
        public GraphicsType ViewDistance;
    }
    
    [Serializable]
    public struct ShadowQualityEntry
    {
        public ShadowQuality quality;
        public GraphicsType shadowDistance;
    }
}
