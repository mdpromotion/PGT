using UnityEngine;

namespace _Project.Features.UI.MainMenu
{
    public class MainMenuModel
    {
        public bool IsInWorldMenu { get; private set; }

        public bool ToggleWorldMenu()
        {
            IsInWorldMenu = !IsInWorldMenu;
            return IsInWorldMenu;
        }
    }
}
