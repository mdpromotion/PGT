using _Project.Features.UI.Application;
using _Project.Features.UI.Infrastructure;
using _Project.Features.UI.MainMenu;
using VContainer;
using VContainer.Unity;

namespace _Project.Features.Core.Bootstrap.Menu
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<MainMenuModel>(Lifetime.Singleton);
            
            builder.Register<ILoadSceneService, LoadSceneService>(Lifetime.Singleton);
            builder.Register<StartGameUseCase>(Lifetime.Singleton);
            
            builder.RegisterComponentInHierarchy<MainMenuPresenter>();
        }
    }
}
