using _Project.Features.Core.Domain;
using UnityEngine;

namespace _Project.Features.Core.Infrastructure
{
    [CreateAssetMenu(menuName = "Settings/Graphics Preset")]
    public class GraphicsPreset  : ScriptableObject
    {
        public GraphicsType Category;
        public ShadowQualityMode ShadowQuality;
        public AntiAliasingMode AntiAliasingMode;
        public int ViewDistance;
    }
}
