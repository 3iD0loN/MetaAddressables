using System.Collections.Generic;

namespace USP.MetaAddressables
{
    public partial class ObjectComparer : IEqualityComparer<object>, IPropertyComparer
    {
        #region Static Fields
        public static readonly ObjectComparer Default = new ObjectComparer();
        #endregion

        #region Properties
        public IEnumerable<PropertyComparerPair> Children => null;
        #endregion

        #region Methods
        protected ObjectComparer()
        {
        }

        public new virtual bool Equals(object leftHand, object rightHand)
        {
            if (leftHand == rightHand)
            {
                return true;
            }

            if (rightHand == null || leftHand == null)
            {
                return false;
            }

            return leftHand.Equals(rightHand);
        }

        public virtual int GetHashCode(object target)
        {
            if (target == null)
            {
                return -1;
            }

            return target.GetHashCode();
        }
        #endregion
    }
}
