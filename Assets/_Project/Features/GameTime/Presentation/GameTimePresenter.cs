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
        private IGameTime _gameTime;
        private IFogSettings _fogSettings;
        private IFogApplier _fogApplier;

        [SerializeField] private Transform _sunTransform;
        [SerializeField] private GameTimePresenterSceneConfig _sceneConfig;

        private Light _sunLight;

        private const float MaximumSunIntensity = 1f;
        private const float MinimumSunIntensity = 0.01f;

        private const float NightFogStartDistance = 0f;
        private const float NightFogEndDistance = 300f;

        [Inject]
        public void Construct(
            IGameTime gameTime,
            IFogSettings fogSettings,
            IFogApplier fogApplier)
        {
            _gameTime = gameTime;
            _fogSettings = fogSettings;
            _fogApplier = fogApplier;
        }

        private void Start()
        { 
            _gameTime.TimeChanged += OnTimeChanged;

            _sunLight = _sunTransform.GetComponent<Light>();
            if (!_sunLight)
                _sunLight = _sunTransform.gameObject.AddComponent<Light>();

            RenderSettings.ambientMode = AmbientMode.Skybox;
            OnTimeChanged(_gameTime.CurrentTime);
        }

        private void OnTimeChanged(float time)
        {
            UpdateSun(time);
            UpdateSunLight(time);
            UpdateFog(time);
            UpdateEnvironment(time);
        }

        private void UpdateSun(float time)
        {
            float sunAngle =
                time / _gameTime.TicksPerDay * 360f
                + _sceneConfig.SunRotationOffset;

            _sunTransform.localRotation = Quaternion.Euler(
                sunAngle,
                0f,
                0f);
        }

        private void UpdateSunLight(float time)
        {
            float transitionDuration = _gameTime.HoursToTicks(_sceneConfig.TransitionDurationHours);
            float dayHour = _gameTime.HoursToTicks(_sceneConfig.DayTransition.Hour);
            float nightHour = _gameTime.HoursToTicks(_sceneConfig.NightTransition.Hour);
            float dayTransitionStart = dayHour - transitionDuration;
            float nightTransitionStart = nightHour - transitionDuration;

            if (IsInTransition(time, dayTransitionStart, dayHour))
            {
                float t = Mathf.InverseLerp(dayTransitionStart, dayHour, time);
                _sunLight.intensity = t;
                return;
            }

            if (IsInTransition(time, nightTransitionStart, nightHour))
            {
                float t = Mathf.InverseLerp(nightTransitionStart, nightHour, time);
                _sunLight.intensity = MaximumSunIntensity - t;
                return;
            }

            _sunLight.intensity =
                IsDay(time, dayHour, nightHour)
                    ? MaximumSunIntensity
                    : MinimumSunIntensity;
        }

        private void UpdateEnvironment(float time)
        {
            float transitionDuration = _gameTime.HoursToTicks(_sceneConfig.TransitionDurationHours);
            float dayHour = _gameTime.HoursToTicks(_sceneConfig.DayTransition.Hour);
            float nightHour = _gameTime.HoursToTicks(_sceneConfig.NightTransition.Hour);
            float dayTransitionStart = dayHour - transitionDuration;
            float nightTransitionStart = nightHour - transitionDuration;

            if (IsInTransition(time, dayTransitionStart, dayHour))
            {
                float t = Mathf.InverseLerp(dayTransitionStart, dayHour, time);
                RenderSettings.ambientIntensity = t;
                return;
            }

            if (IsInTransition(time, nightTransitionStart, nightHour))
            {
                float t = Mathf.InverseLerp(nightTransitionStart, nightHour, time);
                RenderSettings.ambientIntensity = 1f - t;
                return;
            }

            RenderSettings.ambientIntensity =
                IsDay(time, dayHour, nightHour)
                    ? 1f
                    : 0f;
        }

        private void UpdateFog(float time)
        {
            float transitionDuration = _gameTime.HoursToTicks(_sceneConfig.TransitionDurationHours);
            float dayHour = _gameTime.HoursToTicks(_sceneConfig.DayTransition.Hour);
            float nightHour = _gameTime.HoursToTicks(_sceneConfig.NightTransition.Hour);
            float dayTransitionStart = dayHour - transitionDuration;
            float nightTransitionStart = nightHour - transitionDuration;

            float dayStartDistance = _fogSettings.OriginalFogStartDistance;
            float dayEndDistance = _fogSettings.OriginalFogEndDistance;

            float nightStartDistance = NightFogStartDistance;
            float nightEndDistance = NightFogEndDistance;

            if (IsInTransition(time, dayTransitionStart, dayHour))
            {
                float t = Mathf.InverseLerp(dayTransitionStart, dayHour, time);

                ApplyFogTransition(
                    _sceneConfig.NightTransition.FogColor,
                    nightStartDistance,
                    nightEndDistance,
                    _sceneConfig.DayTransition.FogColor,
                    dayStartDistance,
                    dayEndDistance,
                    t);

                return;
            }

            if (IsInTransition(time, nightTransitionStart, nightHour))
            {
                float t = Mathf.InverseLerp(nightTransitionStart, nightHour, time);

                ApplyFogTransition(
                    _sceneConfig.DayTransition.FogColor,
                    dayStartDistance,
                    dayEndDistance,
                    _sceneConfig.NightTransition.FogColor,
                    nightStartDistance,
                    nightEndDistance,
                    t);

                return;
            }

            if (IsDay(time, dayHour, nightHour))
            {
                _fogApplier.Apply(
                    _sceneConfig.DayTransition.FogColor,
                    dayStartDistance,
                    dayEndDistance);
            }
            else
            {
                _fogApplier.Apply(
                    _sceneConfig.NightTransition.FogColor,
                    nightStartDistance,
                    nightEndDistance);
            }
        }

        private void ApplyFogTransition(
            Color fromColor,
            float fromStartDistance,
            float fromEndDistance,
            Color toColor,
            float toStartDistance,
            float toEndDistance,
            float t)
        {
            Color color = Color.Lerp(fromColor, toColor, t);
            float startDistance = Mathf.Lerp(fromStartDistance, toStartDistance, t);
            float endDistance = Mathf.Lerp(fromEndDistance, toEndDistance, t);

            _fogApplier.Apply(
                color,
                startDistance,
                endDistance);
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