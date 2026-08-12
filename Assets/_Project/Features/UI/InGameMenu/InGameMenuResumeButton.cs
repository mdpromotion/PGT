using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Features.UI.InGameMenu
{
    public class InGameMenuResumeButton : ButtonBase
    {
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClicked);
        }
    }
}
