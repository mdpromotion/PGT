using System;
using System.Linq;
using _Project.Features.Core.Domain;
using _Project.Features.Graphics.Domain;
using UnityEngine;

namespace _Project.Features.UI.Infrastructure
{
    [CreateAssetMenu(menuName = "Settings/Graphics Quality Config")]
    public class GraphicsQualityConfig : ScriptableObject
    {
        [SerializeField] private ShadowDistanceEntry[] shadowDistanceEntries;
        [SerializeField] private ViewDistanceEntry[] viewDistanceEntries;

        public ShadowDistanceEntry GetShadowDistanceEntry(GraphicsType type) =>
            shadowDistanceEntries.First(m => m.graphicsType == type);

        public ViewDistanceEntry GetViewDistanceEntry(GraphicsType type) =>
            viewDistanceEntries.First(m => m.graphicsType == type);
        
        public ShadowDistanceEntry GetShadowDistanceEntry(float shadowDistance) =>
            shadowDistanceEntries.First(m => Mathf.Approximately(m.shadowDistance, shadowDistance));
        
        public ViewDistanceEntry GetViewDistanceEntry(int viewDistance) =>
            viewDistanceEntries.First(m => m.viewDistance == viewDistance);
    }

    [Serializable]
    public struct ShadowDistanceEntry
    {
        public GraphicsType graphicsType;
        public float shadowDistance;
    } 
    
    [Serializable]
    public struct ViewDistanceEntry
    {
        public GraphicsType graphicsType;
        public int viewDistance;
    }
}