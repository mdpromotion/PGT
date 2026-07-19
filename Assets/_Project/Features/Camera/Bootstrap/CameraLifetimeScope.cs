using _Project.Features.Camera.Infrastructure;
using VContainer;
using VContainer.Unity;

namespace _Project.Features.Camera.Bootstrap
{
    public sealed class CameraLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<FpsCameraController>();
        }
    }
}