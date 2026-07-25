using Unity.Collections;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public static class HydrologySampler
    {
        public static float SampleBilinear(
            NativeArray<float> data,
            int resolution,
            float gx,
            float gz)
        {
            int x0 = Mathf.Clamp(Mathf.FloorToInt(gx), 0, resolution - 1);
            int z0 = Mathf.Clamp(Mathf.FloorToInt(gz), 0, resolution - 1);
            int x1 = Mathf.Min(x0 + 1, resolution - 1);
            int z1 = Mathf.Min(z0 + 1, resolution - 1);

            float fx = gx - x0;
            float fz = gz - z0;

            float v00 = data[z0 * resolution + x0];
            float v10 = data[z0 * resolution + x1];
            float v01 = data[z1 * resolution + x0];
            float v11 = data[z1 * resolution + x1];

            float top = Mathf.Lerp(v00, v10, fx);
            float bottom = Mathf.Lerp(v01, v11, fx);
            return Mathf.Lerp(top, bottom, fz);
        }
    }
}