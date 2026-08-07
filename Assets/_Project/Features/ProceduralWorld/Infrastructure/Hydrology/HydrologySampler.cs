using Unity.Collections;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public static class HydrologySampler
    {
        public static float SampleBilinear(
            NativeArray<float> grid,
            int resolution,
            float gx,
            float gz)
        {
            int maxIndex = resolution - 1;

            gx = Mathf.Clamp(gx, 0f, maxIndex);
            gz = Mathf.Clamp(gz, 0f, maxIndex);

            int x0 = Mathf.FloorToInt(gx);
            int z0 = Mathf.FloorToInt(gz);

            int x1 = Mathf.Min(x0 + 1, maxIndex);
            int z1 = Mathf.Min(z0 + 1, maxIndex);

            float tx = gx - x0;
            float tz = gz - z0;

            float v00 = grid[z0 * resolution + x0];
            float v10 = grid[z0 * resolution + x1];
            float v01 = grid[z1 * resolution + x0];
            float v11 = grid[z1 * resolution + x1];

            float top = Mathf.Lerp(v00, v10, tx);
            float bottom = Mathf.Lerp(v01, v11, tx);

            return Mathf.Lerp(top, bottom, tz);
        }
    }
}