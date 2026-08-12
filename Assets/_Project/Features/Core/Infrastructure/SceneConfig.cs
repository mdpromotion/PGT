using System;
using UnityEngine;

namespace _Project.Features.Core.Infrastructure
{
    [Serializable]
    public enum SceneType
    {
        Menu,
        Game
    }
    
    [CreateAssetMenu(menuName = "Project/Scene/SceneConfig")]
    public class SceneConfig : ScriptableObject
    {
        public SceneType sceneType;
        public string scenePath;
    }
}