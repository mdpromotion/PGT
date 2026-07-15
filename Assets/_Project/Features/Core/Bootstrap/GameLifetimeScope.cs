using System;
using _Project.Features.Camera.Presentation;
using _Project.Features.Player.Application;
using _Project.Features.Player.Domain;
using _Project.Features.Player.Infrastructure;
using _Project.Features.Player.Presentation;
using _Project.Features.ProceduralWorld.Application.Chunks;
using _Project.Features.ProceduralWorld.Application.Chunks.Modifiers;
using _Project.Features.ProceduralWorld.Application.Hydrology;
using _Project.Features.ProceduralWorld.Application.Interfaces;
using _Project.Features.ProceduralWorld.Application.World;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Biomes;
using _Project.Features.ProceduralWorld.Domain.Hydrology;
using _Project.Features.ProceduralWorld.Domain.World;
using _Project.Features.ProceduralWorld.Infrastructure;
using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;
using _Project.Features.ProceduralWorld.Infrastructure.World;
using _Project.Features.ProceduralWorld.Presentation;
using UnityEngine;
using VContainer;
using VContainer.Unity;


namespace _Project.Features.Core.Bootstrap
{
    public class GameLifetimeScope : LifetimeScope
    {
        [Header("Procedural World")]

        [SerializeField]
        private Terrain chunkPrefab;

        [SerializeField]
        private WorldSettings worldSettings;

        [SerializeField]
        private BiomeDatabase biomeDatabase;

        [SerializeField]
        private HydrologySettings hydrologySettings;

        [SerializeField]
        private Transform chunksParent;

        [SerializeField]
        private int viewDistance = 3;



        protected override void Configure(
            IContainerBuilder builder)
        {
            RegisterPlayer(builder);

            RegisterProceduralWorld(builder);
        }



        private void RegisterPlayer(
            IContainerBuilder builder)
        {
            builder.Register<InputSystem_Actions>(
                Lifetime.Singleton);


            builder.Register<PlayerInputReader>(
                    Lifetime.Singleton)
                .As<IPlayerInputReader>()
                .As<IInitializable>()
                .As<IDisposable>();


            builder.RegisterComponentInHierarchy<FpsCameraController>();


            builder.Register<FpsMovementUseCase>(
                Lifetime.Singleton);


            builder.RegisterComponentInHierarchy<FpsPlayerMotor>()
                .As<IFpsPlayerMotor>();


            builder.RegisterComponentInHierarchy<RigidbodyPlayerState>()
                .As<IPlayerReadOnly>();
        }



        private void RegisterProceduralWorld(
            IContainerBuilder builder)
        {
            builder.RegisterInstance(
                worldSettings);


            builder.RegisterInstance(
                biomeDatabase);


            builder.RegisterInstance(
                hydrologySettings);



            builder.Register(
                container =>
                    new ChunkGrid(
                        chunkPrefab.terrainData.size.x,
                        chunkPrefab.terrainData.size.z),
                Lifetime.Singleton);



            builder.Register<ClimateGenerator>(
                Lifetime.Singleton);


            builder.Register<BiomeResolver>(
                    Lifetime.Singleton)
                .As<IBiomeResolver>();


            builder.Register<WorldGenerator>(
                Lifetime.Singleton);

            
            builder.Register<HydrologyRegionCache>(
                Lifetime.Singleton);


            builder.Register<HydrologyService>(
                    Lifetime.Singleton)
                .As<IHydrologyProvider>()
                .AsSelf();

            builder.Register<HydrologyOrchestrator>(Lifetime.Singleton).AsSelf();
            

            builder.Register<HeightModifierPipeline>(
                Lifetime.Singleton);


            builder.Register(
                    container =>
                        new RiverHeightModifier(
                            container.Resolve<IHydrologyProvider>(),
                            hydrologySettings.Enabled),
                    Lifetime.Singleton)
                .As<IHeightModifier>();
            
            builder.RegisterComponentInHierarchy<HydrologyRegionVisualizer>();
            
            builder.RegisterBuildCallback(
                container =>
                {
                    container.Resolve<HeightModifierPipeline>()
                        .Add(
                            container.Resolve<IHeightModifier>());
                });



            builder.Register<IChunkGenerator>(
                container =>
                    new BurstChunkGenerator(
                        container.Resolve<ChunkGrid>(),
                        container.Resolve<WorldSettings>(),
                        container.Resolve<HeightModifierPipeline>()),
                Lifetime.Singleton);



            builder.Register(
                    container =>
                        new TerrainChunkFactory(
                            chunkPrefab,
                            container.Resolve<ChunkGrid>()),
                    Lifetime.Singleton)
                .As<ITerrainFactory>();



            builder.Register<ChunkNeighborConnector>(
                    Lifetime.Singleton)
                .As<IChunkNeighborConnector>();
            
            builder.Register<UnityTerrainWriter>(
                    Lifetime.Singleton)
                .As<ITerrainWriter>();



            builder.Register<ChunkRepository>(
                    Lifetime.Singleton)
                .AsSelf()
                .As<IChunkLookup>();



            builder.Register(
                    container =>
                        new ChunkGenerationScheduler(
                            container.Resolve<IChunkGenerator>()),
                    Lifetime.Singleton);



            builder.Register(
                    container =>
                        new ChunkApplier(
                            container.Resolve<ITerrainFactory>(),
                            container.Resolve<ITerrainWriter>(),
                            container.Resolve<IChunkNeighborConnector>(),
                            container.Resolve<ChunkRepository>(),
                            chunksParent),
                    Lifetime.Singleton);



            builder.Register(
                    container =>
                        new ChunkManager(
                            container.Resolve<ChunkGrid>(),
                            container.Resolve<ChunkGenerationScheduler>(),
                            container.Resolve<ChunkRepository>(),
                            container.Resolve<ChunkApplier>(),
                            container.Resolve<ITerrainFactory>(),
                            container.Resolve<IChunkNeighborConnector>(),
                            container.Resolve<WorldGenerator>()),
                    Lifetime.Singleton);



            builder.Register(
                    container =>
                        new WorldStreamer(
                            container.Resolve<ChunkManager>(),
                            container.Resolve<ChunkGrid>(),
                            container.Resolve<IPlayerReadOnly>(),
                            viewDistance),
                    Lifetime.Singleton);



            builder.RegisterComponentInHierarchy<ProceduralWorldPresenter>();
        }
    }
}