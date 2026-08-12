using System.Collections.Generic;
using UnityEngine;

namespace _Project.Features.Core.Infrastructure
{
    [CreateAssetMenu(menuName = "Project/Scene/Scene Database")]
    public class SceneDatabase : ScriptableObject
    {
        [SerializeField] private List<SceneConfig> sceneConfigs;

        public string GetScenePath(SceneType sceneType)
        {
            foreach (var scene in sceneConfigs)
            {
                if (scene.sceneType == sceneType)
                    return scene.scenePath;
            }

            return null;
        }
    }
}