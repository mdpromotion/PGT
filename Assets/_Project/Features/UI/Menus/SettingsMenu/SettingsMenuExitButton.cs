using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Features.UI.Menus.SettingsMenu
{
    [RequireComponent(typeof(Button))]
    public class SettingsMenuExitButton : MonoBehaviour
    {
        private Button _button;
        
        public event Action ButtonClicked;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClicked);
        }
        
        private void OnButtonClicked() 
            => ButtonClicked?.Invoke();
        
        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }
    }
}
