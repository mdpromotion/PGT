using System;
using Cysharp.Threading.Tasks;

namespace _Project.Features.UI.Infrastructure
{
    public interface ILoadSceneService
    {
        bool IsLoaded(string sceneAddress);
        UniTask LoadAdditiveAsync(string sceneAddress, IProgress<float> progress = null);
        UniTask UnloadAsync(string sceneAddress);
    }
}