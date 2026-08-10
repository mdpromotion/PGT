using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Features.UI.MainMenu
{
    public enum MenuType
    {
        WorldMenu,
        StartGame,
        Settings
    }
    
    public class ButtonBase : MonoBehaviour
    {
        public MenuType buttonType = MenuType.WorldMenu; 
        public event Action<MenuType> ButtonClicked;

        protected void OnButtonClick()
        {
            ButtonClicked?.Invoke(buttonType);
        }
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
        [SerializeField] private ButtonBase[] Buttons;
        [SerializeField] private List<MenuEntry> Menus;
        
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
            
            foreach (var button in Buttons)
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
            foreach (var menu in Menus)
            {
                if (menu.menuType != menuType)
                    continue;
                
                bool isVisible = menu.isVisibleWhenActive != state;

                menu.mainMenuView.ToggleMenu(isVisible);
            }
        }

        public void OnDestroy()
        {
            foreach (var button in Buttons)
            {
                button.ButtonClicked -= OnButtonClicked;
            }
        }
    }
}
