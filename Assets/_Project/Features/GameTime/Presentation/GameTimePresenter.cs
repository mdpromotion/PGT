using _Project.Features.GameTime.Domain;
using _Project.Features.GameTime.Infrastructure;
using _Project.Features.Graphics.Domain;
using _Project.Features.Graphics.Infrastucture;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;

namespace _Project.Features.GameTime.Presentation
{
    public class GameTimePresenter : MonoBehaviour
    {
        private enum DayNightPhase
        {
            Day,
            Night,
            DayTransition,
            NightTransition
        }

        private readonly struct DayNightTimings
        {
            public readonly float DayHour;
            public readonly float NightHour;
            public readonly float DayTransitionStart;
            public readonly float NightTransitionStart;

            public DayNightTimings(float dayHour, float nightHour, float dayTransitionStart, float nightTransitionStart)
            {
                DayHour = dayHour;
                NightHour = nightHour;
                DayTransitionStart = dayTransitionStart;
                NightTransitionStart = nightTransitionStart;
            }
        }

        private IGameTime _gameTime;
        private IFogSettings _fogSettings;
        private IFogAnimator _fogAnimator;

        [SerializeField] private Transform sunTransform;
        [SerializeField] private GameTimePresenterSceneConfig sceneConfig;

        private Light _sunLight;

        private const float MaximumSunIntensity = 1f;
        private const float MinimumSunIntensity = 0.01f;

        private const float NightFogStartDistance = 0f;
        private const float NightFogEndDistance = 300f;

        private DayNightPhase? _lastEnvironmentPhase;

        private readonly object _timeLock = new object();
        private float _pendingTime;
        private bool _hasPendingTime;
        private float _currentTime;

        [Inject]
        public void Construct(
            IGameTime gameTime,
            IFogSettings fogSettings,
            IFogAnimator fogAnimator)
        {
            _gameTime = gameTime;
            _fogSettings = fogSettings;
            _fogAnimator = fogAnimator;
        }

        private void Start()
        {
            _gameTime.TimeChanged += OnTimeChanged;

            _sunLight = sunTransform.GetComponent<Light>();
            if (!_sunLight)
                _sunLight = sunTransform.gameObject.AddComponent<Light>();

            RenderSettings.ambientMode = AmbientMode.Skybox;

            _currentTime = _gameTime.CurrentTime;

            UpdateSun(_currentTime);
            UpdateSunLight(_currentTime);
            UpdateEnvironment(_currentTime);
            ApplyFog(_currentTime);
        }

        private void Update()
        {
            bool timeUpdated = false;

            lock (_timeLock)
            {
                if (_hasPendingTime)
                {
                    _currentTime = _pendingTime;
                    _hasPendingTime = false;
                    timeUpdated = true;
                }
            }

            if (timeUpdated)
            {
                UpdateSun(_currentTime);
                UpdateSunLight(_currentTime);
                UpdateEnvironment(_currentTime);
                ApplyFog(_currentTime);
            }

            _fogAnimator.Tick(Time.deltaTime);
        }

        private void OnTimeChanged(float time)
        {
            lock (_timeLock)
            {
                _pendingTime = time;
                _hasPendingTime = true;
            }
        }

        private void UpdateSun(float time)
        {
            float sunAngle =
                time / _gameTime.TicksPerDay * 360f
                + sceneConfig.SunRotationOffset;

            sunTransform.localRotation = Quaternion.Euler(
                sunAngle,
                0f,
                0f);
        }

        private void UpdateSunLight(float time)
        {
            DayNightPhase phase = GetPhase(time, GetTimings(), out float t);

            _sunLight.intensity = phase switch
            {
                DayNightPhase.DayTransition => Mathf.Lerp(MinimumSunIntensity, MaximumSunIntensity, t),
                DayNightPhase.NightTransition => Mathf.Lerp(MaximumSunIntensity, MinimumSunIntensity, t),
                DayNightPhase.Day => MaximumSunIntensity,
                _ => MinimumSunIntensity
            };
        }

        private void UpdateEnvironment(float time)
        {
            DayNightPhase phase = GetPhase(time, GetTimings(), out float t);
            bool isTransition = phase is DayNightPhase.DayTransition or DayNightPhase.NightTransition;

            if (!isTransition && _lastEnvironmentPhase == phase)
                return;

            RenderSettings.ambientIntensity = phase switch
            {
                DayNightPhase.DayTransition => t,
                DayNightPhase.NightTransition => 1f - t,
                DayNightPhase.Day => 1f,
                _ => 0f
            };

            _lastEnvironmentPhase = phase;
        }

        private void ApplyFog(float time)
        {
            DayNightPhase phase = GetPhase(time, GetTimings(), out float t);
            FogState naturalState = GetNaturalFogState(phase, t);
            
            _fogAnimator.SetTarget(naturalState);
        }
        
        private FogState GetNaturalFogState(DayNightPhase phase, float t)
        {
            return phase switch
            {
                DayNightPhase.Day => GetDayFogState(),
                DayNightPhase.Night => GetNightFogState(),
                DayNightPhase.DayTransition => FogState.Lerp(GetNightFogState(), GetDayFogState(), t),
                DayNightPhase.NightTransition => FogState.Lerp(GetDayFogState(), GetNightFogState(), t),
                _ => GetNightFogState()
            };
        }

        private DayNightTimings GetTimings()
        {
            float transitionDuration = _gameTime.HoursToTicks(sceneConfig.TransitionDurationHours);
            float dayHour = _gameTime.HoursToTicks(sceneConfig.DayTransition.Hour);
            float nightHour = _gameTime.HoursToTicks(sceneConfig.NightTransition.Hour);

            return new DayNightTimings(
                dayHour,
                nightHour,
                dayHour - transitionDuration,
                nightHour - transitionDuration);
        }

        private DayNightPhase GetPhase(float time, in DayNightTimings timings, out float t)
        {
            if (IsInTransition(time, timings.DayTransitionStart, timings.DayHour))
            {
                t = Mathf.InverseLerp(timings.DayTransitionStart, timings.DayHour, time);
                return DayNightPhase.DayTransition;
            }

            if (IsInTransition(time, timings.NightTransitionStart, timings.NightHour))
            {
                t = Mathf.InverseLerp(timings.NightTransitionStart, timings.NightHour, time);
                return DayNightPhase.NightTransition;
            }

            t = 0f;
            return IsDay(time, timings.DayHour, timings.NightHour) ? DayNightPhase.Day : DayNightPhase.Night;
        }

        private FogState GetDayFogState()
        {
            return new FogState(
                sceneConfig.DayTransition.FogColor,
                _fogSettings.OriginalFogStartDistance,
                _fogSettings.OriginalFogEndDistance);
        }

        private FogState GetNightFogState()
        {
            return new FogState(
                sceneConfig.NightTransition.FogColor,
                NightFogStartDistance,
                NightFogEndDistance);
        }

        private bool IsInTransition(float time, float start, float end)
        {
            return time >= start && time <= end;
        }

        private bool IsDay(float time, float dayHour, float nightHour)
        {
            return time >= dayHour && time < nightHour;
        }

        private void OnDestroy()
        {
            if (_gameTime != null)
                _gameTime.TimeChanged -= OnTimeChanged;
        }
    }
}