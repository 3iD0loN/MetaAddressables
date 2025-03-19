using System;
using System.Collections.Generic;

namespace USP.MetaAddressables
{
    using static USP.MetaAddressables.MetaAddressables;
    using KeyComparer = ObjectComparer<Type>;
    using SchemaDataComparer = DictionaryComparer<Type, MetaAddressables.GroupSchemaData>;

    public class GroupDataComparer : PropertyComparer<GroupData>
    {
        SchemaDataComparer x = new SchemaDataComparer(ObjectComparer<Type>.Default, new GroupSchemaDataComparer());

        public GroupDataComparer() :
            base(new PropertyComparerPair<GroupData, bool>(x => x.IsReadOnly, ObjectComparer<bool>.Default),
                new PropertyComparerPair<GroupData, Dictionary<Type, GroupSchemaData>>(x => x.SchemaData, null))
        {
        }
    }
}
