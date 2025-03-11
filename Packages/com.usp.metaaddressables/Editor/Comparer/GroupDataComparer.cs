using System;
using System.Collections.Generic;

namespace USP.MetaAddressables
{
    public class GroupDataComparer : PropertyComparer<MetaAddressables.GroupData>
    {
        public GroupDataComparer() :
            base((x => x.IsReadOnly, ObjectComparer.Default),
                (x => x.SchemaData, 
                    new EnumerableComparer<KeyValuePair<Type, MetaAddressables.GroupSchemaData>>(
                        new KeyValuePairComparer<Type, MetaAddressables.GroupSchemaData>(ObjectComparer.Default, new GroupSchemaDataComparer()))))
        {
        }
    }
}

