using System.Collections.Generic;

namespace USP.MetaAddressables
{
    public class KeyValuePairComparer<K, V> : PropertyComparer<KeyValuePair<K, V>>
    {
        public KeyValuePairComparer(IItemComparer<K> keyComparer, IItemComparer<V> valueComparer) :
            base(new PropertyComparerPair<KeyValuePair<K, V>, K>(x => x.Key, keyComparer),
                new PropertyComparerPair<KeyValuePair<K, V>, V>(x => x.Value, valueComparer))
        {
        }

        public KeyValuePairComparer(IItemComparer<K> keyComparer) :
            base(new PropertyComparerPair<KeyValuePair<K, V>, K>(x => x.Key, keyComparer))
        {
        }
    }

    public class DictionaryComparer<K, V> : EnumerableComparer<KeyValuePair<K, V>, KeyValuePairComparer<K, V>>
    {
        public DictionaryComparer(IItemComparer<K> keyComparer) :
            this(new KeyValuePairComparer<K, V>(keyComparer))
        {
        }

        public DictionaryComparer(IItemComparer<K> keyComparer, IItemComparer<V> valueComparer) :
            this(new KeyValuePairComparer<K, V>(keyComparer, valueComparer))
        {
        }

        public DictionaryComparer(KeyValuePairComparer<K, V> elementComparer) :
            base(elementComparer)
        {
        }
    }
}

