using Unity.Collections;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    internal struct NativeMinHeap
    {
        private NativeArray<float> _keys;
        private NativeArray<int> _values;
        private int _count;

        public int Count => _count;

        public NativeMinHeap(int capacity, Allocator allocator)
        {
            _keys = new NativeArray<float>(capacity, allocator);
            _values = new NativeArray<int>(capacity, allocator);
            _count = 0;
        }

        public void Push(int value, float key)
        {
            int i = _count++;
            _keys[i] = key;
            _values[i] = value;

            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (_keys[parent] <= _keys[i]) break;

                (_keys[parent], _keys[i]) = (_keys[i], _keys[parent]);
                (_values[parent], _values[i]) = (_values[i], _values[parent]);
                i = parent;
            }
        }

        public void Pop(out int value, out float key)
        {
            key = _keys[0];
            value = _values[0];

            _count--;
            _keys[0] = _keys[_count];
            _values[0] = _values[_count];

            int i = 0;
            while (true)
            {
                int left = i * 2 + 1;
                int right = i * 2 + 2;
                int smallest = i;

                if (left < _count && _keys[left] < _keys[smallest]) smallest = left;
                if (right < _count && _keys[right] < _keys[smallest]) smallest = right;
                if (smallest == i) break;

                (_keys[smallest], _keys[i]) = (_keys[i], _keys[smallest]);
                (_values[smallest], _values[i]) = (_values[i], _values[smallest]);
                i = smallest;
            }
        }

        public void Dispose()
        {
            _keys.Dispose();
            _values.Dispose();
        }
    }
}