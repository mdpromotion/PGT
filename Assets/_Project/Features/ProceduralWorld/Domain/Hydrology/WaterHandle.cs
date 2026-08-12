using UnityEngine;

namespace _Project.Features.ProceduralWorld.Domain.Hydrology
{
    public sealed class WaterHandle
    {
        public readonly Transform Root;
        public readonly MeshFilter Filter;
        public readonly Renderer Renderer;
        public readonly Mesh Mesh;
        public readonly MaterialPropertyBlock PropertyBlock;

        public Texture2D MaskTexture;

        public WaterHandle(Transform root, MeshFilter filter, Renderer renderer, Mesh mesh)
        {
            Root = root;
            Filter = filter;
            Renderer = renderer;
            Mesh = mesh;
            PropertyBlock = new MaterialPropertyBlock();
        }
    }
}