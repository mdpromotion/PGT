using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using _Project.Features.ProceduralWorld.Application.Vegetation;
using _Project.Features.ProceduralWorld.Infrastructure.Vegetation;

namespace _Project.Features.ProceduralWorld.Tests.Vegetation
{
    public class DetailCatalogIndexTests
    {
        private GameObject _prefabA;
        private GameObject _prefabB;

        [SetUp]
        public void SetUp()
        {
            _prefabA = new GameObject("GrassPrefab");
            _prefabB = new GameObject("FlowerPrefab");
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_prefabA);
            Object.DestroyImmediate(_prefabB);
        }

        [Test]
        public void Constructor_BuildsOneDetailPrototypePerEntry()
        {
            var entries = new List<VegetationCatalogEntry>
            {
                new() { Prefab = _prefabA, UniformScaleRange = new Vector2(0.5f, 1.5f) },
                new() { Prefab = _prefabB, UniformScaleRange = new Vector2(0.2f, 0.8f) },
            };

            var index = new DetailCatalogIndex(entries);

            Assert.AreEqual(2, index.Prototypes.Length);
            Assert.AreEqual(_prefabA, index.Prototypes[0].prototype);
            Assert.AreEqual(0.5f, index.Prototypes[0].minWidth);
            Assert.AreEqual(1.5f, index.Prototypes[0].maxWidth);
        }

        [Test]
        public void TryGetLayerIndex_KnownPrefab_ReturnsCorrectIndex()
        {
            var entries = new List<VegetationCatalogEntry>
            {
                new() { Prefab = _prefabA },
                new() { Prefab = _prefabB },
            };

            var index = new DetailCatalogIndex(entries);

            Assert.IsTrue(index.TryGetLayerIndex(_prefabB, out int layer));
            Assert.AreEqual(1, layer);
        }

        [Test]
        public void Constructor_EmptyList_ProducesEmptyIndex()
        {
            var index = new DetailCatalogIndex(new List<VegetationCatalogEntry>());

            Assert.AreEqual(0, index.Entries.Count);
            Assert.AreEqual(0, index.Prototypes.Length);
        }
    }
}