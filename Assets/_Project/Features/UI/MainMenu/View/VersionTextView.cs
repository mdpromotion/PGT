using UnityEngine;
using UnityEngine.UI;

namespace _Project.Features.UI.MainMenu.View
{
    [RequireComponent(typeof(Text))]
    public class VersionTextView : MonoBehaviour
    {
        private Text _versionText;

        private void Awake()
        {
            _versionText = GetComponent<Text>();
            _versionText.text = $"{UnityEngine.Application.version}";
        }
    }
}
