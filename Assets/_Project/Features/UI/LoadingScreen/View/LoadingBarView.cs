using DG.Tweening;
using UnityEngine;

namespace _Project.Features.UI.LoadingScreen.View
{
    [RequireComponent(typeof(Transform))]
    public class LoadingBarView : MonoBehaviour
    {
        [Header("Rotation")]
        [SerializeField] private float rotationDuration = 1f;
        [SerializeField] private RotateMode rotateMode = RotateMode.FastBeyond360;

        [Header("Pulse")]
        [SerializeField] private bool usePulse = true;
        [SerializeField] private float pulseScale = 1.15f;
        [SerializeField] private float pulseDuration = 0.6f;
        [SerializeField] private Ease pulseEase = Ease.InOutSine;

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
                .DOLocalRotate(new Vector3(0f, 0f, -360f), rotationDuration, rotateMode)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart)
                .SetUpdate(UpdateType.Normal, true)
                .SetTarget(_loadingBar);

            if (usePulse)
            {
                _pulseTween = _loadingBar
                    .DOScale(pulseScale, pulseDuration)
                    .SetEase(pulseEase)
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