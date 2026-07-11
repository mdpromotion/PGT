using System;
using _Project.Features.Camera.Presentation;
using _Project.Features.Player.Application;
using _Project.Features.Player.Domain;
using _Project.Features.Player.Infrastructure;
using _Project.Features.Player.Presentation;
using _Project.Features.ProceduralWorld.Application;
using _Project.Features.ProceduralWorld.Domain;
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
        private NoiseSettings noiseSettings;

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
            
            builder.Register<InputSystem_Actions>(Lifetime.Singleton); 
            
            builder.Register<PlayerInputReader>(Lifetime.Singleton) 
                .As<IPlayerInputReader>() 
                .As<IInitializable>() 
                .As<IDisposable>();
            
            builder.RegisterComponentInHierarchy<FpsCameraController>();
            
            builder.Register<FpsMovementUseCase>(Lifetime.Singleton);
            
            builder.RegisterComponentInHierarchy<FpsPlayerMotor>() 
                .As<IFpsPlayerMotor>();
            
            builder.RegisterComponentInHierarchy<RigidbodyPlayerState>()
                .As<IPlayerReadOnly>();
        }


        private void RegisterProceduralWorld(
            IContainerBuilder builder)
        {
            builder.RegisterInstance(noiseSettings);


            builder.Register(
                container => new ChunkGrid(
                    chunkPrefab.terrainData.size.x,
                    chunkPrefab.terrainData.size.z),
                Lifetime.Singleton);


            builder.Register<IHeightmapGenerator,
                    PerlinHeightmapGenerator>(
                Lifetime.Singleton);


            builder.Register<UnityTerrainWriter>(
                Lifetime.Singleton);


            builder.Register<GenerateTerrainUseCase>(
                Lifetime.Singleton);


            builder.Register(
                container => new TerrainChunkFactory(
                    chunkPrefab,
                    container.Resolve<ChunkGrid>()),
                Lifetime.Singleton);


            builder.Register(
                container => new ChunkManager(
                    container.Resolve<TerrainChunkFactory>(),
                    container.Resolve<GenerateTerrainUseCase>(),
                    chunksParent),
                Lifetime.Singleton);


            builder.Register(
                container => new WorldStreamer(
                    container.Resolve<ChunkManager>(),
                    container.Resolve<ChunkGrid>(),
                    container.Resolve<NoiseSettings>(),
                    container.Resolve<IPlayerReadOnly>(),
                    viewDistance),
                Lifetime.Singleton);


            builder.RegisterComponentInHierarchy
                <ProceduralWorldPresenter>();
        }
    }
}