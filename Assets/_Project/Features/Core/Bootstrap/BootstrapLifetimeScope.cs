using _Project.Features.Core.Infrastructure;
using _Project.Features.Core.Persistence;
using _Project.Features.Core.Presentation;
using _Project.Features.Graphics.Domain;
using _Project.Features.Graphics.Infrastucture;
using _Project.Features.UI.Application;
using _Project.Features.UI.Infrastructure;
using _Project.Features.UI.Menus.LoadingScreen.View;
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
        [SerializeField] private GraphicsConfigResolver graphicsConfigResolver;

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
            
            builder.Register<JsonFileStore>(Lifetime.Singleton)
                .As<IJsonReader>()
                .As<IJsonWriter>();
            
            builder.RegisterInstance(graphicsConfigResolver)
                .As<IGraphicsConfigResolver>();

            builder.Register<GraphicsSettingsRepository>(Lifetime.Singleton)
                .As<IGraphicsSettingsRepository>();

            builder.Register<GraphicsState>(Lifetime.Singleton);

            builder.Register<UnitySettingsApplier>(Lifetime.Singleton)
                .As<IInitializable>();
        }
    }
}