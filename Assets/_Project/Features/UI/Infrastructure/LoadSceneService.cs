using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace _Project.Features.UI.Infrastructure
{
    public class LoadSceneService : ILoadSceneService
    {
        public async UniTask LoadSceneAsync(string sceneAddress, IProgress<float> progress = null)
        {
            if (string.IsNullOrEmpty(sceneAddress))
                throw new ArgumentException("Scene address must not be null or empty.", nameof(sceneAddress));

            AsyncOperationHandle<SceneInstance> handle =
                Addressables.LoadSceneAsync(sceneAddress, LoadSceneMode.Single);

            while (!handle.IsDone)
            {
                progress?.Report(handle.PercentComplete);
                await UniTask.Yield();
            }

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[LoadSceneService] Failed to load scene by address: {sceneAddress}");
                Addressables.Release(handle);
                throw handle.OperationException ?? new Exception("Failed to load scene via Addressables.");
            }

            progress?.Report(1f);
        }
    }
}