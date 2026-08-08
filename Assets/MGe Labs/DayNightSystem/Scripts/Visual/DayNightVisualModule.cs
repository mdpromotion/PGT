using UnityEngine;

namespace MGeLabs.DayNightSystem
{
    /// <summary>
    /// Base visual module invoked by the day-night visualizer.
    /// </summary>
    public abstract class DayNightVisualModule : MonoBehaviour
    {
        /// <summary>Apply visual changes for the given normalized time (0-1).</summary>
        /// <param name="timePercentage">Normalized time of day in range (0,1).</param>
        public abstract void UpdateVisual(float timePercentage);
    }
}