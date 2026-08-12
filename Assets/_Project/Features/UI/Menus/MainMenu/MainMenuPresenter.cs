using System;
using System.Collections.Generic;
using _Project.Features.UI.Application;
using _Project.Features.UI.Infrastructure;
using _Project.Features.UI.Menus.MainMenu.View;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace _Project.Features.UI.Menus.MainMenu
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
        private LoadSceneController _loadSceneController;
        private SceneTransitionService _sceneTransitionService;

        private bool _isLoading;

        [Inject]
        public void Construct(MainMenuModel model, LoadSceneController loadSceneController, SceneTransitionService sceneTransitionService)
        {
            _model = model;
            _loadSceneController = loadSceneController;
            _sceneTransitionService = sceneTransitionService;
        }

        public void Start()
        {
            _isLoading = false;
            
            HandleWorldMenu();
            
            foreach (var button in buttons)
            {
                button.ButtonClicked += OnButtonClicked;
            }
            
            _sceneTransitionService.CompleteAsync().Forget();
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
            if (_isLoading)
            {
                print("here");
                return;
            }
            
            _loadSceneController.LoadGameScene().Forget();
            _isLoading = true;
        }

        private void HandleSettings()
        {
            // soon
        }
        
        private void SetMenuState(MenuType menuType, bool state)
        {
            foreach (var menu in menus)
            {
                menu.mainMenuView.ToggleMenu(false);
                
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
