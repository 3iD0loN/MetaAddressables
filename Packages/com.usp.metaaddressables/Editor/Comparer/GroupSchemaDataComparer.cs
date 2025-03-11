using System;
using System.Collections.Generic;

namespace USP.MetaAddressables
{
    public class GroupSchemaDataComparer : GenericComparer<MetaAddressables.GroupSchemaData>
    {
        private static readonly Dictionary<Type, IPropertyComparer> lookup = new Dictionary<Type, IPropertyComparer>
        {
            { typeof(MetaAddressables.BundledAssetGroupSchemaData), new BundledAssetGroupSchemaDataComparer() },
            { typeof(MetaAddressables.ContentUpdateGroupSchemaData), new ContentUpdateGroupSchemaDataComparer() },
        };

        #region Static Methods
        public static IPropertyComparer GetComparer<T>() where T : MetaAddressables.GroupSchemaData
        {
            return GetComparer(typeof(T));
        }

        public static IPropertyComparer GetComparer(Type type)
        {
            bool found = lookup.TryGetValue(type, out IPropertyComparer value);

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

            IPropertyComparer comparer = GetComparer(leftHand.GetType());

            return comparer.Equals(leftHand, rightHand);
        }

        private static int GetHashCodeInternal(MetaAddressables.GroupSchemaData target)
        {
            if (target == null)
            {
                return 0;
            }

            IPropertyComparer comparer = GetComparer(target.GetType());

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

