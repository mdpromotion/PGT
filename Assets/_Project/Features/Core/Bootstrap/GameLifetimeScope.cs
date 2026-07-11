using _Project.Features.ProceduralWorld.Application;
using _Project.Features.TerrainGeneration.Presentation;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Features.Core.Bootstrap
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private TerrainGenerationPresenter terrainPresenter;


        protected override void Configure(
            IContainerBuilder builder)
        {
            builder.Register<
                ChunkManager>(
                Lifetime.Singleton);


            builder.RegisterComponent(
                terrainPresenter);
        }
    }
}