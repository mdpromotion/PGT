using UnityEngine;

namespace MGeLabs.DayNightSystem
{
    /// <summary>
    /// Represents a clock that visually displays the current time based on the day-night cycle.
    /// Updates the rotation of the hour and minute hands dynamically.
    /// </summary>
    public class Clock : MonoBehaviour
    {
        [Header("Setup")]
        [Tooltip("Reference to the DayNightManager that provides the current time.")]
        [SerializeField] private DayNightManager dayNightManager;
        [Tooltip("Transform representing the hour hand of the clock.")]
        [SerializeField] private Transform hourHand;
        [Tooltip("Transform representing the minute hand of the clock.")]
        [SerializeField] private Transform minuteHand;

        [Header("Rotation")]
        [Tooltip("Determines whether the clock hands rotate clockwise or counterclockwise.")]
        [SerializeField] private bool clockwise = true;
        [Tooltip("Offset applied to the hour hand rotation, in hours.")]
        [SerializeField, Range(0f, 24f)] private float hourOffset;

        private void Update()
        {
            float totalMinutes = dayNightManager.CurrentTime;
            float hours24 = totalMinutes / 60f;
            float minutes = totalMinutes % 60f;
            float dir = clockwise ? 1f : -1f;
            float hours12 = (hours24 + hourOffset) % 12f;

            float hourRotation = (hours12 / 12f) * 360f;
            hourHand.localRotation = Quaternion.Euler(0f, 0f, dir * hourRotation);

            float minuteRotation = (minutes / 60f) * 360f;
            minuteHand.localRotation = Quaternion.Euler(0f, 0f, dir * minuteRotation);
        }
    }
}