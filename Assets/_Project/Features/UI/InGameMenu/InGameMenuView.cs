using UnityEngine;

namespace _Project.Features.UI.InGameMenu
{
    public class InGameMenuView : MonoBehaviour
    {
        public void Toggle(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}
