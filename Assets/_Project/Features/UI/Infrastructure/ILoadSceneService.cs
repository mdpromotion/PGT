using System;
using Cysharp.Threading.Tasks;

namespace _Project.Features.UI.Infrastructure
{
    public interface ILoadSceneService
    {
        UniTask LoadSceneAsync(string sceneAddress, IProgress<float> progress = null);
    }
}