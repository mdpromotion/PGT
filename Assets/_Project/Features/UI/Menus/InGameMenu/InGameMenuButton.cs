using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Features.UI.Menus.InGameMenu
{
    [RequireComponent(typeof(Button))]
    public class InGameMenuButton : MonoBehaviour
    {
        public event Action ButtonClicked;
        
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClicked);
        }
        
        private void OnButtonClicked()
        {
            ButtonClicked?.Invoke();
        }
    }
}
