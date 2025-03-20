using System.Collections.Generic;

namespace USP.MetaAddressables
{
    public class KeyValuePairComparer<K, V> : PropertyComparer<KeyValuePair<K, V>>
    {
        public KeyValuePairComparer(IPropertyComparer keyComparer) :
            base((x => x.Key, keyComparer))
        {
        }

        public KeyValuePairComparer(IPropertyComparer keyComparer, IPropertyComparer valueComparer) :
            base((x => x.Key, keyComparer),
                (x => x.Value, valueComparer))
        {
        }
    }
}

