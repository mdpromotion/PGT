using System.Linq;
using _Project.Features.Graphics.Domain;
using _Project.Features.UI.Infrastructure;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Features.Graphics.Infrastucture
{
    [CreateAssetMenu(menuName = "Settings/FogConfig")]
    public class FogConfig : ScriptableObject
    {
        [SerializeField] private FogConfigEntry[] entries;
        
        public float GetFogStartromType(GraphicsType type)
            => entries.FirstOrDefault(x => x.type == type).fogStart;
        public float GetFogEndFromType(GraphicsType type)
            => entries.FirstOrDefault(x => x.type == type).fogEnd;
    }
    [System.Serializable]
    public struct FogConfigEntry
    {
        public GraphicsType type;
        public float fogStart;
        public float fogEnd;
    }
}
