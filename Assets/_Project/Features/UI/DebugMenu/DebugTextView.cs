using UnityEngine;
using UnityEngine.UI;

namespace _Project.Features.UI.DebugMenu
{
    [RequireComponent(typeof(Text))]
    public class DebugTextView : MonoBehaviour
    {
        private Text _debugText;

        private void Awake()
        {
            _debugText = GetComponent<Text>();
        }

        public void ChangeText(string text)
        {
            _debugText.text = text;
        }

        public void ToggleText(bool state)
        {
            _debugText.gameObject.SetActive(state);
        }
    }
}
