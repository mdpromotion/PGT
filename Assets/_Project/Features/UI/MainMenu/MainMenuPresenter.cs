using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace _Project.Features.UI.MainMenu
{
    public enum MenuType
    {
        WorldMenu,
        StartGame,
        Settings
    }

    [Serializable]
    public struct MenuEntry
    {
        public MenuType menuType;
        public MainMenuView mainMenuView;
        public bool isVisibleWhenActive;
    }
    
    public class MainMenuPresenter : MonoBehaviour
    {
        [SerializeField] private MenuButton[] buttons;
        [SerializeField] private List<MenuEntry> menus;
        
        private MainMenuModel _model;

        [Inject]
        public void Construct(MainMenuModel model)
        {
            _model = model;
        }

        public void Start()
        {
            _model = new MainMenuModel();
            
            HandleWorldMenu();
            
            foreach (var button in buttons)
            {
                button.ButtonClicked += OnButtonClicked;
            }
        }

        public void OnButtonClicked(MenuType buttonType)
        {
            switch (buttonType)
            {
                case MenuType.WorldMenu:
                    HandleWorldMenu();
                    break;
                case MenuType.StartGame:
                    HandleStartGame();
                    break;
                case MenuType.Settings:
                    HandleSettings();
                    break;
            }
        }


        private void HandleWorldMenu()
        {
            bool state = _model.ToggleWorldMenu();
            SetMenuState(MenuType.WorldMenu, state);
        }

        private void HandleStartGame()
        {
            
        }

        private void HandleSettings()
        {
            // soon
        }
        
        private void SetMenuState(MenuType menuType, bool state)
        {
            foreach (var menu in menus)
            {
                if (menu.menuType != menuType)
                    continue;
                
                bool isVisible = menu.isVisibleWhenActive != state;

                menu.mainMenuView.ToggleMenu(isVisible);
            }
        }

        public void OnDestroy()
        {
            foreach (var button in buttons)
            {
                button.ButtonClicked -= OnButtonClicked;
            }
        }
    }
}
