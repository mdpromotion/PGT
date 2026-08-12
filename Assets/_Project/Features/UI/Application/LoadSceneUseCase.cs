using _Project.Features.Core.Infrastructure;
using Cysharp.Threading.Tasks;
using _Project.Features.UI.Infrastructure;
using VContainer;

namespace _Project.Features.UI.Application
{
    public class LoadSceneUseCase
    {
        private readonly SceneTransitionService _sceneTransitionService;
        private readonly SceneDatabase _sceneDatabase;

        [Inject]
        public LoadSceneUseCase(SceneTransitionService sceneTransitionService,  SceneDatabase sceneDatabase)
        {
            _sceneTransitionService = sceneTransitionService;
            _sceneDatabase = sceneDatabase;
        }

        public string GetMenuScenePath() => _sceneDatabase.GetScenePath(SceneType.Menu);
        public string GetGameScenePath() => _sceneDatabase.GetScenePath(SceneType.Game);

        public UniTask ExecuteAsync() => ExecuteAsync(GetGameScenePath());

        public UniTask ExecuteAsync(string sceneAddress) =>
            _sceneTransitionService.BeginAsync(GetMenuScenePath(), sceneAddress);
    }
}