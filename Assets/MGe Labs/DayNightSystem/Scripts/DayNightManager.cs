using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MGeLabs.DayNightSystem
{
    /// <summary>
    /// Manages the day-night cycle in the game, including time progression, visual updates, and events.
    /// </summary>
    public class DayNightManager : MonoBehaviour
    {
        #region Fields

        [Header("Time & Day")]
        [SerializeField] [Range(0, 1440f)] protected float currentTime;
        [SerializeField] protected int currentDay;

        [Header("Visuals")]
        [Tooltip("Configuration for visual aspects of the day-night cycle (leave empty for none).")]
        [SerializeField] protected DayNightVisualConfig visualConfig;
        [Tooltip("The directional light representing the sun in the scene.")]
        [SerializeField] protected Light directionalLight;
        [Tooltip("Additional visual modules to update with the day-night cycle.")]
        [SerializeField] protected List<DayNightVisualModule> visualModules;

        [Header("Configuration")]
        [Tooltip("Multiplier for the time progression speed.")]
        [SerializeField] protected float timeScale = 1;
        [Tooltip("Format for time formatting.")]
        [SerializeField] protected string displayTimeFormat = "{0:00}:{1:00}";

        [Header("Limit")]
        [Tooltip(
            "A limit to the time. Once reached, the time won't progress further until manually reset. Keep at -1 for no limit.")]
        [SerializeField] [Range(-1, 1440f)] protected float currentTimeLimit = -1;

        [Header("Events")]
        [Tooltip("Invoked when the hour changes, passing the new hour as an integer (0-23).")]
        public UnityEvent<int> OnHourChanged;
        [Tooltip("Invoked when the time limit is reached, passing the reached time.")]
        public UnityEvent<float> OnTimeLimitReached;
        [Tooltip("Invoked when the day changes, passing the new day count as an integer.")]
        public UnityEvent<int> OnDayChanged;
        public event Action<int> OnHourChangedAction;
        public event Action<float> OnTimeLimitReachedAction;
        public event Action<int> OnDayChangedAction;

        protected const float MinutesPerDay = 1440f;

        protected int lastHour = -1;
        protected float startTime;
        protected bool isTimeLimitReached;
        protected DayNightVisualizer visualizer;

        #endregion

        #region Getters

        public virtual float TimeScale => timeScale;
        public virtual float CurrentTime => currentTime;
        public virtual float NormalizedTime => currentTime * InverseDayLength;
        public virtual string DisplayTime => string.Format(displayTimeFormat, CurrentHour, CurrentMinutes);
        public virtual int CurrentDay => currentDay;
        public virtual int CurrentHour => Mathf.FloorToInt(currentTime / 60);
        public virtual int CurrentMinutes => Mathf.FloorToInt(currentTime % 60);
        public virtual DayNightData DayNightData => new(currentDay, currentTime);
        public static float OneHourInMinutes => MinutesPerDay / 24f;
        public static float InverseDayLength => 1f / MinutesPerDay;

        #endregion


        /// <summary>
        /// Unity's Awake method. Initializes the visualizer and base class.
        /// </summary>
        protected virtual void Awake()
        {
            startTime = currentTime;

            if (visualConfig)
            {
                visualizer = new DayNightVisualizer(visualConfig, directionalLight, visualModules);
                visualizer.Init();
            }
        }

        /// <summary>
        /// Unity's Update method. Progresses time, triggers events, and updates visuals.
        /// </summary>
        protected virtual void Update()
        {
            if (isTimeLimitReached) return;

            float delta = Time.deltaTime * timeScale;

            if (currentTimeLimit >= 0)
            {
                float nextTime = Mathf.Repeat(currentTime + delta, MinutesPerDay);

                if (HasCrossedClock(currentTime, nextTime, currentTimeLimit))
                {
                    currentTime = currentTimeLimit;
                    isTimeLimitReached = true;

                    OnTimeLimitReached?.Invoke(currentTimeLimit);
                    OnTimeLimitReachedAction?.Invoke(currentTimeLimit);

                    visualizer?.Update(NormalizedTime);
                    return;
                }
            }

            float previousTime = currentTime;
            currentTime = Mathf.Repeat(currentTime + delta, MinutesPerDay);

            int newHour = Mathf.FloorToInt(currentTime / OneHourInMinutes);
            if (newHour != lastHour)
            {
                lastHour = newHour;
                OnHourChanged?.Invoke(newHour);
                OnHourChangedAction?.Invoke(newHour);
            }

            if (currentTime < previousTime)
            {
                currentDay++;

                OnDayChanged?.Invoke(currentDay);
                OnDayChangedAction?.Invoke(currentDay);
            }

            visualizer?.Update(NormalizedTime);
        }

        #region Setters

        /// <summary>
        /// Sets the visual configuration for the day-night cycle.
        /// </summary>
        /// <param name="config">The visual configuration.</param>
        /// <returns>The updated DayNightManager instance.</returns>
        public virtual DayNightManager SetVisualConfig(DayNightVisualConfig config)
        {
            visualConfig = config;

            if (visualConfig)
            {
                visualizer?.Cleanup();

                visualizer = new DayNightVisualizer(visualConfig, directionalLight, visualModules);
                visualizer.Init();
                visualizer.Update(NormalizedTime);
            }

            return this;
        }

        /// <summary>
        /// Sets the time progression speed multiplier.
        /// </summary>
        /// <param name="scale">The new time scale multiplier.</param>
        public virtual DayNightManager SetTimeScale(float scale)
        {
            timeScale = scale;
            return this;
        }

        /// <summary>
        /// Sets the current time of day.
        /// </summary>
        /// <param name="time">Time as a percentage (0 for midnight, 1 for noon).</param>
        public virtual DayNightManager SetTime(float time)
        {
            time = Mathf.Repeat(time, 1f);

            currentTime = time * MinutesPerDay;
            lastHour = -1;
            return this;
        }

        /// <summary>
        /// Sets the current time by hour.
        /// </summary>
        /// <param name="hour">The hour to set (0-23).</param>
        public virtual DayNightManager SetTimeByHour(int hour)
        {
            if (hour is < 0 or >= 24)
            {
                throw new ArgumentOutOfRangeException(nameof(hour), "Hour must be between 0 and 23.");
            }

            currentTime = hour * OneHourInMinutes;
            lastHour = -1;
            return this;
        }

        /// <summary>
        /// Sets the current day count.
        /// </summary>
        /// <param name="day">The day count to set.</param>
        public virtual DayNightManager SetDay(int day)
        {
            currentDay = day;
            return this;
        }

        /// <summary>
        /// Sets the day and time from the provided data.
        /// </summary>
        /// <param name="data">The DayNightData containing the day and time information.</param>
        /// <returns>The updated DayNightManager instance.</returns>
        public virtual DayNightManager SetDayAndTimeFromData(DayNightData data)
        {
            currentDay = data.currentDay;
            currentTime = data.currentTime;
            lastHour = -1;
            return this;
        }

        /// <summary>
        /// Resets the current time to the starting time (required if the set <see cref="currentTimeLimit"/>
        /// is reached in order to start over.).
        /// </summary>
        public virtual DayNightManager ResetTime()
        {
            currentTime = startTime;
            isTimeLimitReached = false;
            lastHour = -1;
            return this;
        }

        /// <summary>
        /// Marks the time limit as not reached.
        /// </summary>
        public virtual DayNightManager ResetTimeLimit()
        {
            isTimeLimitReached = false;
            lastHour = -1;
            return this;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Checks if the current time is within a specified range.
        /// </summary>
        /// <param name="from">Start time in hours.</param>
        /// <param name="to">End time in hours.</param>
        /// <returns>True if the current time is within the range, otherwise false.</returns>
        public virtual bool IsWithinTime(float from, float to)
        {
            if (from < to)
            {
                return currentTime >= from * OneHourInMinutes && currentTime <= to * OneHourInMinutes;
            }

            return currentTime >= from * OneHourInMinutes || currentTime <= to * OneHourInMinutes;
        }

        /// <summary>
        /// Checks if the time has crossed a specific clock value.
        /// </summary>
        /// <param name="from">The starting time.</param>
        /// <param name="to">The ending time.</param>
        /// <param name="target">The target time to check.</param>
        protected virtual bool HasCrossedClock(float from, float to, float target)
        {
            if (from == to) return false;
            if (from < to) return target >= from && target <= to;
            return target >= from || target <= to;
        }

        #endregion
    }
}