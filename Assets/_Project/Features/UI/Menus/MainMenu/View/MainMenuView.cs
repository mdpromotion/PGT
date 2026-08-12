using UnityEngine;

namespace _Project.Features.UI.Menus.MainMenu.View
{
    public class MainMenuView : MonoBehaviour
    {
        private GameObject _currentMenu;

        public void Awake()
        {
            _currentMenu = gameObject;
            _currentMenu.SetActive(false);
        }

        public void ToggleMenu(bool state)
        {
            _currentMenu.SetActive(state);   
        }
    }
}
