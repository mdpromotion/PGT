using System.Collections.Generic;
using NUnit.Framework;
using _Project.Features.ProceduralWorld.Application.Vegetation;
using _Project.Features.ProceduralWorld.Infrastructure.Vegetation;

namespace _Project.Features.ProceduralWorld.Tests.Vegetation
{
    public class VegetationEntryPickerTests
    {
        private static VegetationCatalogEntry MakeEntry(float weight, float minHeight = 0f, float maxHeight = 1f)
        {
            return new VegetationCatalogEntry
            {
                Weight = weight,
                MinHeight01 = minHeight,
                MaxHeight01 = maxHeight,
                MinSlopeDegrees = 0f,
                MaxSlopeDegrees = 90f,
            };
        }

        [Test]
        public void Pick_NoEntries_ReturnsNull()
        {
            var picker = new VegetationEntryPicker();
            var result = picker.Pick(new List<VegetationCatalogEntry>(), 0.5f, 10f, 12345);
            Assert.IsNull(result);
        }

        [Test]
        public void Pick_NoMatchingEntry_ReturnsNull()
        {
            var entries = new List<VegetationCatalogEntry> { MakeEntry(1f, 0.9f, 1f) };
            var picker = new VegetationEntryPicker();

            var result = picker.Pick(entries, 0.1f, 10f, 12345);

            Assert.IsNull(result);
        }

        [Test]
        public void Pick_AllMatchingWeightsZero_ReturnsNull()
        {
            var entries = new List<VegetationCatalogEntry> { MakeEntry(0f), MakeEntry(0f) };
            var picker = new VegetationEntryPicker();

            var result = picker.Pick(entries, 0.5f, 10f, 999);

            Assert.IsNull(result);
        }

        [Test]
        public void Pick_SingleMatchingEntry_AlwaysReturnsIt()
        {
            var entry = MakeEntry(1f);
            var entries = new List<VegetationCatalogEntry> { entry };
            var picker = new VegetationEntryPicker();

            for (uint seed = 1; seed < 200; seed++)
                Assert.AreSame(entry, picker.Pick(entries, 0.5f, 10f, seed));
        }

        [Test]
        public void Pick_SameSeedAndInputs_IsDeterministic()
        {
            var entries = new List<VegetationCatalogEntry>
            {
                MakeEntry(1f), MakeEntry(2f), MakeEntry(3f),
            };

            var pickerA = new VegetationEntryPicker();
            var pickerB = new VegetationEntryPicker();

            for (uint seed = 1; seed < 500; seed++)
            {
                var a = pickerA.Pick(entries, 0.42f, 12.5f, seed);
                var b = pickerB.Pick(entries, 0.42f, 12.5f, seed);
                Assert.AreSame(a, b, $"Mismatch for seed {seed}");
            }
        }

        [Test]
        public void Pick_SeedZero_TreatedSameAsSeedOne()
        {
            var entries = new List<VegetationCatalogEntry> { MakeEntry(1f), MakeEntry(1f) };
            var picker = new VegetationEntryPicker();
            
            var resultZero = picker.Pick(entries, 0.5f, 10f, 0);
            var resultOne = picker.Pick(entries, 0.5f, 10f, 1);

            Assert.AreSame(resultOne, resultZero);
        }

        [Test]
        public void Pick_AllWeightedEntries_AreEventuallySelected()
        {
            var e1 = MakeEntry(1f);
            var e2 = MakeEntry(1f);
            var e3 = MakeEntry(1f);
            var entries = new List<VegetationCatalogEntry> { e1, e2, e3 };

            var picker = new VegetationEntryPicker();
            var seen = new HashSet<VegetationCatalogEntry>();

            for (uint i = 1; i < 2000; i++)
            {
                uint seed = i * 2654435761u;
                var result = picker.Pick(entries, 0.5f, 10f, seed);
                if (result != null)
                    seen.Add(result);
            }

            Assert.AreEqual(3, seen.Count);
        }

        [Test]
        public void Pick_LargeEntryList_DoesNotThrow()
        {
            var entries = new List<VegetationCatalogEntry>();
            for (int i = 0; i < 1000; i++)
                entries.Add(MakeEntry(i % 5));

            var picker = new VegetationEntryPicker();

            Assert.DoesNotThrow(() =>
            {
                for (uint seed = 1; seed < 100; seed++)
                    picker.Pick(entries, 0.5f, 10f, seed);
            });
        }
    }
}