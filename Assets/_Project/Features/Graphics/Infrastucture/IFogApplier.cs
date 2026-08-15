using UnityEngine;

namespace _Project.Features.Graphics.Infrastucture
{
    public interface IFogApplier
    {
        void Apply(Color color, float startDistance, float endDistance);
    }

    public class FogApplier : IFogApplier
    {
        public void Apply(Color color, float startDistance, float endDistance)
        {
            RenderSettings.fogColor = color;
            RenderSettings.fogStartDistance = startDistance;
            RenderSettings.fogEndDistance = endDistance;
        }
    }
}