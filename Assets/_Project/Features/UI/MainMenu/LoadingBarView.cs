using DG.Tweening;
using UnityEngine;

namespace _Project.Features.UI.MainMenu
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
        private Sequence _sequence;

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

            _sequence = DOTween.Sequence(_loadingBar);
            
            _sequence.Join(
                _loadingBar
                    .DOLocalRotate(new Vector3(0f, 0f, -360f), _rotationDuration, _rotateMode)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Restart)
            );

            if (_usePulse)
            {
                _sequence.Join(
                    _loadingBar
                        .DOScale(_pulseScale, _pulseDuration)
                        .SetEase(_pulseEase)
                        .SetLoops(-1, LoopType.Yoyo)
                );
            }

            _sequence.SetUpdate(UpdateType.Normal, true);
            _sequence.SetTarget(_loadingBar);
        }

        private void KillAnimation()
        {
            if (_sequence != null && _sequence.IsActive())
            {
                _sequence.Kill();
                _sequence = null;
            }

            _loadingBar.localRotation = Quaternion.identity;
            _loadingBar.localScale = Vector3.one;
        }
    }
}