using _Project.Features.UI.Application;
using _Project.Features.UI.Infrastructure;
using _Project.Features.UI.MainMenu;
using VContainer;
using VContainer.Unity;

namespace _Project.Features.Core.Bootstrap.Menu
{
    public class GameLifetimeScope : ChildLifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<MainMenuModel>(Lifetime.Singleton);
            
            builder.RegisterComponentInHierarchy<MainMenuPresenter>();
        }
    }
}
