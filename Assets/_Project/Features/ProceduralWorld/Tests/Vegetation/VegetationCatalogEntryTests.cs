using NUnit.Framework;
using _Project.Features.ProceduralWorld.Infrastructure.Vegetation;

namespace _Project.Features.ProceduralWorld.Tests.Vegetation
{
    public class VegetationCatalogEntryTests
    {
        private static VegetationCatalogEntry MakeEntry(
            float minHeight, float maxHeight, float minSlope, float maxSlope)
        {
            return new VegetationCatalogEntry
            {
                MinHeight01 = minHeight,
                MaxHeight01 = maxHeight,
                MinSlopeDegrees = minSlope,
                MaxSlopeDegrees = maxSlope,
            };
        }

        [Test]
        public void Matches_ExactLowerBounds_ReturnsTrue()
        {
            var entry = MakeEntry(0.2f, 0.8f, 5f, 30f);
            Assert.IsTrue(entry.Matches(0.2f, 5f));
        }

        [Test]
        public void Matches_ExactUpperBounds_ReturnsTrue()
        {
            var entry = MakeEntry(0.2f, 0.8f, 5f, 30f);
            Assert.IsTrue(entry.Matches(0.8f, 30f));
        }

        [Test]
        public void Matches_JustBelowLowerHeightBound_ReturnsFalse()
        {
            var entry = MakeEntry(0.2f, 0.8f, 5f, 30f);
            Assert.IsFalse(entry.Matches(0.199f, 10f));
        }

        [Test]
        public void Matches_JustAboveUpperSlopeBound_ReturnsFalse()
        {
            var entry = MakeEntry(0.2f, 0.8f, 5f, 30f);
            Assert.IsFalse(entry.Matches(0.5f, 30.001f));
        }

        [Test]
        public void Matches_FullRange_AlwaysTrue()
        {
            var entry = MakeEntry(0f, 1f, 0f, 90f);
            Assert.IsTrue(entry.Matches(0f, 0f));
            Assert.IsTrue(entry.Matches(1f, 90f));
            Assert.IsTrue(entry.Matches(0.5f, 45f));
        }
    }
}