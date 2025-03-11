using System.Collections.Generic;

namespace USP.MetaAddressables
{
    public class KeyValuePairComparer<K, V> : PropertyComparer<KeyValuePair<K, V>>
    {
        public KeyValuePairComparer(IPropertyComparer keyComparer = null, IPropertyComparer valueComparer = null) :
            base((x => x.Key, keyComparer),
                (x => x.Value, valueComparer))
        {
        }
    }
}

