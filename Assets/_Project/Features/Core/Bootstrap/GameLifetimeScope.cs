using System;
using System.Collections.Generic;
using _Project.Features.Camera.Infrastructure;
using _Project.Features.Player.Application;
using _Project.Features.Player.Domain;
using _Project.Features.Player.Infrastructure;
using _Project.Features.Player.Presentation;
using _Project.Features.ProceduralWorld.Application.Chunks;
using _Project.Features.ProceduralWorld.Application.Chunks.Generation;
using _Project.Features.ProceduralWorld.Application.Interfaces;
using _Project.Features.ProceduralWorld.Application.Landscape;
using _Project.Features.ProceduralWorld.Application.World;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.World;
using _Project.Features.ProceduralWorld.Infrastructure;
using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;
using _Project.Features.ProceduralWorld.Infrastructure.Interfaces;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Settings;
using _Project.Features.ProceduralWorld.Infrastructure.Landscape;
using _Project.Features.ProceduralWorld.Presentation;
using _Project.Features.Sound.Application;
using _Project.Features.Sound.Infrastructure;
using _Project.Features.Sound.Presentation;
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
        private Transform chunksParent;
        
        [SerializeField] 
        private HydrologySettings hydrologySettings;

        [SerializeField]
        private int viewDistance = 3;
        
        [SerializeField] 
        private SoundDatabase _database;
        
        [SerializeField] 
        private int _globalMaxVoices = 32;

        protected override void Configure(
            IContainerBuilder builder)
        {
            RegisterSound(builder);
            
            RegisterPlayer(builder);

            RegisterProceduralWorld(builder);
        }


        private void RegisterSound(IContainerBuilder builder)
        {
            builder.RegisterInstance(_database);
            builder.Register(_ => new SoundPlaybackGuard(_globalMaxVoices), Lifetime.Singleton);
            builder.RegisterComponentOnNewGameObject<SoundVoicePool>(Lifetime.Singleton, "SoundVoicePool")
                .DontDestroyOnLoad();
            builder.Register<SoundService>(Lifetime.Singleton).As<ISoundService>();
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
                hydrologySettings);



            builder.Register(
                    container =>
                        new ChunkGrid(
                            chunkPrefab.terrainData.size.x,
                            chunkPrefab.terrainData.size.z),
                    Lifetime.Singleton);
            
            builder.Register<TerrainNoiseSettingsProvider>(
                    Lifetime.Singleton)
                .AsSelf()
                .As<IDisposable>();



            builder.Register<LandscapeGenerator>(
                    Lifetime.Singleton)
                .As<IGenerationStage>();



            builder.Register<HydrologyRegionBuilder>(
                    Lifetime.Singleton);


            builder.Register<HydrologyRegionCache>(
                    Lifetime.Singleton);
            

            builder.Register<HydrologyGenerator>(
                    Lifetime.Singleton)
                .As<IGenerationStage>()
                .As<IGenerationCacheEvictor>()
                .As<IDisposable>();



            builder.Register<ChunkGenerationPipeline>(
                Lifetime.Singleton);



            builder.RegisterBuildCallback(
                container =>
                {
                    ChunkGenerationPipeline pipeline =
                        container.Resolve<ChunkGenerationPipeline>();


                    foreach(IGenerationStage stage in 
                            container.Resolve<
                                IEnumerable<IGenerationStage>>())
                    {
                        pipeline.Add(stage);
                    }
                });



            builder.Register<IChunkGenerator>(
                    container =>
                        container.Resolve<
                            ChunkGenerationPipeline>(),
                    Lifetime.Singleton);



            builder.Register(
                    container =>
                        new ChunkGenerationScheduler(
                            container.Resolve<
                                IChunkGenerator>()),
                    Lifetime.Singleton);



            builder.Register(
                    container =>
                        new LandscapeChunkFactory(
                            chunkPrefab,
                            container.Resolve<ChunkGrid>()),
                    Lifetime.Singleton)
                .As<ILandscapeFactory>()
                .As<IDisposable>();



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
                    new ChunkWaterPresenter(
                        chunkPrefab.terrainData.size.y,
                        1),
                Lifetime.Singleton);



            builder.Register(
                    container =>
                        new LandscapeApplier(
                            container.Resolve<ILandscapeFactory>(),
                            container.Resolve<ITerrainWriter>(),
                            container.Resolve<IChunkNeighborConnector>(),
                            container.Resolve<ChunkRepository>(),
                            container.Resolve<ChunkWaterPresenter>(),
                            chunksParent),
                    Lifetime.Singleton);



            builder.Register(
                    container =>
                        new ChunkManager(
                            container.Resolve<ChunkGenerationScheduler>(),
                            container.Resolve<ChunkRepository>(),
                            container.Resolve<LandscapeApplier>(),
                            container.Resolve<ILandscapeFactory>(),
                            container.Resolve<IChunkNeighborConnector>()),
                    Lifetime.Singleton);



            builder.Register(
                container =>
                    new WorldStreamer(
                        container.Resolve<ChunkManager>(),
                        container.Resolve<ChunkGrid>(),
                        container.Resolve<IPlayerReadOnly>(),
                        viewDistance,
                        container.Resolve<IEnumerable<IGenerationCacheEvictor>>()),
                Lifetime.Singleton);



            builder.RegisterComponentInHierarchy<ProceduralWorldPresenter>();
        }
    }
}