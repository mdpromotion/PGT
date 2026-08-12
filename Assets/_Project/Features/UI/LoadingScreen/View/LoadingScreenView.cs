using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Project.Features.UI.LoadingScreen.View
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    public class LoadingScreenView : MonoBehaviour
    {
        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        
        [SerializeField] private float _fadeDuration = 0.35f;
        [SerializeField] private Ease _fadeEase = Ease.InOutSine;

        private Tween _fadeTween;
        
        private const int TopmostSortOrder = 32760;

        private void Reset()
        {
            _canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Awake()
        {
            if (!_canvas)
                _canvas = GetComponent<Canvas>();

            if (!_canvasGroup)
                _canvasGroup = GetComponent<CanvasGroup>();
            
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
            _canvas.sortingOrder = TopmostSortOrder;
        }

        public UniTask FadeInAsync()
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
            return FadeTo(1f);
        }

        public UniTask FadeOutAsync()
        {
            return FadeTo(0f).ContinueWith(() =>
            {
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.interactable = false;
            });
        }

        private UniTask FadeTo(float target)
        {
            _fadeTween?.Kill();

            var tcs = new UniTaskCompletionSource();

            _fadeTween = _canvasGroup
                .DOFade(target, _fadeDuration)
                .SetEase(_fadeEase)
                .SetUpdate(UpdateType.Normal, true)
                .OnComplete(() => tcs.TrySetResult());

            return tcs.Task;
        }
    }
}