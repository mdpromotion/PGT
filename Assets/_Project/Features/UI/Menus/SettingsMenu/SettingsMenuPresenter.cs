using System.Collections.Generic;
using _Project.Features.Graphics.Domain;
using _Project.Features.Graphics.Infrastucture;
using UnityEngine;
using VContainer;

namespace _Project.Features.UI.Menus.SettingsMenu
{
    public enum SettingsMenuMode
    {
        Quality,
        ShadowQuality,
        ViewDistance,
        AntiAliasing,
        ShadowDistance,
        WindowMode,
        VSync,
    }

    public class SettingsMenuPresenter : MonoBehaviour
    {
        [SerializeField] private List<SettingsMenuDropdown> dropdowns;
        [SerializeField] private SettingsMenuView settingsMenu;
        [SerializeField] private SettingsMenuExitButton exitButton;

        private readonly Dictionary<SettingsMenuMode, int> _cachedValues = new();

        private GraphicsState _graphicsState;
        private IGraphicsConfigResolver _configResolver;

        [Inject]
        public void Construct(
            GraphicsState graphicsState,
            IGraphicsConfigResolver configResolver)
        {
            _graphicsState = graphicsState;
            _configResolver = configResolver;
        }

        private void Awake()
        {
            InitCache();
            
            settingsMenu.ToggleMenuRequested += OnToggled;
            exitButton.ButtonClicked += ForceCloseMenu;
            
            foreach (var dropdown in dropdowns)
            {
                dropdown.ValueChanged += OnValueChanged;
            }
        }

        private void InitCache()
        {
            _cachedValues.Clear();
            _configResolver.ConvertFromGraphicState(_graphicsState, _cachedValues);
        }

        private void OnToggled(bool state)
        {
            if (state)
                OpenMenu();
            else
                CloseMenu();
        }

        private void OpenMenu()
        {
            InitCache();

            foreach (var dropdown in dropdowns)
            {
                if (_cachedValues.TryGetValue(dropdown.Mode, out var value))
                {
                    dropdown.SetValueWithoutNotify(value);
                }
            }
        }

        private void CloseMenu()
        {
            if (_cachedValues.Count == 0)
            {
                InitCache();
            }
            ApplyCachedValues();
        }

        private void ForceCloseMenu()
        {
            settingsMenu.Toggle(false);
            CloseMenu();
        }

        private void OnValueChanged(SettingsMenuMode mode, int value)
        {
            _cachedValues[mode] = value;
        }

        private void ApplyCachedValues()
        {
            var graphicsData =
                _configResolver.ConvertToGraphicData(_cachedValues);
            
            _graphicsState.SetGraphicsData(graphicsData);
        }

        private void OnDestroy()
        {
            foreach (var dropdown in dropdowns)
            {
                dropdown.ValueChanged -= OnValueChanged;
            }
            
            exitButton.ButtonClicked -= ForceCloseMenu;
            settingsMenu.ToggleMenuRequested -= OnToggled;
        }
    }
}