using NUnit.Framework;
using UnityEngine;
using _Project.Features.ProceduralWorld.Domain.Vegetation;

namespace _Project.Features.ProceduralWorld.Tests.Vegetation
{
    public class GameObjectInstancePoolTests
    {
        private GameObject _root;
        private GameObject _prefab;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject("PoolRoot");
            _prefab = new GameObject("RockPrefab");
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_root);
            Object.DestroyImmediate(_prefab);
        }

        [Test]
        public void Rent_FirstTime_InstantiatesNewObject()
        {
            var pool = new GameObjectInstancePool(_root.transform);

            pool.BeginFrame();
            GameObject instance = pool.Rent(_prefab);
            pool.EndFrame();

            Assert.IsNotNull(instance);
            Assert.AreNotSame(_prefab, instance);
            Assert.AreEqual(_root.transform, instance.transform.parent);
            Assert.IsTrue(instance.activeSelf);
        }

        [Test]
        public void Rent_SameCountAcrossFrames_DoesNotGrowPool()
        {
            var pool = new GameObjectInstancePool(_root.transform);

            pool.BeginFrame();
            for (int i = 0; i < 5; i++) pool.Rent(_prefab);
            pool.EndFrame();

            int sizeAfterFirstFrame = pool.GetPoolSize(_prefab);

            pool.BeginFrame();
            for (int i = 0; i < 5; i++) pool.Rent(_prefab);
            pool.EndFrame();

            Assert.AreEqual(5, sizeAfterFirstFrame);
            Assert.AreEqual(5, pool.GetPoolSize(_prefab));
        }

        [Test]
        public void Rent_FewerThanPreviousFrame_DeactivatesUnusedInstances()
        {
            var pool = new GameObjectInstancePool(_root.transform);

            pool.BeginFrame();
            GameObject[] first = { pool.Rent(_prefab), pool.Rent(_prefab), pool.Rent(_prefab) };
            pool.EndFrame();

            pool.BeginFrame();
            pool.Rent(_prefab);
            pool.EndFrame();

            Assert.IsTrue(first[0].activeSelf);
            Assert.IsFalse(first[1].activeSelf);
            Assert.IsFalse(first[2].activeSelf);
            Assert.AreEqual(3, pool.GetPoolSize(_prefab));
        }

        [Test]
        public void Rent_MoreThanPreviousFrame_GrowsPoolAndReactivatesAll()
        {
            var pool = new GameObjectInstancePool(_root.transform);

            pool.BeginFrame();
            pool.Rent(_prefab);
            pool.EndFrame();

            pool.BeginFrame();
            for (int i = 0; i < 4; i++) pool.Rent(_prefab);
            pool.EndFrame();

            Assert.AreEqual(4, pool.GetPoolSize(_prefab));
        }

        [Test]
        public void Rent_ZeroInstancesRequested_DeactivatesEverything()
        {
            var pool = new GameObjectInstancePool(_root.transform);

            pool.BeginFrame();
            GameObject a = pool.Rent(_prefab);
            GameObject b = pool.Rent(_prefab);
            pool.EndFrame();
            
            pool.BeginFrame();
            pool.EndFrame();

            Assert.IsFalse(a.activeSelf);
            Assert.IsFalse(b.activeSelf);
        }

        [Test]
        public void Rent_ExternallyDestroyedPooledInstance_IsRecreatedWithoutThrowing()
        {
            var pool = new GameObjectInstancePool(_root.transform);

            pool.BeginFrame();
            GameObject instance = pool.Rent(_prefab);
            pool.EndFrame();

            Object.DestroyImmediate(instance);

            Assert.DoesNotThrow(() =>
            {
                pool.BeginFrame();
                GameObject recreated = pool.Rent(_prefab);
                pool.EndFrame();

                Assert.IsNotNull(recreated);
            });
        }

        [Test]
        public void MultiplePrefabs_AreTrackedIndependently()
        {
            var otherPrefab = new GameObject("OtherPrefab");
            try
            {
                var pool = new GameObjectInstancePool(_root.transform);

                pool.BeginFrame();
                pool.Rent(_prefab);
                pool.Rent(_prefab);
                pool.Rent(otherPrefab);
                pool.EndFrame();

                Assert.AreEqual(2, pool.GetPoolSize(_prefab));
                Assert.AreEqual(1, pool.GetPoolSize(otherPrefab));
            }
            finally
            {
                Object.DestroyImmediate(otherPrefab);
            }
        }
    }
}