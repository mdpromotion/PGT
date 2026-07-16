using System;
using System.Collections.Generic;
using _Project.Features.Camera.Presentation;
using _Project.Features.Player.Application;
using _Project.Features.Player.Domain;
using _Project.Features.Player.Infrastructure;
using _Project.Features.Player.Presentation;
using _Project.Features.ProceduralWorld.Application.Chunks;
using _Project.Features.ProceduralWorld.Application.Chunks.Modifiers;
using _Project.Features.ProceduralWorld.Application.Interfaces;
using _Project.Features.ProceduralWorld.Application.World;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Biomes;
using _Project.Features.ProceduralWorld.Domain.World;
using _Project.Features.ProceduralWorld.Infrastructure;
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



            builder.Register(
                container =>
                    new ChunkGrid(
                        chunkPrefab.terrainData.size.x,
                        chunkPrefab.terrainData.size.z),
                Lifetime.Singleton);



            builder.Register<BiomeResolver>(
                    Lifetime.Singleton)
                .As<IBiomeResolver>();



            builder.Register<HeightModifierPipeline>(
                Lifetime.Singleton);

            builder.RegisterBuildCallback(
                container =>
                {
                    HeightModifierPipeline pipeline =
                        container.Resolve<HeightModifierPipeline>();

                    foreach(IHeightModifier modifier in 
                            container.Resolve<IEnumerable<IHeightModifier>>())
                    {
                        pipeline.Add(modifier);
                    }
                });
            
            builder.Register<ILandscapeGenerator>(
                container =>
                    new BurstLandscapeGenerator(
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
                            container.Resolve<ILandscapeGenerator>()),
                    Lifetime.Singleton);



            builder.Register(
                    container =>
                        new LandscapeApplier(
                            container.Resolve<ITerrainFactory>(),
                            container.Resolve<ITerrainWriter>(),
                            container.Resolve<IChunkNeighborConnector>(),
                            container.Resolve<ChunkRepository>(),
                            chunksParent),
                    Lifetime.Singleton);



            builder.Register(
                    container =>
                        new ChunkManager(
                            container.Resolve<ChunkGenerationScheduler>(),
                            container.Resolve<ChunkRepository>(),
                            container.Resolve<LandscapeApplier>(),
                            container.Resolve<ITerrainFactory>(),
                            container.Resolve<IChunkNeighborConnector>()),
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