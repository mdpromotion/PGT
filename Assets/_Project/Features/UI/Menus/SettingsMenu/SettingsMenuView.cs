using System;
using UnityEngine;

namespace _Project.Features.UI.Menus.SettingsMenu
{
    public class SettingsMenuView : MonoBehaviour
    {
        public event Action<bool> ToggleMenuRequested; 
        
        public void Toggle(bool state)
        {
            gameObject.SetActive(state);
            ToggleMenuRequested?.Invoke(state);
        }
    }
}
