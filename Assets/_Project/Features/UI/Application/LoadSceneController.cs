using _Project.Features.Core.Infrastructure;
using Cysharp.Threading.Tasks;
using _Project.Features.UI.Infrastructure;
using VContainer;

namespace _Project.Features.UI.Application
{
    public class LoadSceneController
    {
        private readonly SceneTransitionService _sceneTransitionService;
        private readonly SceneDatabase _sceneDatabase;

        [Inject]
        public LoadSceneController(SceneTransitionService sceneTransitionService, SceneDatabase sceneDatabase)
        {
            _sceneTransitionService = sceneTransitionService;
            _sceneDatabase = sceneDatabase;
        }

        public UniTask LoadMenuScene() => LoadScene(SceneType.Game, SceneType.Menu);
        public UniTask LoadGameScene() => LoadScene(SceneType.Menu, SceneType.Game);
        
        private UniTask LoadScene(SceneType originSceneType, SceneType destinationSceneType)
        {
            var originScenePath = _sceneDatabase.GetScenePath(originSceneType);
            var destinationScenePath = _sceneDatabase.GetScenePath(destinationSceneType);
            
            return _sceneTransitionService.BeginAsync(originScenePath, destinationScenePath);
        }
    }
}