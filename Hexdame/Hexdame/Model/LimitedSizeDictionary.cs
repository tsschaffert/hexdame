using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hexdame.Model
{
    class LimitedSizeDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public const int MAX_ITEMS = 2000000;

        private Queue<TKey> keyQueue;

        public LimitedSizeDictionary()
            : base()
        {
            keyQueue = new Queue<TKey>();
        }

        public new void Add(TKey key, TValue value)
        {
            if (keyQueue.Count >= MAX_ITEMS)
            {
                this.Remove(keyQueue.Dequeue());
            }

            keyQueue.Enqueue(key);
            base.Add(key, value);
        }
    }
}
