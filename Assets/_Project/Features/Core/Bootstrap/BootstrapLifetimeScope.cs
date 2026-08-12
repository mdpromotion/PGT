using _Project.Features.Core.Infrastructure;
using _Project.Features.Core.Presentation;
using _Project.Features.UI.Application;
using _Project.Features.UI.Infrastructure;
using _Project.Features.UI.LoadingScreen.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Features.Core.Bootstrap
{
    public class BootstrapLifetimeScope : LifetimeScope
    {
        public static BootstrapLifetimeScope Instance { get; private set; }

        [SerializeField] private LoadingScreenView loadingScreenView;
        [SerializeField] private SceneDatabase sceneDatabase;

        protected override void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<InputSystem_Actions>(Lifetime.Singleton);
            
            builder.Register<InputReader>(Lifetime.Singleton)
                .As<IPlayerInputReader>()
                .As<IPlayerUIInputReader>()
                .As<IInitializable>();
            
            builder.Register<ILoadSceneService, LoadSceneService>(Lifetime.Singleton);
            
            builder.Register<BootstrapEntryPoint>(Lifetime.Singleton)
                .As<IInitializable>();
            
            builder.RegisterInstance(loadingScreenView);
            builder.RegisterInstance(sceneDatabase);
            
            builder.Register<SceneTransitionService>(Lifetime.Singleton);
            
            builder.Register<LoadSceneController>(Lifetime.Singleton);
        }
    }
}