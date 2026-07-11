using _Project.Features.ProceduralWorld.Application;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Infrastructure;
using _Project.Features.TerrainGeneration.Presentation;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Features.ProceduralWorld.Bootstrap
{
    public class TerrainGenerationLifetimeScope 
        : LifetimeScope
    {
        [SerializeField]
        private Terrain chunkPrefab;


        [SerializeField]
        private NoiseSettings noiseSettings;


        protected override void Configure(
            IContainerBuilder builder)
        {
            builder.Register<
                IHeightmapGenerator,
                PerlinHeightmapGenerator>(
                Lifetime.Singleton);


            builder.Register<
                UnityTerrainWriter>(
                Lifetime.Singleton);


            builder.Register<
                GenerateTerrainUseCase>(
                Lifetime.Singleton);


            builder.RegisterInstance(
                noiseSettings);


            builder.Register(
                container =>
                    new TerrainChunkFactory(
                        chunkPrefab,
                        container.Resolve<IHeightmapGenerator>(),
                        noiseSettings),
                Lifetime.Singleton);


            builder.Register<
                ChunkManager>(
                Lifetime.Singleton);


            builder.RegisterComponentInHierarchy
                <TerrainGenerationPresenter>();
        }
    }
}