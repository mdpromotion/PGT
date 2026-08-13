using System;
using _Project.Features.Core.Domain;
using _Project.Features.Core.Presentation;
using _Project.Features.UI.Application;
using _Project.Features.UI.Menus.SettingsMenu;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Project.Features.UI.Menus.InGameMenu
{
    public class InGameMenuPresenter : MonoBehaviour
    {
        [SerializeField] private InGameMenuButton resumeButton;
        [SerializeField] private InGameMenuButton exitButton;
        [SerializeField] private InGameMenuButton settingsButton;
        
        [SerializeField] private InGameMenuView pauseMenu;
        [SerializeField] private SettingsMenuView settingsMenu;
        
        private LoadSceneController _loadSceneController;
        private IPlayerUIInputReader _playerUIInputReader;
        private IGameStateController _gameStateController;

        private bool _isVisible;
        private bool _isSettingsVisible;
        
        private void Awake()
        {
            resumeButton.ButtonClicked += OnResumeClicked;
            exitButton.ButtonClicked += OnExitClicked;
            settingsButton.ButtonClicked += OnSettingsClicked;
        }

        [Inject]
        public void Construct(LoadSceneController loadSceneController, IPlayerUIInputReader playerUIInputReader, IGameStateController gameStateController)
        {
            _loadSceneController = loadSceneController;
            _playerUIInputReader = playerUIInputReader;
            _gameStateController = gameStateController;
            
            _playerUIInputReader.PauseClicked += OnPauseClicked;
        }

        private void OnPauseClicked()
        {
            if (_isSettingsVisible)
            {
                _isSettingsVisible = false;
                settingsMenu.Toggle(_isSettingsVisible);
                return;
            }
            
            _isVisible = !_isVisible;
            pauseMenu.Toggle(_isVisible);
            
            _gameStateController.SetPaused(_isVisible);
        }

        private void OnResumeClicked()
        {
            _isVisible = false;
            pauseMenu.Toggle(_isVisible);
            
            _gameStateController.SetPaused(false);
        }

        private void OnSettingsClicked()
        {
            _isSettingsVisible = !_isSettingsVisible;
            _isVisible = !_isVisible;
            
            settingsMenu.Toggle(_isSettingsVisible);
            pauseMenu.Toggle(_isVisible);
        }

        private void OnExitClicked()
        {
            _loadSceneController.LoadMenuScene().Forget();
        }

        private void OnDestroy()
        {
            resumeButton.ButtonClicked -= OnResumeClicked;
            exitButton.ButtonClicked -= OnExitClicked;
            settingsButton.ButtonClicked -= OnSettingsClicked;
            _playerUIInputReader.PauseClicked -= OnPauseClicked;
        }
    }
}
