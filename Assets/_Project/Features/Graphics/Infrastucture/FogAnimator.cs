using _Project.Features.Graphics.Domain;

namespace _Project.Features.Graphics.Infrastucture
{
    public interface IFogAnimator
    {
        void SnapTo(FogState state);
    }

    public class FogAnimator : IFogAnimator
    {
        private readonly IFogApplier _applier;

        public FogAnimator(IFogApplier applier)
        {
            _applier = applier;
        }

        public void SnapTo(FogState state)
        {
            _applier.Apply(state.Color, state.StartDistance, state.EndDistance);
        }
    }
}