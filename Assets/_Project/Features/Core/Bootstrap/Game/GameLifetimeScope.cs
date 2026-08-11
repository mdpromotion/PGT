using System.Collections.Generic;
using _Project.Features.Camera.Infrastructure;
using _Project.Features.Core.Application;
using _Project.Features.GameTime.Application;
using _Project.Features.GameTime.Domain;
using _Project.Features.GameTime.Presentation;
using _Project.Features.Player.Application;
using _Project.Features.Player.Domain;
using _Project.Features.Player.Infrastructure;
using _Project.Features.Player.Presentation;
using _Project.Features.ProceduralWorld.Application.Chunks;
using _Project.Features.ProceduralWorld.Application.Chunks.Generation;
using _Project.Features.ProceduralWorld.Application.Interfaces;
using _Project.Features.ProceduralWorld.Application.Landscape;
using _Project.Features.ProceduralWorld.Application.Vegetation;
using _Project.Features.ProceduralWorld.Application.World;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Hydrology;
using _Project.Features.ProceduralWorld.Domain.World;
using _Project.Features.ProceduralWorld.Infrastructure;
using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;
using _Project.Features.ProceduralWorld.Infrastructure.Interfaces;
using _Project.Features.ProceduralWorld.Infrastructure.Landscape;
using _Project.Features.ProceduralWorld.Infrastructure.Vegetation;
using _Project.Features.ProceduralWorld.Presentation;
using _Project.Features.Shared.Application;
using _Project.Features.Sound.Application;
using _Project.Features.Sound.Infrastructure;
using _Project.Features.Sound.Presentation;
using _Project.Features.Tick;
using _Project.Features.Tick.Application;
using _Project.Features.Tick.Domain;
using _Project.Features.UI.DebugMenu;
using _Project.Features.UI.Infrastructure;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Features.Core.Bootstrap.Game
{
    public class GameLifetimeScope : LifetimeScope
    {
        [Header("Procedural World")]
        [SerializeField] private Terrain chunkPrefab;
        [SerializeField] private WorldSettings worldSettings;
        [SerializeField] private MacroGridSettings macroGridSettings;
        [SerializeField] private RiverCarvingSettings riverCarvingSettings;
        [SerializeField] private VegetationGridSettings vegetationGridSettings;
        [SerializeField] private VegetationCatalog vegetationCatalog;
        [SerializeField] private Material waterMaterial;
        [SerializeField] private Transform chunksParent;
        [SerializeField] private int viewDistance = 3;

        [Header("Sound")]
        [SerializeField] private SoundDatabase database;
        [SerializeField] private int globalMaxVoices = 32;
        [SerializeField] private PlayerSoundSet playerSoundSet;

        [Header("Player")]
        [SerializeField] private PlayerMovementConfig playerMovementConfig;

        protected override void Configure(IContainerBuilder builder)
        { 
            RegisterSound(builder);
            RegisterPlayer(builder);
            RegisterTickSystem(builder);
            RegisterGameTimeSystem(builder);
            RegisterProceduralWorld(builder);
            RegisterCore(builder);
            RegisterUI(builder);
        }

        private void RegisterSound(IContainerBuilder builder)
        {
            builder.RegisterInstance(database);

            builder.Register(_ => new SoundPlaybackGuard(globalMaxVoices), Lifetime.Singleton);

            builder.RegisterComponentOnNewGameObject<SoundVoicePool>(Lifetime.Singleton, "SoundVoicePool")
                .DontDestroyOnLoad();

            builder.Register<SoundService>(Lifetime.Singleton)
                .As<ISoundService>();
        }

        private void RegisterPlayer(IContainerBuilder builder)
        {
            builder.Register<InputSystem_Actions>(Lifetime.Singleton);

            builder.Register<PlayerInputReader>(Lifetime.Singleton)
                .As<IPlayerInputReader>()
                .As<IInitializable>();

            builder.RegisterComponentInHierarchy<FpsCameraController>();

            builder.RegisterInstance(playerMovementConfig)
                .AsSelf();

            builder.Register<GroundMovementUseCase>(Lifetime.Singleton);

            builder.Register<SwimmingMovementUseCase>(Lifetime.Singleton);

            builder.RegisterComponentInHierarchy<PlayerStanceController>()
                .As<IPlayerStanceState>();

            builder.RegisterComponentInHierarchy<FpsPlayerMotor>()
                .As<IFpsPlayerMotor>();
            
            builder.Register<PlayerController>(Lifetime.Singleton)
                .As<IFixedTickable>()
                .As<IPlayerController>();

            builder.RegisterComponentInHierarchy<RigidbodyPlayerState>()
                .As<IPlayerReadOnly>();

            builder.RegisterComponentInHierarchy<WaterVolumeTracker>()
                .As<IWaterState>();

            builder.RegisterComponentInHierarchy<PlayerWaterSoundController>();

            builder.Register(
                    container => new WaterQueryService(
                        container.Resolve<ChunkGrid>(),
                        container.Resolve<IChunkLookup>(),
                        chunkPrefab.terrainData.size.y),
                    Lifetime.Singleton)
                .As<IWaterQuery>();

            builder.RegisterInstance(playerSoundSet);
            
            builder.RegisterComponentInHierarchy<FootstepController>();
        }

        private void RegisterTickSystem(IContainerBuilder builder)
        {
            builder.Register<TickData>(Lifetime.Singleton);
            
            builder.Register<TickController>(Lifetime.Singleton)
                .As<IFixedTickable>()
                .As<ITick>();

            builder.Register<TickDebug>(Lifetime.Singleton)
                .As<IInitializable>()
                .AsSelf();
        }

        private void RegisterGameTimeSystem(IContainerBuilder builder)
        {
            builder.Register<GameTime.Domain.GameTime>(Lifetime.Singleton)
                .As<IGameTime>()
                .AsSelf();

            builder.Register<GameTimeController>(Lifetime.Singleton)
                .As<IInitializable>();

            builder.RegisterComponentInHierarchy<GameTimePresenter>();
        }

        private void RegisterProceduralWorld(IContainerBuilder builder)
        {
            builder.RegisterInstance(worldSettings);
            builder.RegisterInstance(macroGridSettings);
            builder.RegisterInstance(riverCarvingSettings);
            builder.RegisterInstance(vegetationGridSettings);
            builder.RegisterInstance(vegetationCatalog);

            // Grid / caches
            builder.Register(
                    container => new ChunkGrid(
                        chunkPrefab.terrainData.size.x,
                        chunkPrefab.terrainData.size.z),
                    Lifetime.Singleton);

            builder.Register<VegetationTileCache>(Lifetime.Singleton)
                .AsSelf()
                .As<IGenerationCacheEvictor>();

            builder.Register<MacroRegionCache>(Lifetime.Singleton);

            builder.Register<ChunkRepository>(Lifetime.Singleton)
                .AsSelf()
                .As<IChunkLookup>();

            // Appliers
            builder.Register(
                    container => new WaterSurfaceApplier(
                        container.Resolve<ChunkGrid>(),
                        chunkPrefab.terrainData.size.y,
                        waterMaterial),
                    Lifetime.Singleton);

            builder.Register<VegetationApplier>(Lifetime.Singleton);

            builder.Register<TerrainNoiseSettingsProvider>(Lifetime.Singleton);

            builder.Register<UnityTerrainWriter>(Lifetime.Singleton)
                .As<ITerrainWriter>();

            builder.Register<ChunkNeighborConnector>(Lifetime.Singleton)
                .As<IChunkNeighborConnector>();

            builder.Register(
                    container => new LandscapeChunkFactory(
                        chunkPrefab,
                        container.Resolve<ChunkGrid>()),
                    Lifetime.Singleton)
                .As<ILandscapeFactory>();

            builder.Register(
                    container => new LandscapeApplier(
                        container.Resolve<ILandscapeFactory>(),
                        container.Resolve<ITerrainWriter>(),
                        container.Resolve<IChunkNeighborConnector>(),
                        container.Resolve<ChunkRepository>(),
                        container.Resolve<WaterSurfaceApplier>(),
                        container.Resolve<VegetationApplier>(),
                        chunksParent),
                    Lifetime.Singleton);

            // Generation pipeline
            builder.Register<LandscapeGenerator>(Lifetime.Singleton)
                .As<IGenerationStage>();

            builder.Register(
                    container => new HydrologyGenerator(
                        container.Resolve<ChunkGrid>(),
                        container.Resolve<MacroRegionCache>(),
                        container.Resolve<MacroGridSettings>(),
                        localAccumulationNormalizationRange: 16f),
                    Lifetime.Singleton)
                .As<IGenerationStage>();

            builder.Register<WaterSurfaceStage>(Lifetime.Singleton)
                .As<IGenerationStage>();

            builder.Register(
                    container => new VegetationGenerator(
                        container.Resolve<ChunkGrid>(),
                        container.Resolve<VegetationTileCache>(),
                        container.Resolve<VegetationGridSettings>(),
                        container.Resolve<WorldSettings>(),
                        chunkPrefab.terrainData.size.y),
                    Lifetime.Singleton)
                .As<IGenerationStage>();

            builder.Register<ChunkGenerationPipeline>(Lifetime.Singleton)
                .AsSelf()
                .As<IChunkGenerator>();

            builder.RegisterBuildCallback(container =>
            {
                ChunkGenerationPipeline pipeline = container.Resolve<ChunkGenerationPipeline>();

                foreach (IGenerationStage stage in container.Resolve<IEnumerable<IGenerationStage>>())
                {
                    pipeline.Add(stage);
                }
            });

            // Streaming / management
            builder.Register<ChunkGenerationScheduler>(Lifetime.Singleton);

            builder.Register<ChunkManager>(Lifetime.Singleton);

            builder.Register(
                    container => new WorldStreamer(
                        container.Resolve<ChunkManager>(),
                        container.Resolve<ChunkGrid>(),
                        container.Resolve<IPlayerReadOnly>(),
                        viewDistance,
                        container.Resolve<IEnumerable<IGenerationCacheEvictor>>()),
                    Lifetime.Singleton);

            builder.RegisterComponentInHierarchy<ProceduralWorldPresenter>();
        }
        
        private void RegisterCore(IContainerBuilder builder)
        {
            builder.Register<CoreGameLoop>(Lifetime.Singleton)
                .As<IInitializable>();
        }

        private void RegisterUI(IContainerBuilder builder)
        {
            builder.Register<FPSCounter>(Lifetime.Singleton)
                .As<ITickable>()
                .As<IFPSCounter>();
            
            builder.RegisterComponentInHierarchy<DebugMenuPresenter>();
        }
    }
}