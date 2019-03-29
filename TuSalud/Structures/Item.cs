using System;
using System.Collections.Generic;
using System.Text;

namespace Structures
{
    public class Item<K, P> : IEquatable<Item<K, P>>
    {

        public K Key { get; set; }

        public P Pointer { get; set; }

        public bool Equals(Item<K, P> other)
        {
            return this.Key.Equals(other.Key) && this.Pointer.Equals(other.Pointer);
        }

    }
}
