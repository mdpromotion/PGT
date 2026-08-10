using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Features.UI.MainMenu
{
    [RequireComponent(typeof(Button))]
    public class MenuButton : ButtonBase
    {
        private Button _menuButton;

        private void Awake()
        {
            _menuButton = GetComponent<Button>();
            _menuButton.onClick.AddListener(OnButtonClick);
        }

        private void OnDestroy()
        {
            _menuButton.onClick.RemoveAllListeners();
        }
    }
}
