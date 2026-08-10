using Cysharp.Threading.Tasks;
using _Project.Features.UI.Infrastructure;
using VContainer;

namespace _Project.Features.UI.Application
{
    public class StartGameUseCase
    {
        private const string DefaultGameSceneAddress = "Assets/Scenes/Game.unity";

        private readonly ILoadSceneService _loadSceneService;

        [Inject]
        public StartGameUseCase(ILoadSceneService loadSceneService)
        {
            _loadSceneService = loadSceneService;
        }

        public UniTask ExecuteAsync()
        {
            return ExecuteAsync(DefaultGameSceneAddress);
        }
        
        public UniTask ExecuteAsync(string sceneAddress)
        {
            return _loadSceneService.LoadSceneAsync(sceneAddress);
        }
    }
}