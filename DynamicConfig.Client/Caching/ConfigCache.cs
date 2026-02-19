using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicConfig.Client.Models;

namespace DynamicConfig.Client.Caching
{
    internal class ConfigCache
    {
        private readonly ConcurrentDictionary<string, ConfigItem> _cache = new();

        public void Set(IEnumerable<ConfigItem> items)
        {
            _cache.Clear();

            foreach (var item in items)
            {
                _cache[item.Name] = item;
            }
        }

        public void Update(ConfigItem item)
        {
            _cache[item.Name] = item;
        }

        public bool TryGet(string key, out ConfigItem item)
        {
            return _cache.TryGetValue(key, out item);
        }
    }
}
