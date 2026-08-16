using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Domain.Vegetation;
using _Project.Features.ProceduralWorld.Infrastructure.Vegetation;

namespace _Project.Features.ProceduralWorld.Application.Vegetation
{
    internal static class VegetationCategoryCombiner
    {
        public static List<VegetationCatalogEntry> Combine(
            VegetationCatalog catalog,
            VegetationCategory a,
            VegetationCategory b)
        {
            var entries = new List<VegetationCatalogEntry>();
            entries.AddRange(catalog.GetByCategory(a));
            entries.AddRange(catalog.GetByCategory(b));
            return entries;
        }
    }
}