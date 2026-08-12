using _Project.Features.UI.Menus.MainMenu;
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
