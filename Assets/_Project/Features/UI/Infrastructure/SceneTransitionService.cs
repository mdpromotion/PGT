using Cysharp.Threading.Tasks;
using _Project.Features.UI.LoadingScreen.View;

namespace _Project.Features.UI.Infrastructure
{
    public class SceneTransitionService
    {
        private readonly ILoadSceneService _loadSceneService;
        private readonly LoadingScreenView _loadingScreenView;

        private string _currentSceneAddress;
        private bool _isTransitioning;

        public SceneTransitionService(
            ILoadSceneService loadSceneService,
            LoadingScreenView loadingScreenView)
        {
            _loadSceneService = loadSceneService;
            _loadingScreenView = loadingScreenView;
        }
        
        public async UniTask BeginAsync(string fromSceneAddress, string toSceneAddress)
        {
            if (_isTransitioning)
                return;

            _isTransitioning = true;

            await _loadingScreenView.FadeInAsync();
            await _loadSceneService.LoadAdditiveAsync(toSceneAddress);

            if (!string.IsNullOrEmpty(fromSceneAddress))
                await _loadSceneService.UnloadAsync(fromSceneAddress);

            _currentSceneAddress = toSceneAddress;
        }
        
        public async UniTask CompleteAsync()
        {
            if (!_isTransitioning)
                return;

            await _loadingScreenView.FadeOutAsync();
            _isTransitioning = false;
        }
    }
}