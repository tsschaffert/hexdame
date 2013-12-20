using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hexdame.Model
{
    class LimitedSizeDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>
    {
        public void Add(TKey key, TValue value)
        {
            base.TryAdd(key, value);
        }
    }
}
