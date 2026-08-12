using UnityEngine.UI;

namespace _Project.Features.UI.Menus.InGameMenu
{
    public class InGameMenuExitButton : ButtonBase
    {
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClicked);
        }
    }
}
