using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile]
    public struct BoxBlurFieldJob : IJob
    {
        public int PaddedSize;
        public int Radius;

        [ReadOnly] public NativeArray<float> Source;
        public NativeArray<float> Destination;

        public void Execute()
        {
            int size = PaddedSize;
            var temp = new NativeArray<float>(size * size, Allocator.Temp);

            for (int z = 0; z < size; z++)
                BlurLine(Source, temp, z * size, 1, size);

            for (int x = 0; x < size; x++)
                BlurLine(temp, Destination, x, size, size);

            temp.Dispose();
        }

        private void BlurLine(NativeArray<float> src, NativeArray<float> dst, int offset, int stride, int length)
        {
            int r = Radius;
            float sum = 0f;
            int count = 0;

            for (int k = -r; k <= r; k++)
            {
                if ((uint)k < (uint)length)
                {
                    sum += src[offset + k * stride];
                    count++;
                }
            }

            dst[offset] = sum / count;

            for (int i = 1; i < length; i++)
            {
                int addIdx = i + r;
                int removeIdx = i - r - 1;

                if ((uint)addIdx < (uint)length)
                {
                    sum += src[offset + addIdx * stride];
                    count++;
                }
                if ((uint)removeIdx < (uint)length)
                {
                    sum -= src[offset + removeIdx * stride];
                    count--;
                }

                dst[offset + i * stride] = sum / count;
            }
        }
    }
}