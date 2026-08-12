namespace _Project.Features.UI.Menus.MainMenu
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
