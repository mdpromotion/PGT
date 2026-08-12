using System;
using _Project.Features.Core.Domain;
using _Project.Features.Core.Presentation;
using _Project.Features.UI.Application;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Project.Features.UI.Menus.InGameMenu
{
    [RequireComponent(typeof(Button))]
    public class ButtonBase : MonoBehaviour
    {
        public event Action ButtonClicked;
        protected void OnButtonClicked()
        {
            ButtonClicked?.Invoke();
        }
    }
    
    public class InGameMenuPresenter : MonoBehaviour
    {
        [SerializeField] private ButtonBase resumeButton;
        [SerializeField] private ButtonBase exitButton;
        [SerializeField] private InGameMenuView pauseMenu;
        
        private LoadSceneController _loadSceneController;
        private IPlayerUIInputReader _playerUIInputReader;
        private IGameStateController _gameStateController;

        private bool _isVisible;
        
        private void Awake()
        {
            resumeButton.ButtonClicked += OnResumeClicked;
            exitButton.ButtonClicked += OnExitClicked;
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

        private void OnExitClicked()
        {
            _loadSceneController.LoadMenuScene().Forget();
        }

        private void OnDestroy()
        {
            resumeButton.ButtonClicked -= OnResumeClicked;
            exitButton.ButtonClicked -= OnExitClicked;
            _playerUIInputReader.PauseClicked -= OnPauseClicked;
        }
    }
}
