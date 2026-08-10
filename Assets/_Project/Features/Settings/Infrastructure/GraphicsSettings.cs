using UnityEngine;

namespace _Project.Features.Settings.Infrastructure
{
    public enum GraphicsType
    {
        Low, 
        Medium,
        High
    }

    [CreateAssetMenu(menuName = "Settings/Graphics Preset")]
    public class GraphicsSettings : ScriptableObject
    {
        public GraphicsType category;
        public int shadowQuality;
        public int viewDistance;
        public int antiAliasing;
    }
}
