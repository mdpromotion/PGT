using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Features.UI.Menus.SettingsMenu
{
    [RequireComponent(typeof(Dropdown))]
    public class SettingsMenuDropdown : MonoBehaviour
    {
        [SerializeField] private SettingsMenuMode currentMode;

        private Dropdown _dropdown;

        public SettingsMenuMode Mode => currentMode;

        public event Action<SettingsMenuMode, int> ValueChanged;

        private void Awake()
        {
            _dropdown = GetComponent<Dropdown>();
            _dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        public void SetValueWithoutNotify(int value)
        {
            if (!_dropdown)
                _dropdown = GetComponent<Dropdown>();
                
            _dropdown.SetValueWithoutNotify(value);
        }

        private void OnValueChanged(int value)
        {
            ValueChanged?.Invoke(currentMode, value);
        }

        private void OnDestroy()
        {
            _dropdown.onValueChanged.RemoveListener(OnValueChanged);
        }
    }
}