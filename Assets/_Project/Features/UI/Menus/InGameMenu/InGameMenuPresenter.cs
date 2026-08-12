using System;
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
        
        private LoadSceneController _loadSceneController;

        private bool _isVisible = false;
        
        private void Awake()
        {
            resumeButton.ButtonClicked += OnResumeClicked;
            exitButton.ButtonClicked += OnExitClicked;
        }

        [Inject]
        public void Construct(LoadSceneController loadSceneController)
        {
            _loadSceneController = loadSceneController;
        }

        private void OnPauseClicked()
        {
            // soon
        }

        private void OnResumeClicked()
        {
            print("resume");
            // soon
        }

        private void OnExitClicked()
        {
            _loadSceneController.LoadMenuScene().Forget();
        }

        private void OnDestroy()
        {
            resumeButton.ButtonClicked -= OnResumeClicked;
            exitButton.ButtonClicked -= OnExitClicked;
        }
    }
}
