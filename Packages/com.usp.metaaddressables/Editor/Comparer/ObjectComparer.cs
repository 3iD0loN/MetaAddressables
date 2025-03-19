using System.Collections;
using System.Collections.Generic;

namespace USP.MetaAddressables
{
    public partial class ObjectComparer : IItemComparer
    {
        #region Static Fields
        public static readonly ObjectComparer Default = new ObjectComparer();
        #endregion

        #region Methods
        protected ObjectComparer()
        {
        }
        #endregion

        #region IEqualityComparer
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
                return default;
            }

            return target.GetHashCode();
        }
        #endregion
    }

    public class ObjectComparer<T> : ObjectComparer, IItemComparer<T>
    {
        #region Static Fields
        public new static readonly ObjectComparer<T> Default = new ObjectComparer<T>();
        #endregion

        #region Methods
        protected ObjectComparer()
        {
        }

        #region IEqualityComparer<T>
        public virtual bool Equals(T leftHand, T rightHand)
        {
            if (rightHand == null || leftHand == null)
            {
                return false;
            }

            return leftHand.Equals(rightHand);
        }

        public virtual int GetHashCode(T target)
        {
            if (target == null)
            {
                return default;
            }

            return target.GetHashCode();
        }
        #endregion
        #endregion
    }
}
