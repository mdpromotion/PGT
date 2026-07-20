using System.Collections.Generic;
using UnityEngine;
using _Project.Features.Sound.Domain;

namespace _Project.Features.Sound.Infrastructure
{
    [CreateAssetMenu(menuName = "Project/Sound/Sound Database", fileName = "SoundDatabase")]
    public sealed class SoundDatabase : ScriptableObject
    {
        [SerializeField] private List<SoundDefinition> _sounds = new();

        private Dictionary<SoundKey, SoundDefinition> _lookup;

        public SoundDefinition Get(SoundKey key)
        {
            EnsureLookupBuilt();

            if (_lookup.TryGetValue(key, out SoundDefinition definition))
                return definition;
            
            return null;
        }
        
        [ContextMenu("Dump Database")]
        public void DumpDatabase()
        {
            Debug.Log("========== SOUND DATABASE ==========");

            if (_sounds == null || _sounds.Count == 0)
            {
                Debug.Log("Database is empty.");
                return;
            }

            for (int i = 0; i < _sounds.Count; i++)
            {
                SoundDefinition definition = _sounds[i];

                if (definition == null)
                {
                    Debug.Log($"[{i}] NULL");
                    continue;
                }

                Debug.Log(
                    $"[{i}] Key='{definition.Key}', " +
                    $"Category={definition.Category}, " +
                    $"Cooldown={definition.Cooldown}, " +
                    $"MaxInstances={definition.MaxConcurrentInstances}");
            }

            Debug.Log("====================================");
        }

        private void EnsureLookupBuilt()
        {
            if (_lookup != null)
                return;

            _lookup = new Dictionary<SoundKey, SoundDefinition>();

            Debug.Log($"[SoundDatabase] Building lookup. Sounds count = {_sounds.Count}");

            foreach (SoundDefinition definition in _sounds)
            {
                if (definition == null)
                {
                    Debug.LogWarning("[SoundDatabase] Null SoundDefinition.");
                    continue;
                }

                if (_lookup.ContainsKey(definition.Key))
                {
                    Debug.LogWarning($"[SoundDatabase] Duplicate key '{definition.Key}'.");
                    continue;
                }

                _lookup.Add(definition.Key, definition);

                Debug.Log($"[SoundDatabase] Added '{definition.Key}'.");
            }

            Debug.Log($"[SoundDatabase] Lookup size = {_lookup.Count}");
        }

        private void OnEnable() => _lookup = null;

#if UNITY_EDITOR
        private void OnValidate() => _lookup = null;
#endif
    }
}