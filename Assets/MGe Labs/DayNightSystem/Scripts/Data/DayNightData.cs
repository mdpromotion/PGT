using System;

namespace MGeLabs.DayNightSystem
{
    /// <summary>
    /// Represents the data structure for the day-night cycle, including the current day and time.
    /// </summary>
    [Serializable]
    public struct DayNightData
    {
        public int currentDay;
        public float currentTime;

        public DayNightData(int currentDay = 0, float currentTime = 0f)
        {
            this.currentDay = currentDay;
            this.currentTime = currentTime;
        }

        public override string ToString() => $"Day: {currentDay}, Time: {currentTime}";
    }
}