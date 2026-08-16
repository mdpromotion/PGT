using System.Collections.Generic;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Domain.Vegetation
{
    public sealed class GameObjectInstancePool
    {
        private readonly Transform _root;
        private readonly Dictionary<GameObject, List<GameObject>> _poolByPrefab = new();
        private readonly Dictionary<GameObject, int> _activeCountByPrefab = new();

        public GameObjectInstancePool(Transform root)
        {
            _root = root;
        }

        public void BeginFrame()
        {
            var prefabs = new List<GameObject>(_activeCountByPrefab.Keys);
            for (int i = 0; i < prefabs.Count; i++)
                _activeCountByPrefab[prefabs[i]] = 0;
        }

        public GameObject Rent(GameObject prefab)
        {
            if (!_poolByPrefab.TryGetValue(prefab, out List<GameObject> pool))
            {
                pool = new List<GameObject>();
                _poolByPrefab[prefab] = pool;
                _activeCountByPrefab[prefab] = 0;
            }

            int activeCount = _activeCountByPrefab[prefab];

            GameObject instance;

            if (activeCount < pool.Count)
            {
                instance = pool[activeCount];
                
                if (instance == null)
                {
                    instance = Object.Instantiate(prefab, _root);
                    pool[activeCount] = instance;
                }
            }
            else
            {
                instance = Object.Instantiate(prefab, _root);
                pool.Add(instance);
            }

            _activeCountByPrefab[prefab] = activeCount + 1;
            instance.SetActive(true);

            return instance;
        }

        public void EndFrame()
        {
            foreach (KeyValuePair<GameObject, List<GameObject>> pair in _poolByPrefab)
            {
                List<GameObject> pool = pair.Value;
                int activeCount = _activeCountByPrefab[pair.Key];

                for (int i = activeCount; i < pool.Count; i++)
                {
                    if (pool[i] != null)
                        pool[i].SetActive(false);
                }
            }
        }
        
        public int GetPoolSize(GameObject prefab)
        {
            return _poolByPrefab.TryGetValue(prefab, out List<GameObject> pool) ? pool.Count : 0;
        }
        
        public int GetActiveCount(GameObject prefab)
        {
            return _activeCountByPrefab.TryGetValue(prefab, out int count) ? count : 0;
        }
    }
}