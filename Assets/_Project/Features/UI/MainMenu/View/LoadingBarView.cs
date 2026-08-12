using DG.Tweening;
using UnityEngine;

namespace _Project.Features.UI.MainMenu.View
{
    [RequireComponent(typeof(Transform))]
    public class LoadingBarView : MonoBehaviour
    {
        [Header("Rotation")]
        [SerializeField] private float _rotationDuration = 1f;
        [SerializeField] private RotateMode _rotateMode = RotateMode.FastBeyond360;

        [Header("Pulse")]
        [SerializeField] private bool _usePulse = true;
        [SerializeField] private float _pulseScale = 1.15f;
        [SerializeField] private float _pulseDuration = 0.6f;
        [SerializeField] private Ease _pulseEase = Ease.InOutSine;

        private Transform _loadingBar;
        private Tween _rotationTween;
        private Tween _pulseTween;

        private void Awake()
        {
            _loadingBar = transform;
        }

        private void OnEnable()
        {
            PlayAnimation();
        }

        private void OnDisable()
        {
            KillAnimation();
        }

        private void PlayAnimation()
        {
            KillAnimation();
            
            _rotationTween = _loadingBar
                .DOLocalRotate(new Vector3(0f, 0f, -360f), _rotationDuration, _rotateMode)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart)
                .SetUpdate(UpdateType.Normal, true)
                .SetTarget(_loadingBar);

            if (_usePulse)
            {
                _pulseTween = _loadingBar
                    .DOScale(_pulseScale, _pulseDuration)
                    .SetEase(_pulseEase)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetUpdate(UpdateType.Normal, true)
                    .SetId(_loadingBar);
            }
        }

        private void KillAnimation()
        {
            _rotationTween?.Kill();
            _rotationTween = null;

            _pulseTween?.Kill();
            _pulseTween = null;

            _loadingBar.localRotation = Quaternion.identity;
            _loadingBar.localScale = Vector3.one;
        }
    }
}