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

        private void EnsureLookupBuilt()
        {
            if (_lookup != null)
                return;

            _lookup = new Dictionary<SoundKey, SoundDefinition>();

            foreach (SoundDefinition definition in _sounds)
            {
                if (definition == null)
                {
                    continue;
                }

                if (_lookup.ContainsKey(definition.Key))
                {
                    continue;
                }

                _lookup.Add(definition.Key, definition);
            }
        }

        private void OnEnable() => _lookup = null;

#if UNITY_EDITOR
        private void OnValidate() => _lookup = null;
#endif
    }
}