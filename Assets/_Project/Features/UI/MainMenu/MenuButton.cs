using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Features.UI.MainMenu
{
    [RequireComponent(typeof(Button))]
    public class MenuButton : MonoBehaviour
    {
        private Button _menuButton;
        
        public MenuType buttonType = MenuType.WorldMenu; 
        public event Action<MenuType> ButtonClicked;

        private void Awake()
        {
            _menuButton = GetComponent<Button>();
            _menuButton.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            ButtonClicked?.Invoke(buttonType);
        }

        private void OnDestroy()
        {
            _menuButton.onClick.RemoveAllListeners();
        }
    }
}
