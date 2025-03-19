using System;
using System.Collections.Generic;

namespace USP.MetaAddressables
{
    using IGroupSchemaDataComparer = IEqualityComparer<MetaAddressables.GroupSchemaData>;

    public class GroupSchemaDataComparer : GenericComparer<MetaAddressables.GroupSchemaData>
    {
        private static readonly Dictionary<Type, IEqualityComparer<MetaAddressables.GroupSchemaData>> lookup = new Dictionary<Type, IGroupSchemaDataComparer>
        {
            //{ typeof(MetaAddressables.BundledAssetGroupSchemaData), new BundledAssetGroupSchemaDataComparer() },
            //{ typeof(MetaAddressables.ContentUpdateGroupSchemaData), new ContentUpdateGroupSchemaDataComparer() },
        };
        
        #region Static Methods
        public static IEqualityComparer<MetaAddressables.GroupSchemaData> GetComparer<T>() where T : MetaAddressables.GroupSchemaData
        {
            return GetComparer(typeof(T));
        }

        public static IEqualityComparer<MetaAddressables.GroupSchemaData> GetComparer(Type type)
        {
            bool found = lookup.TryGetValue(type, out IEqualityComparer<MetaAddressables.GroupSchemaData> value);

            if (!found)
            {
                throw new InvalidCastException();
            }

            return value;
        }

        private static bool EqualsInternal(MetaAddressables.GroupSchemaData leftHand, MetaAddressables.GroupSchemaData rightHand)
        {
            if (leftHand == rightHand)
            {
                return true;
            }

            if (rightHand == null || leftHand == null)
            {
                return false;
            }

            if (leftHand.GetType() != rightHand.GetType())
            {
                return false;
            }

            IGroupSchemaDataComparer comparer = GetComparer(leftHand.GetType());

            return comparer.Equals(leftHand, rightHand);
        }

        private static int GetHashCodeInternal(MetaAddressables.GroupSchemaData target)
        {
            if (target == null)
            {
                return 0;
            }

            IGroupSchemaDataComparer comparer = GetComparer(target.GetType());

            return comparer.GetHashCode(target);
        }
        #endregion

        #region Methods
        public GroupSchemaDataComparer() :
            base(EqualsInternal, GetHashCodeInternal)
        {
        }
        #endregion
    }
}

