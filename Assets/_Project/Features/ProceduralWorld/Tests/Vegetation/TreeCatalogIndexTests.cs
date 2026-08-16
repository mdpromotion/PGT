using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using _Project.Features.ProceduralWorld.Application.Vegetation;
using _Project.Features.ProceduralWorld.Infrastructure.Vegetation;

namespace _Project.Features.ProceduralWorld.Tests.Vegetation
{
    public class TreeCatalogIndexTests
    {
        private GameObject _prefabA;
        private GameObject _prefabB;

        [SetUp]
        public void SetUp()
        {
            _prefabA = new GameObject("PrefabA");
            _prefabB = new GameObject("PrefabB");
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_prefabA);
            Object.DestroyImmediate(_prefabB);
        }

        [Test]
        public void Constructor_BuildsOnePrototypePerEntry()
        {
            var entries = new List<VegetationCatalogEntry>
            {
                new() { Prefab = _prefabA },
                new() { Prefab = _prefabB },
            };

            var index = new TreeCatalogIndex(entries);

            Assert.AreEqual(2, index.Prototypes.Length);
            Assert.AreEqual(_prefabA, index.Prototypes[0].prefab);
            Assert.AreEqual(_prefabB, index.Prototypes[1].prefab);
        }

        [Test]
        public void TryGetPrototypeIndex_KnownPrefab_ReturnsCorrectIndex()
        {
            var entries = new List<VegetationCatalogEntry>
            {
                new() { Prefab = _prefabA },
                new() { Prefab = _prefabB },
            };

            var index = new TreeCatalogIndex(entries);

            Assert.IsTrue(index.TryGetPrototypeIndex(_prefabB, out int idx));
            Assert.AreEqual(1, idx);
        }

        [Test]
        public void TryGetPrototypeIndex_UnknownPrefab_ReturnsFalse()
        {
            var entries = new List<VegetationCatalogEntry> { new() { Prefab = _prefabA } };
            var index = new TreeCatalogIndex(entries);

            var unknown = new GameObject("Unknown");
            try
            {
                Assert.IsFalse(index.TryGetPrototypeIndex(unknown, out _));
            }
            finally
            {
                Object.DestroyImmediate(unknown);
            }
        }

        [Test]
        public void Constructor_DuplicatePrefabAcrossEntries_LastEntryWins()
        {
            var entries = new List<VegetationCatalogEntry>
            {
                new() { Prefab = _prefabA },
                new() { Prefab = _prefabA },
            };

            var index = new TreeCatalogIndex(entries);

            Assert.IsTrue(index.TryGetPrototypeIndex(_prefabA, out int idx));
            Assert.AreEqual(1, idx);
        }

        [Test]
        public void Constructor_EmptyList_ProducesEmptyIndex()
        {
            var index = new TreeCatalogIndex(new List<VegetationCatalogEntry>());

            Assert.AreEqual(0, index.Entries.Count);
            Assert.AreEqual(0, index.Prototypes.Length);
        }
    }
}