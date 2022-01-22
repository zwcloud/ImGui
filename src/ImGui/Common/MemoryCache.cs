using System.Collections.Generic;

namespace ImGui.Internal
{
    public class MemoryCache
    {
        public void Set(int key, object value)
        {
            _cache[key] = value;
        }

        public T Get<T>(int key) where T : class
        {
            if (_cache.TryGetValue(key, out var value))
            {
                return value as T;
            }

            return null;
        }

        private readonly Dictionary<int, object> _cache = new Dictionary<int, object>();
    }
}