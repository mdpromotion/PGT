using UnityEngine;
using UnityEngine.UI;

namespace _Project.Features.UI.MainMenu.View
{
    [RequireComponent(typeof(Text))]
    public class GameNameText : MonoBehaviour
    {
        private Text _gameNameText;
        
        private void Awake()
        {
            _gameNameText = GetComponent<Text>();
            _gameNameText.text = $"{UnityEngine.Application.productName}";
        }
    }
}
