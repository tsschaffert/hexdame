using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hexdame.Model
{
    class LimitedSizeDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public new void Add(TKey key, TValue value)
        {
            try
            {
                base.Add(key, value);
            }
            catch(Exception e)
            {

            }
        }
    }
}
