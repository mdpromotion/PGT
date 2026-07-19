using System;
using UnityEngine;

namespace _Project.Features.Sound.Domain
{
    [Serializable]
    public struct SoundKey : IEquatable<SoundKey>
    {
        [SerializeField] private string _value;

        public SoundKey(string value)
        {
            _value = value;
        }

        public string Value => _value;

        public bool Equals(SoundKey other) => string.Equals(_value, other._value, StringComparison.Ordinal);
        public override bool Equals(object obj) => obj is SoundKey other && Equals(other);
        public override int GetHashCode() => _value != null ? _value.GetHashCode() : 0;
        public override string ToString() => _value;

        public static implicit operator SoundKey(string value) => new SoundKey(value);

        public static bool operator ==(SoundKey a, SoundKey b) => a.Equals(b);
        public static bool operator !=(SoundKey a, SoundKey b) => !a.Equals(b);
    }
}