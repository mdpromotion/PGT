using Unity.Collections;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    public struct NativeMinHeap
    {
        private NativeArray<int> _indices;
        private NativeArray<float> _keys;
        private int _count;

        public int Count => _count;

        public NativeMinHeap(int capacity, Allocator allocator)
        {
            _indices = new NativeArray<int>(capacity, allocator);
            _keys = new NativeArray<float>(capacity, allocator);
            _count = 0;
        }

        public void Push(int index, float key)
        {
            int i = _count++;
            _indices[i] = index;
            _keys[i] = key;

            while (i > 0)
            {
                int parent = (i - 1) / 2;

                if (_keys[parent] <= _keys[i])
                    break;

                Swap(i, parent);
                i = parent;
            }
        }

        public void Pop(out int index, out float key)
        {
            index = _indices[0];
            key = _keys[0];

            _count--;
            _indices[0] = _indices[_count];
            _keys[0] = _keys[_count];

            int i = 0;

            while (true)
            {
                int left = i * 2 + 1;
                int right = i * 2 + 2;
                int smallest = i;

                if (left < _count && _keys[left] < _keys[smallest]) smallest = left;
                if (right < _count && _keys[right] < _keys[smallest]) smallest = right;

                if (smallest == i)
                    break;

                Swap(i, smallest);
                i = smallest;
            }
        }

        private void Swap(int a, int b)
        {
            (_indices[a], _indices[b]) = (_indices[b], _indices[a]);
            (_keys[a], _keys[b]) = (_keys[b], _keys[a]);
        }

        public void Dispose()
        {
            _indices.Dispose();
            _keys.Dispose();
        }
    }
}