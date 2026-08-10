using _Project.Features.GameTime.Domain;
using _Project.Features.GameTime.Infrastructure;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;

namespace _Project.Features.GameTime.Presentation
{
    public class GameTimePresenter : MonoBehaviour
    {
        private IGameTime _gameTime;

        [SerializeField] private Transform _sunTransform;
        [SerializeField] private GameTimePresenterSceneConfig _sceneConfig;

        [Inject]
        public void Construct(IGameTime gameTime)
        {
            _gameTime = gameTime;
        }

        private void Start()
        {
            _gameTime.TimeChanged += OnTimeChanged;

            RenderSettings.ambientMode = AmbientMode.Skybox;
        }

        private void OnTimeChanged(float time)
        {
            UpdateSun(time);
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

        private void UpdateEnvironment(float time)
        {
            float transitionDuration =
                _gameTime.HoursToTicks(
                    _sceneConfig.TransitionDurationHours);

            float dayHour =
                _gameTime.HoursToTicks(
                    _sceneConfig.DayTransition.Hour);

            float nightHour =
                _gameTime.HoursToTicks(
                    _sceneConfig.NightTransition.Hour);

            float dayTransitionStart =
                dayHour - transitionDuration;

            float nightTransitionStart =
                nightHour - transitionDuration;

            if (IsInTransition(
                    time,
                    dayTransitionStart,
                    dayHour))
            {
                float t = Mathf.InverseLerp(
                    dayTransitionStart,
                    dayHour,
                    time);

                RenderSettings.ambientIntensity = t;

                return;
            }

            if (IsInTransition(
                    time,
                    nightTransitionStart,
                    nightHour))
            {
                float t = Mathf.InverseLerp(
                    nightTransitionStart,
                    nightHour,
                    time);

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
            float transitionDuration =
                _gameTime.HoursToTicks(
                    _sceneConfig.TransitionDurationHours);

            float dayHour =
                _gameTime.HoursToTicks(
                    _sceneConfig.DayTransition.Hour);

            float nightHour =
                _gameTime.HoursToTicks(
                    _sceneConfig.NightTransition.Hour);

            float dayTransitionStart =
                dayHour - transitionDuration;

            float nightTransitionStart =
                nightHour - transitionDuration;

            if (IsInTransition(
                    time,
                    dayTransitionStart,
                    dayHour))
            {
                float transition = Mathf.InverseLerp(
                    dayTransitionStart,
                    dayHour,
                    time);

                ApplyFogTransition(
                    _sceneConfig.NightTransition,
                    _sceneConfig.DayTransition,
                    transition);

                return;
            }

            if (IsInTransition(
                    time,
                    nightTransitionStart,
                    nightHour))
            {
                float transition = Mathf.InverseLerp(
                    nightTransitionStart,
                    nightHour,
                    time);

                ApplyFogTransition(
                    _sceneConfig.DayTransition,
                    _sceneConfig.NightTransition,
                    transition);

                return;
            }

            if (IsDay(time, dayHour, nightHour))
            {
                ApplyFogState(
                    _sceneConfig.DayTransition);
            }
            else
            {
                ApplyFogState(
                    _sceneConfig.NightTransition);
            }
        }

        private bool IsInTransition(
            float time,
            float start,
            float end)
        {
            return time >= start && time <= end;
        }

        private bool IsDay(
            float time,
            float dayHour,
            float nightHour)
        {
            return time >= dayHour && time < nightHour;
        }

        private void ApplyFogTransition(
            GameTimeTransition from,
            GameTimeTransition to,
            float t)
        {
            RenderSettings.fogColor = Color.Lerp(
                from.FogColor,
                to.FogColor,
                t);

            RenderSettings.fogEndDistance = Mathf.Lerp(
                from.FogEnd,
                to.FogEnd,
                t);
        }

        private void ApplyFogState(
            GameTimeTransition state)
        {
            RenderSettings.fogColor = state.FogColor;
            RenderSettings.fogEndDistance = state.FogEnd;
        }

        private void OnDestroy()
        {
            if (_gameTime != null)
            {
                _gameTime.TimeChanged -= OnTimeChanged;
            }
        }
    }
}