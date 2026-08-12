using _Project.Features.Core.Infrastructure;
using _Project.Features.UI.Infrastructure;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace _Project.Features.Core.Bootstrap
{
    public sealed class BootstrapEntryPoint : IInitializable
    {
        private readonly ILoadSceneService _sceneService;
        private readonly SceneDatabase _sceneDatabase;

        public BootstrapEntryPoint(ILoadSceneService sceneService, SceneDatabase sceneDatabase)
        {
            _sceneService = sceneService;
            _sceneDatabase = sceneDatabase;
        }

        public void Initialize()
        {
            var scenePath = _sceneDatabase.GetScenePath(SceneType.Menu);
            _sceneService.LoadAdditiveAsync(scenePath).Forget();;
        }
        
    }
}