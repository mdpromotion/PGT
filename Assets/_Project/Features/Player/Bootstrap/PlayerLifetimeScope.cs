using System;
using _Project.Features.Player.Application;
using _Project.Features.Player.Domain;
using _Project.Features.Player.Infrastructure;
using _Project.Features.Player.Presentation;
using VContainer;
using VContainer.Unity;
using IFpsPlayerMotor = _Project.Features.Camera.Presentation.IFpsPlayerMotor;

namespace _Project.Features.Player.Bootstrap
{
    public sealed class PlayerLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<InputSystem_Actions>(Lifetime.Singleton);
            builder.Register<PlayerInputReader>(Lifetime.Singleton)
                .As<IPlayerInputReader>()
                .As<IInitializable>()
                .As<IDisposable>();
            
            builder.Register<FpsMovementUseCase>(Lifetime.Singleton);
            
            builder.RegisterComponentInHierarchy<FpsPlayerMotor>()
                .As<IFpsPlayerMotor>();

            builder.RegisterComponentInHierarchy<RigidbodyPlayerState>()
                .As<IPlayerReadOnly>();
        }
    }
}