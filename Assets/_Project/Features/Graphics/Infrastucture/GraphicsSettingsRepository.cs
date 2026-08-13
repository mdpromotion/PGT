using _Project.Features.Core.Persistence;
using _Project.Features.Graphics.Domain;
using UnityEngine;

namespace _Project.Features.Graphics.Infrastucture
{
    public interface IGraphicsSettingsRepository
    {
        GraphicsData Load();
        void Save(GraphicsData data);
    }

    public class GraphicsSettingsRepository : IGraphicsSettingsRepository
    {
        private const string Category = "graphics";
        private const GraphicsType FallbackPreset = GraphicsType.Medium;

        private readonly IJsonReader _reader;
        private readonly IJsonWriter _writer;
        private readonly IGraphicsConfigResolver _resolver;

        public GraphicsSettingsRepository(
            IJsonReader reader,
            IJsonWriter writer,
            IGraphicsConfigResolver resolver)
        {
            _reader = reader;
            _writer = writer;
            _resolver = resolver;
        }

        public GraphicsData Load()
        {
            if (_reader.TryRead<GraphicsData>(Category, out var data))
                return data;

            var fallback = _resolver.GetDefaultGraphicsData(FallbackPreset);
            
            Debug.Log(fallback.HasValue ? fallback.Value : FallbackPreset);
            
            return fallback ?? default;
        }

        public void Save(GraphicsData data) =>
            _writer.Write(Category, data);
    }
}