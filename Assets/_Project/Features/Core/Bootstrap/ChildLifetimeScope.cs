using VContainer.Unity;

namespace _Project.Features.Core.Bootstrap
{
    public abstract class ChildLifetimeScope : LifetimeScope
    {
        protected override void Awake()
        {
            if (BootstrapLifetimeScope.Instance != null)
                EnqueueParent(BootstrapLifetimeScope.Instance);

            base.Awake();
        }
    }
}