using System;
using _Project.Features.UI.Infrastructure;
using UnityEngine;
using VContainer;

namespace _Project.Features.UI.DebugMenu
{
    public class DebugMenuPresenter : MonoBehaviour
    { 
        [SerializeField] private DebugTextView _textView;
        
        private IFPSCounter _fpsCounter;

        private const string FpsCounterName = "FPS";
        
        [Inject]
        public void Construct(IFPSCounter fpsCounter)
        {
            _fpsCounter = fpsCounter;
        }


        private void Update()
        {
            if (_fpsCounter == null)
                return;
            
            var text = $"{FpsCounterName}: {_fpsCounter.CurrentFps:F0}";
            _textView.ChangeText(text);
        }
        
        
    }
}
