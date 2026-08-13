using System;
using System.IO;
using UnityEngine;

namespace _Project.Features.Core.Persistence
{
    public interface IJsonWriter
    {
        void Write<T>(string category, T data);
    }

    public interface IJsonReader
    {
        bool TryRead<T>(string category, out T data);
    }

    public class JsonFileStore : IJsonWriter, IJsonReader
    {
        private readonly string _rootPath = UnityEngine.Application.persistentDataPath;

        public void Write<T>(string category, T data)
        {
            var path = GetPath(category);
            var json = JsonUtility.ToJson(data, prettyPrint: true);

            var tmpPath = path + ".tmp";
            File.WriteAllText(tmpPath, json);

            if (File.Exists(path))
                File.Delete(path);

            File.Move(tmpPath, path);
        }

        public bool TryRead<T>(string category, out T data)
        {
            var path = GetPath(category);

            if (!File.Exists(path))
            {
                data = default;
                return false;
            }

            try
            {
                var json = File.ReadAllText(path);
                data = JsonUtility.FromJson<T>(json);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[JsonFileStore] Failed to read '{category}': {e}");
                data = default;
                return false;
            }
        }

        private string GetPath(string category) =>
            Path.Combine(_rootPath, category + ".json");
    }
}