using System;
using System.Collections.Generic;

namespace USP.MetaAddressables
{
    public class GenericComparer<T> : ObjectComparer, IEqualityComparer<T>
    {
        #region Properties
        public Func<T, T, bool> Equality { get; }

        public Func<T, int> Hash { get; }
        #endregion

        #region Methods
        public GenericComparer(IEqualityComparer<T> comparer) :
            this(comparer.Equals, comparer.GetHashCode)
        {
        }

        public GenericComparer(Func<T, T, bool> equality = null, Func<T, int> hash = null)
        {
            Equality = equality != null ? equality : (x, y) => base.Equals(x as object, y as object);
            Hash = hash != null ? hash : x => base.GetHashCode(x as object);
        }

        public virtual int GetHashCode(T target)
        {
            return Hash != null ? Hash.Invoke(target) : default;
        }

        public virtual bool Equals(T leftHand, T rightHand)
        {
            return Equality != null ? Equality.Invoke(leftHand, rightHand) : false;
        }

        public override bool Equals(object leftHand, object rightHand)
        {
            return Equals((T)leftHand, (T)rightHand);
        }

        public override int GetHashCode(object target)
        {
            return GetHashCode((T)target);
        }
        #endregion
    }
}
