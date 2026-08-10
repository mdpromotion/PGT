namespace _Project.Features.Tick.Domain
{
    public class TickData
    {
        public float TickInterval { get; private set; } = 1f / 20f;

        public void SetTickInterval(float tickInterval)
        {
            TickInterval = tickInterval;
        }
    }
}